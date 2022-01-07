using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class TurnManager {

    public MapManager manager;
    
    public int curUnitsActed;
    public int curMaxUnits;

    private bool forceEnd; //for "end" selection, called only by ForceTurnEnd and CheckTurnEnd

    //must be created after Mapmanger unitLists are set
    public TurnManager(MapManager newManager) {
        manager = newManager;
    }


    public void CompleteAction() {
        UnitList curUnitList = manager.curUnitList;
        manager.grid.display.UpdateDisplayedDanger();

        SetMidTurnVals(manager.unitLists);

        if (CheckGameEnd(manager.unitLists)) {
            manager.GameEnd();
            return;
        }

        //unitsacted+=1
        if (CheckTurnEnd()) {
            Debug.Log("Cycling Teams");
            CircularList<UnitList> unitLists = manager.unitLists;
            foreach (UnitList unitlist in unitLists) {
                if (unitlist.team == curUnitList.team) {
                    unitlist.OnTurnEnd();
                }
            }
            manager.NextGroup();
            curUnitList = manager.curUnitList;
            foreach (UnitList unitlist in unitLists) {
                if (unitlist.team == curUnitList.team) {
                    unitlist.OnTurnStart();
                }
            }
            SetUnitVals(manager.unitLists);
            if (curUnitList.team != manager.playerTeam) {
                //wait until player is done moving?
                manager.SetState(GameState.EnemyPhase);
                manager.StartCoroutine(AITurn(curUnitList));
            }
        }
       // foreach (UnitList unitList in manager.unitLists) {
        //    if (unitList.unitTotal <= 0) {
        //        manager.SetState(GameState.GameOver);
       //         return;
        //    }
        //} 

        if (curUnitList.team == manager.playerTeam)
            manager.SetState(GameState.PlayerNoSelection);//enemy phase if empty
    }

    //sets curUnitsActed and curMaxUnits based on current list
    //called in init and every turn
    public void SetUnitVals(CircularList<UnitList> unitLists) {
        SetMidTurnVals(unitLists);
    }

    //sets max units and units acted based on unitList, since units can die
    //used in SetUnitVals and every action
    private void SetMidTurnVals(CircularList<UnitList> unitLists) {
        int curTeam = manager.curUnitList.team;
        curMaxUnits = 0;
        curUnitsActed = 0;
        foreach (UnitList unitList in unitLists) {
            if (unitList.team == curTeam && unitList.isActive) {
                foreach (GameObject obj in unitList.mapUnits) {
                    Unit unit = obj.GetComponent<Unit>(); {
                        if (unit != null) {
                            if (unit.hasActed) curUnitsActed++;
                        }
                    }
                }
                curMaxUnits += unitList.unitTotal;
            }
        }
    }
    private bool CheckTurnEnd() {
        Debug.Log(curUnitsActed + "/" + curMaxUnits);
        if (forceEnd) {
            forceEnd = false;
            return true;
        }
        if (curUnitsActed >= curMaxUnits)
            return true;
        else
            return false;
    }
    private bool CheckGameEnd(CircularList<UnitList> unitLists) {
        SetDeadGroups(unitLists);
        HashSet<int> aliveTeams = new HashSet<int>();
        foreach (UnitList unitList in unitLists) {
            if (unitList.isActive)
                aliveTeams.Add(unitList.team);
        }
        
        if ((!aliveTeams.Contains(manager.playerTeam)) || !aliveTeams.Any(x => x != manager.playerTeam))
            return true;
        else
            return false; 
    }

        //used in "end" selection, used to force turn end on next turnmanager.CompleteAction()
    public void ForceTurnEnd() {
        forceEnd = true;
    }
    public void SetDeadGroups(CircularList<UnitList> teams) {
        List<int> deadTeams = GetDeadGroups(teams);
        foreach(int group in deadTeams) {
            teams[group].isActive = false;
        }
    }

    public List<int> GetDeadGroups(CircularList<UnitList> teams) {
        List<int> output = new List<int>();
        foreach (UnitList unitList in teams) {
            if (unitList.unitTotal <= 0) {
                output.Add(unitList.group);
            }
            //if (unitList.unitTotal <= 0) {
            //    manager.SetState(GameState.GameOver);
            //    return;
            //}
        }
        return output; 
    }

    //called on start of the aiturn
    public IEnumerator AITurn(UnitList inputList) {
        int index = 0;
        int unitCount = inputList.unitTotal; //debug this, crashes when AI kills self (fixed)
        //uses copy of list so removing units from actual list doesn't affect indexing
        List<GameObject> mapUnits = new List<GameObject>();
        foreach (GameObject unit in inputList.mapUnits) {
            mapUnits.Add(unit);
        }
        //start
        while (index < unitCount) {
            GameObject curObject = mapUnits[index];
            if (curObject != null) {
                Unit curUnit = curObject.GetComponent<Unit>();
                if (curUnit != null && curUnit.ai != null) {
                    manager.cursor.SetCameraPos(curUnit.gridPos);
                    yield return curUnit.StartCoroutine(curUnit.ai.MakeBestMove(curUnit));
                    if (curUnit != null)
                        curUnit.Act();
                    else CompleteAction(); //unit can die mid attack
                } //else CompleteAction(); //unit could be dead but still in list
                index++;
            }
            //else CompleteAction();
        }
        manager.cursor.SetCameraPos(manager.cursor.gridPos);
        //failsafe
    }
}
