using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectUnit : SelectionResponse
{
    public Unit selection;

    //default constructor
    public SelectUnit(Unit unit) {
        selection = unit;
    }

    //displays valid moves, sets initialpos
    public void OnSelect() {
        selection.MoveAnimation();
        selection.SetValidMoves();
        selection.grid.display.DisplayValidMoves(selection);
        selection.initPos = selection.gridPos;
        selection.grid.manager.cursor.UpdatePath();
    }
    
    //called when deselecting with cursor or when performing an action
    //stops displaying valid moves
    //resets unit position, emptys pathfinding list
    public void OnDeselect() {
        selection.ClearValidMoves();
        selection.grid.display.StopPlayerDisplay();
        selection.StartIdleAnimation();
    
        //references path of tiles made for moving
        var path = selection.grid.manager.cursor.path;
        path.Clear();
    }

    
}
