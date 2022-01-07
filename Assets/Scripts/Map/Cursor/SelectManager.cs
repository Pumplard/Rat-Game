using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manages selecting and performing actions on selection
public class SelectManager : MonoBehaviour
{

    public CursorManager cursor;
    private SelectionResponse selectResponse;
    internal GridMovable selection;
    public void Select(GameObject gridObject) {
        if (gridObject != null)
            selection = gridObject.GetComponent<Unit>();
        if (selection != null) {
            cursor.hasSelection = true; //not needed
            selectResponse = selection.response;
            selectResponse.OnSelect();
            //cursor.path.Add(cursor.grid.GetTile(cursor.gridPos));  //creates initial path w/ no movement.
            cursor.grid.manager.SetState(GameState.PlayerHasSelection);
        }        
    }

    public void Deselect() {
        selectResponse.OnDeselect();
        selection = null;
        cursor.hasSelection = false;
        //cursor.TilesMoved.Clear();
    }

    //moves selected object if space is valid, returns true if successful
    //Passes new Movelist cloned from cursor movelist to the selection's move function
    public bool MoveSelection(Vector2 endPos) {
        Tile endTile = cursor.grid.GetTile(endPos);
         if (selection != null && endTile != null && endTile.validMove && (endTile.IsEmpty() || cursor.gridPos == selection.gridPos)) { //should add this in mapmanager instead?
            if (cursor.gridPos != selection.gridPos) {
                StartCoroutine(selection.Move(cursor.path));
            }
            return true;
        }  
        return false;     
    }

    public void UndoMoveSelection() {
        if (selection != null) {
            selection.SetPos(selection.initPos);
            selection.MoveAnimation();
        }
    }
}
