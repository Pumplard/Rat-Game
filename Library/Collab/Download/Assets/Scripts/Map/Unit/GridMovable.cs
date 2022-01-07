using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMovable : MonoBehaviour
{
    public SelectionResponse response; //not used for enemy units

    public MapGrid grid;
    public Vector2 gridPos;
    internal Vector2 initPos; //position on grid before moving, set in onSelect();

    public Animator animator;
   
    internal HashSet<Tile> validMoves = new HashSet<Tile>();
    internal CircularList<GameObject> validAttacks = new CircularList<GameObject>();
    internal HashSet<GameObject> inRangeEnemies = new HashSet<GameObject>(); //objects in range of this, excludes this

    
    public UnitData data; 
    //movement
    internal bool isMoving;
    private Vector2 startPos, endPos;
    private float timetoMove = 0.4f;

    //map relevant
    public int team = -1;
    public int group = -1;
    public bool hasActed = false;




    

    //referenced in unitList and Act() below
    public void StartIdleAnimation() {
        animator.Play("unit_idle");
    }

    //called whenever unit has performed an action
    public void Act() {
        hasActed = true;
        if (animator != null)
            animator.Play("unit_acted");
        grid.manager.turnManager.CompleteAction();
    }

    //true if same team, else false
    public bool CompareTeam(GameObject otherObject) {
        GridMovable otherUnit = otherObject.GetComponent<GridMovable>();
        return (otherUnit != null && otherUnit.team == team);    
    }

    //removes object from grid data, moves, re-inserts object
    public IEnumerator Move(List<Tile> destinations) {
        isMoving = true;
        float timePerTile = timetoMove/destinations.Count;
        float elapsedTime = 0;

        foreach(var destination in destinations) {
            elapsedTime = 0;
            startPos = transform.position;
            endPos = destination.gridPos;
            while(elapsedTime < timePerTile) {
                transform.position = Vector2.Lerp(startPos, endPos, (elapsedTime/ timePerTile));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transform.position = endPos;
        }
        SetPos(endPos);
        isMoving = false;   
    }

    //Sets transform and grid pos to newPos
    public void SetPos(Vector2 newPos) {
        grid.Empty(gridPos);
        gridPos = newPos;
        grid.Insert(gridPos, gameObject);
        transform.position = gridPos;
    }

    //changes position on grid, should replace with newPos, don't use for initialization
    public void SetGridPos(Vector2 newPos) {
        grid.Empty(gridPos);
        gridPos = newPos;
        grid.Insert(gridPos, gameObject);
    }

    public void SetValidMoves() {
        validMoves.Clear();
        SetValidTiles(data.mv.GetValue(), data.maxRange.GetValue(), gridPos);
        //foreach(var unit in inRange) {
        //    Debug.Log(unit);
        //}        
    }


    //initial space not set to validAttack
    //validAtk: tile is occupied && in mv or atk range, tile has open tiles for attack
    //actions can be used on them, use toTarget pathfinding
    //validMove: tile is traversable, tile is in mv range, even if cannot actually move there (in this case, it isn't validAtk)
    //can be moved to, use regular pathfinding
    //recursive helper for setValidMoves, modifies tile bools, adds to inRange.
    public void SetValidTiles(int mvt, int rng, Vector2 pos) {
        if (grid.IsValidPos(pos)) {
            var tile = grid.GetTile(pos);
            if (mvt >= 0) { 
                mvt -= tile.mvCost;
                if (!tile.validMove) { 
                    //Sets to valid if in mv && (empty || same team)
                    if (tile.TileIsTraversable(gameObject)) {
                        validMoves.Add(tile);
                        tile.validMove = true;
                        //if it isn't empty, sets to validAttack after validMoves are set
                    }
                    else if (!tile.IsEmpty() && pos != gridPos || !tile.walkable /*&& !CompareTeam(tile.tileObject)*/) {
                        mvt = -1;  
                        rng--;
                        //InRangeAdd(tile); caused enemies to stack
                        //if not traversable, stops recursion and subracts from range
                    }                                                                                                
                }
            }
            else if (rng > 0) {
                rng--;
            }
            if (mvt >= 0 || rng > 0) {
                SetNextTiles(mvt, rng, pos);
            }
            //checks if there is a spot available to attack the enemy unit
            if (!tile.IsEmpty() && !tile.openTilesChecked && pos != gridPos) {
                //sets validAttack for allies(so they use correct pathfinding, can change this to validheal)
                tile.openTilesChecked = true;
                if (CheckForOpenTiles(pos)) {
                    tile.validAtk = true;
                    validMoves.Add(tile);
                    InRangeAdd(tile);
                    //Debug.Log("いいね！" + tile.gridPos);
                }                    
                //else Debug.Log("悪いね" + tile.gridPos);   
            }
            if (mvt < 0 && rng >= 0) {
                if (!tile.validAtk && CheckForOpenTiles(pos)) {
                    validMoves.Add(tile); //yes
                    //tile.validMove = false; //not needed
                    tile.validAtk = true;
                    InRangeAdd(tile);
                }
            }  
        }
    }   

    public void SetNextTiles(int mvt, int rng, Vector2 pos) {
        SetValidTiles(mvt, rng, pos + Vector2.up);
        SetValidTiles(mvt, rng, pos + Vector2.right);
        SetValidTiles(mvt, rng, pos + Vector2.down);
        SetValidTiles(mvt, rng, pos + Vector2.left);
    }

    //called many times, but always cleared when unit dies and after setvalidmoves
    public void ClearValidMoves() {
        foreach(var tile in validMoves) {
            tile.validMove = false;
            tile.validAtk = false;
            tile.openTilesChecked = false;
        }
        validMoves.Clear();
        inRangeEnemies.Clear();
    }

    //attempts to add tileObject to inRange
    public void InRangeAdd(Tile tile) {
        GameObject tileObject = tile.tileObject;
        if (tileObject != null && tile.gridPos != gridPos) {
            Unit tileUnit = tileObject.GetComponent<Unit>();
            if (tileUnit != null && tileUnit.team != team)
                inRangeEnemies.Add(tileObject);
        }
    }

    //sets validattack list, doens't change valid tile
    //can sort by distance from 0,0 later
    //used for target select
    public void SetValidAttacks(Vector2 newPos) {
        HashSet<GameObject> targets = new HashSet<GameObject>();
        GetValidAttacks(targets, 0, newPos, newPos);
        validAttacks = new CircularList<GameObject>();
        validAttacks.AddRange(targets);
    }

    //adds all enemy units to "targets" list, excludes allys (which are filtered out in TargetSelect)
    public void GetValidAttacks(HashSet<GameObject> targets, int rng, Vector2 pos, Vector2 startPos) {
        if (grid.IsValidPos(pos)) {
            GameObject tileObject = grid.GetObject(pos);            
            if (rng <= data.maxRange.GetValue()) {
                if (rng >= data.minRange.GetValue()) {
                    if (tileObject != null && pos != startPos) {
                        Unit tileUnit = tileObject.GetComponent<Unit>();
                        if (tileUnit != null && tileUnit.team != team)
                            targets.Add(tileObject);
                    }
                }
                rng++;
                if (rng <= data.maxRange.GetValue()) {
                    GetValidAttacks(targets, rng, pos + Vector2.up, startPos);
                    GetValidAttacks(targets, rng, pos + Vector2.right, startPos);
                    GetValidAttacks(targets, rng, pos + Vector2.down, startPos);
                    GetValidAttacks(targets, rng, pos + Vector2.left, startPos); 
                }
 
            }          
        }
    }

    //Only needed for nonempty tiles
    public bool CheckForOpenTiles(Vector2 initPos) {
        return (CheckIfOpenTile(initPos, 0));    
    }

    //could use a do
    public bool CheckIfOpenTile(Vector2 tilePos, int rng) {
        if (rng <= data.maxRange.GetValue() && grid.IsValidPos(tilePos)) {        
            if (rng >= data.minRange.GetValue()) {
                Tile curTile = grid.GetTile(tilePos);   
                if ((curTile.IsEmpty() || tilePos == gridPos) && curTile.validMove)
                    return true;
            }
            rng++; 
            return (CheckIfOpenTile(tilePos + Vector2.up, rng) ||
                    CheckIfOpenTile(tilePos + Vector2.right, rng) ||
                    CheckIfOpenTile(tilePos + Vector2.down, rng) ||
                    CheckIfOpenTile(tilePos + Vector2.left, rng));
        }    
        return false;
    }
}
