using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile", menuName = "Tiles/BaseTile")]
[System.Serializable]
public class Tile : ScriptableObject, IHeapItem<Tile> {
     internal GameObject tileObject;
     internal Unit tileUnit {get {return tileObject.GetComponent<Unit>();}} //only call if tileObject != null
     public int mvCost = 1;
     public bool walkable = true;
     public string tileName;
     internal int x, y;
     public Vector2 gridPos;

     internal bool openTilesChecked = false;
     internal bool validMove = false; //currently modified by SetValidmoves in gridMovable, used in calculation, actual validMoves stored in unit
     internal bool validAtk = false;
     
     //pathfinding variables
     internal int gCost = 0;
     internal int hCost = 0;
     internal Tile parent;
     int heapIndex;
     
     //someday will remove x & y
     public void SetPos(int xPos, int yPos) {
          x = xPos;
          y = yPos;
          gridPos = new Vector2(xPos,yPos);
     }

     public bool TileIsTraversable() {
          if (!walkable)
               return false;
          if (IsEmpty())
               return true;
          else
               return TileIsTraversable(tileObject);
     }

     //TileObject is not same team as comparison object
     public bool TileIsTraversable(GameObject otherObject) {
          if (!walkable)
               return false;
          if (otherObject != null && tileObject != null) {
               Unit otherUnit = otherObject.GetComponent<Unit>();
               if (otherUnit != null && tileUnit != null && otherUnit.team != tileUnit.team) {
                    return false;
               }
          }
          return true;
     }

     public bool IsEmpty() {
          if (tileObject == null)
               return true;
          else
               return false;
     }

     //IHeapItem
     public int HeapIndex {
          get {
               return heapIndex;
          }
          set {
               heapIndex = value;
          }
     }

     public int CompareTo(Tile tile) {
          int compare = (gCost + hCost).CompareTo(tile.gCost + tile.hCost);
          if (compare == 0) {
               compare = hCost.CompareTo(tile.hCost);
          }
          return -compare;
     }
}
