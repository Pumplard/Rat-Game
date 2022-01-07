
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PathManager", menuName = "Pathing/PathManager")]
public class PathManager : ScriptableObject {

    public MapGrid grid;
    public List<Tile> foundPath;

    HashSet<Vector2> validEndPos;
    int bestDistance;
    private Vector2 bestValidEndPos;

    
    public void Init(MapGrid newGrid) {
        grid = newGrid;
    }

    public void ClearPath() {
        foundPath.Clear();
    }

    public virtual void FindPath(Vector2 startPos, Vector2 endPos) {

        SetBestEndPos(startPos, endPos);    

        Tile startTile = grid.GetTile(startPos);
        Tile endTile = grid.GetTile(bestValidEndPos);

        Heap<Tile> openSet = new Heap<Tile>(grid.MaxSize);
        HashSet<Tile> closedSet = new HashSet<Tile>();

        openSet.Add(startTile);

        while (openSet.Count > 0) {
            Tile currentTile = openSet.RemoveFirst();
            closedSet.Add(currentTile);

            if (currentTile == endTile) {
                RetracePath(startTile, endTile);
                    return;
            }            
            
            foreach (Tile neighbor in grid.GetNeighbors(currentTile)) {
                if (!neighbor.validMove || closedSet.Contains(neighbor)) {
                  continue;
                }

                int newMoveCost = currentTile.gCost + GetDistance(currentTile, neighbor);
                if (newMoveCost < neighbor.gCost || !openSet.Contains(neighbor)) {
                    neighbor.gCost = newMoveCost;
                    neighbor.hCost = GetDistance(neighbor, endTile);
                    neighbor.parent = currentTile;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                    else
                        openSet.UpdateItem(neighbor);
                }
            }
        }
    }

    public virtual void RetracePath(Tile startTile, Tile endTile) {
        List<Tile> path = new List<Tile>();
        Tile currentTile = endTile;

        while (currentTile != startTile) {
            path.Add(currentTile);
            currentTile = currentTile.parent;
        }
        path.Add(startTile);
        path.Reverse();  
        foundPath = path;
    }

    public int GetDistance(Tile tile1, Tile tile2) {
        return Math.Abs(tile1.x - tile2.x) + Math.Abs(tile1.y - tile2.y);
    }

    public int GetDistance(Vector2 pos1, Vector2 pos2) {
        return (int)(Math.Abs(pos1.x - pos2.x) + Math.Abs(pos1.y - pos2.y));
    }

    public int GetFCost(Tile tile) {
        return (tile.gCost + tile.hCost);
    }

    private void SetBestEndPos(Vector2 startPos, Vector2 endPos) {

        Tile endTile = grid.GetTile(endPos);
        Tile startTile = grid.GetTile(startPos);

        if (endTile.IsEmpty() || endPos == startPos) {
            bestValidEndPos = endPos;
            return;
        }

        if (!endTile.IsEmpty()) {
            GridMovable startObject = startTile.tileObject.GetComponent<GridMovable>();
            bestValidEndPos = endPos;
            validEndPos = new HashSet<Vector2>();
            bestDistance = GetDistance(startPos, endPos);
            FindBestEndPos(endPos, startPos, 0, startObject, new HashSet<Tile>());
        }

        if (foundPath.Any()) {
            foreach (Tile tile in foundPath) {
                if (validEndPos.Contains(tile.gridPos))
                    bestValidEndPos = tile.gridPos;
            }
        }
    }

    //if we get to startPos, that is the best distance!
    private void FindBestEndPos(Vector2 curPos, Vector2 startPos, int curRange, GridMovable startObject, HashSet<Tile> examinedTiles) {
        if (!grid.IsValidPos(curPos))
            return;

        Tile curTile = grid.GetTile(curPos);

        int maxRange = startObject.data.maxRange.GetValue();
        int minRange = startObject.data.minRange.GetValue();

        if (curRange <= maxRange) {
            if (curRange >= minRange && !examinedTiles.Contains(curTile)) {
                if (curTile.IsEmpty() && curTile.validMove || curPos == startPos) {
                    int curDistance = GetDistance(curPos, startPos);
                    if (curDistance < bestDistance || !validEndPos.Any()) {
                        bestValidEndPos = curPos;
                        bestDistance = curDistance;
                    }
                    validEndPos.Add(curPos);
                    examinedTiles.Add(curTile);
                    //if (bestDistance == 0)
                    //    return;
                }
            }
            curRange++; 
            FindBestEndPos(curPos + Vector2.up, startPos, curRange, startObject, examinedTiles);
            FindBestEndPos(curPos + Vector2.right, startPos, curRange, startObject, examinedTiles);
            FindBestEndPos(curPos + Vector2.down, startPos, curRange, startObject, examinedTiles);
            FindBestEndPos(curPos + Vector2.left, startPos, curRange, startObject, examinedTiles);
        } 
    }
}
