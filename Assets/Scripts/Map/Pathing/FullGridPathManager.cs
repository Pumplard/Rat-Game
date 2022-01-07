using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FullGridPathManager", menuName = "Pathing/FullGridPathManager")]
public class FullGridPathManager : PathManager {

    public override void FindPath(Vector2 startPos, Vector2 endPos) {

        Tile startTile = grid.GetTile(startPos);
        Tile endTile = grid.GetTile(endPos);

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
                if (!neighbor.TileIsTraversable() || closedSet.Contains(neighbor)) {
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

    public override void RetracePath(Tile startTile, Tile endTile) {
        List<Tile> path = new List<Tile>();
        Tile currentTile = endTile;

        while (currentTile != startTile) {
            path.Add(currentTile);
            currentTile = currentTile.parent;
        }
        path.Add(startTile);

        foundPath = path;
    }
}
