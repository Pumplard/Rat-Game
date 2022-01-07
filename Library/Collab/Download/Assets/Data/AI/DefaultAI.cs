using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultAI", menuName = "AIType/Default")]

public class DefaultAI : AIType {

    public AttackManager attackManager;
    public PathManager pathfinding;
    public FullGridPathManager outOfRangePathfinding;

    public override IEnumerator MakeBestMove(Unit unit) {
        unit.SetValidMoves();
        //foreach (GameObject inRange in unit.inRangeEnemies) {
       //     Debug.Log(inRange.transform.position);
       // }
        Unit bestTarget = null;
        if (unit.inRangeEnemies.Any()) {
            bestTarget = BestTarget(unit, unit.inRangeEnemies);
            if (bestTarget != null) {
                MoveUnit(unit, bestTarget.gridPos);
                while (unit.isMoving) {
                    yield return null; 
                }
                yield return unit.StartCoroutine(attackManager.DoAttacks(unit, bestTarget));
            }
        }
        else {
            bestTarget = BestTarget(unit, GetTargetList(unit));
            if (bestTarget != null) {
                Vector2 bestPos = BestEndPos(unit, bestTarget);
                MoveUnit(unit, bestPos);
                while (unit.isMoving) {
                    yield return null; 
                }
            }
        }
        //inrange debug
        unit.ClearValidMoves();
    }

    //finds path, moves unit
    public void MoveUnit(Unit unit, Vector2 endPos) {
        pathfinding.ClearPath();
        pathfinding.FindPath(unit.gridPos, endPos);
        unit.StartCoroutine(unit.Move(pathfinding.foundPath));
    }

    //gets list of enemy targets
    public List<GameObject> GetTargetList(Unit unit) {
        CircularList<UnitList> unitLists = attackManager.manager.unitLists;
        List<GameObject> targetList = new List<GameObject>();
        foreach (UnitList unitList in unitLists) {
            if (unitList.team != unit.team)
                targetList.AddRange(unitList.mapUnits);
        }
        return targetList;
    }

    public Unit BestTarget(Unit unit, IEnumerable<GameObject> targetList) {
        Unit bestTarget = null;
        int bestResult = -1; //won't work if attack results in enemy healing
        foreach (GameObject gridObject in targetList) {
            Unit target = gridObject.GetComponent<Unit>();
            if (target != null && target.isAlive) { //eeeeeeeeeeeeeeeee
                int atkResult = AttackResult(unit, target);
                if (atkResult > bestResult) {
                    bestResult = atkResult;
                    bestTarget = target;
                }
            }
        }
        return bestTarget;
    }

    //chooses best on attack based on damage done, where killing blows have highest priorty.
    public int AttackResult(Unit attacker, Unit defender) {
        int dmgDone = attackManager.CalcDamage(attacker, defender, false).Item2;
        if (attacker.CheckFollowUp(defender)) {
            dmgDone += dmgDone;
        }
        int defenderResultHP = defender.data.hp - dmgDone;
        if (defenderResultHP <= 0)
            return dmgDone + defender.data.maxHp;
        else
            return dmgDone;
    }

    public Vector2 BestEndPos(Unit unit, Unit target) {
        outOfRangePathfinding.Init(unit.grid);
        outOfRangePathfinding.FindPath(unit.gridPos, target.gridPos);
        foreach (Tile tile in outOfRangePathfinding.foundPath) {
            if (tile.validMove && tile.IsEmpty())
                return tile.gridPos;
        }
        return unit.gridPos;
    }
}
