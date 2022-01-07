using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetSelectManager  {
    public MapManager mapManager;
    public AttackManager attackManager;
    private Unit unit;
    private Vector2 unitEndPos;
    private GameObject curTarget;
    internal CircularList<GameObject> validAttacks;

    //used in actionselect, also sets state
    public TargetSelectManager(Unit newSelection) {
        Initialize(newSelection);
        curTarget = validAttacks[0];
        mapManager.cursor.SetPos(curTarget.transform.position);
        mapManager.SetState(GameState.PlayerChoosingTarget); 
        DisplayAttackInfo();
    }

    //used everywhere else
    public TargetSelectManager(Unit newSelection, Vector2 targetPos, Vector2 endPos) {
        Initialize(newSelection);
        curTarget = unit.grid.GetObject(targetPos);
        if (curTarget.GetComponent<Unit>().team == unit.team || curTarget == null) {
            mapManager.SetState(GameState.PlayerHasSelection);
            return;  
        }        
        unitEndPos = endPos;
        mapManager.SetState(GameState.PlayerChoosingTarget); 
        DisplayAttackInfo();
    }

    private void Initialize(Unit newSelection) {
        unit = newSelection;
        validAttacks = unit.validAttacks;
        mapManager = unit.grid.manager;
        attackManager = mapManager.attackManager;
        unitEndPos = unit.gridPos;
    }

    public IEnumerator ChooseTarget() {
        mapManager.SetState(GameState.NoControl);
        //mapManager.grid.display.StopPlayerDisplay();
        Unit target = curTarget.GetComponent<Unit>();
        Vector2 endPos = mapManager.pathfinding.foundPath.Last().gridPos;
        if (unit.gridPos != endPos)
            mapManager.cursor.selector.MoveSelection(endPos);
        while (unit.isMoving) {
            yield return null;
        }
        mapManager.cursor.selector.Deselect();
        yield return mapManager.StartCoroutine(attackManager.DoAttacks(unit, target));
        if (unit != null) {
            //makes sure unit is alive
            unit.Act();
        } else mapManager.turnManager.CompleteAction();
    }

    internal void ManageMovement() {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
            curTarget = validAttacks.PreviousItem();
            mapManager.cursor.SetPos(curTarget.transform.position); 
            Debug.Log("Attack " + curTarget + "?");
            DisplayAttackInfo(); 
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
            curTarget = validAttacks.PreviousItem();
            mapManager.cursor.SetPos(curTarget.transform.position); 
            Debug.Log("Attack " + curTarget + "?");
            DisplayAttackInfo();
        }  
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
            curTarget = validAttacks.NextItem();
            mapManager.cursor.SetPos(curTarget.transform.position); 
            Debug.Log("Attack " + curTarget + "?");
            DisplayAttackInfo();
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
            curTarget = validAttacks.NextItem();
            mapManager.cursor.SetPos(curTarget.transform.position); 
            Debug.Log("Attack " + curTarget + "?");
            DisplayAttackInfo();
        }
    }

    private void DisplayAttackInfo() {
        attackManager.DisplayAttackInfo(unit, curTarget, unitEndPos);
    }
}
