using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//manages the action select menu in map
//called in mapmanager
public enum Action { Attack, Wait, End };
public class ActionSelectManager : MonoBehaviour {
    public ActionSelectDisplay display;
    private GridMovable unit;
    public MapManager manager;
    private Action curAction;
    private CircularList<Action> actions; //might be an issue if no memory


    //Called in any situation where no unit is selected
    public void Init() {
        Init(null);
    }

    //runs Init when unit is done moving
    //calls Init(unit)
    public IEnumerator DelayedInit(GridMovable selection) {
        while(selection.isMoving) {
            yield return null;
        }
        Init(selection);

    }

    //if selection isn't null, does unit actions, else menu actions
    private void Init(GridMovable selection) {
        actions = new CircularList<Action>();
        unit = null;
        
        if (selection != null) {
            unit = selection;
            unit.SetValidAttacks(manager.cursor.path.Last().gridPos);
            if (unit.validAttacks.Any()) {
                actions.Add(Action.Attack);
            } 
            actions.Add(Action.Wait);
        } 
        else {
            actions.Add(Action.End);
        }
        
         curAction = actions[0];
        //display
        display.DisplayActions(actions);
        display.HighlightAction(actions.CurrentIndex);
    }


    
    public void ChooseAction() {
        display.Clear();
        switch (curAction) {
            case Action.Wait: {
                manager.cursor.selector.Deselect();
                unit.Act();
                break;
            }
            case Action.Attack: {
                if (unit.GetComponent<Unit>() == null) {
                    Debug.Log("Attack action unsuccessful");
                    break;
                }
                manager.targetSelect = new TargetSelectManager(unit.GetComponent<Unit>());//enemy phase if empty
                break;
            }
            case Action.End: {
                manager.turnManager.ForceTurnEnd();
                manager.turnManager.CompleteAction();
                break;
            }
            default: {
                Debug.Log("Invalid Action");
                break;
            }
        }
    }

    internal void ManageMovement() {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
            curAction = actions.NextItem();
            Debug.Log(curAction + " " + actions[actions.CurrentIndex]);
            display.HighlightAction(actions.CurrentIndex);
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
            curAction = actions.PreviousItem();
            Debug.Log(curAction  + " " + actions[actions.CurrentIndex]);
            display.HighlightAction(actions.CurrentIndex);
        }
    }

    public void ClearDisplay() {
        display.Clear();
    }
}
