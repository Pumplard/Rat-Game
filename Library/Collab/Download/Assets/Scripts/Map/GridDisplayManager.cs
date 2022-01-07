using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//displays tiles and path
//tiles SpriteRenderer objects are instantiated at on init
//path GameObjects are instantiated when making path
//all mentions of "unit" refer to gridMovable
//seperate functions for selecting and displaying and just displaying all
public class GridDisplayManager : ScriptableObject {
    internal MapGrid grid;

    private SpriteRenderer[,] displayTiles; //Instances of tileprefab
    private HashSet<Vector2> displayedTiles = new HashSet<Vector2>(); //used for clearing

    private SpriteRenderer[,] dangerTiles; //Instances of tile prefab
    public bool allDanger = false; //whether displaying all dangertiles, set in 2 DisplayAllDanger functions, also used in mapmanager 
    private Dictionary<Vector2,int> displayedDanger = new Dictionary<Vector2,int>(); //pos, number of times display is being called by dangerUnits.
    private Dictionary<GridMovable,List<Tile>> displayedDangerUnits = new Dictionary<GridMovable,List<Tile>>(); //gridMovables(not just units) with displayedDanger called on them, also keeps a copy of validmoves
    private HashSet<GridMovable> selectedDangerUnits = new HashSet<GridMovable>();//units selected to view danger area

    internal HashSet<GameObject> displayedPath = new HashSet<GameObject>();
    public GameObject path;
  
    public Sprite valid, validAtk, selDanger,danger;
    public SpriteRenderer tilePrefab;
    private Transform tileParent, dangerParent;


    //assigns grid, instantiates prefabs for width/height
    public void Init(MapGrid inputGrid, int width, int height) {
            grid = inputGrid;
            dangerParent = new GameObject("dangerParent").GetComponent<Transform>();
            tileParent = new GameObject("tileParent").GetComponent<Transform>();
            tilePrefab = grid.manager.displayTilePrefab;
            displayTiles = new SpriteRenderer[width,height];
            dangerTiles = new SpriteRenderer[width,height];
            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {
                    displayTiles[x, y] = Instantiate(tilePrefab, new Vector2(x,y), Quaternion.identity, tileParent);
                    dangerTiles[x, y] = Instantiate(tilePrefab, new Vector2(x,y), Quaternion.identity, dangerParent);
                    dangerTiles[x, y].sortingLayerName = "GridDangerUI";
                }
            }
            validAtk = grid.manager.validAtk;
            valid = grid.manager.valid;
            selDanger = grid.manager.selDanger;
            danger = grid.manager.danger;
            path = grid.manager.path;
    }

    //displays a single tile
    private void DisplayTile(Vector2 pos, Sprite sprite, bool dangerTile) {
        SpriteRenderer tile;
        if (!dangerTile)
            tile = displayTiles[(int)pos.x,(int)pos.y];
        else
            tile = dangerTiles[(int)pos.x,(int)pos.y];
        tile.sprite = sprite;
        tile.gameObject.SetActive(true);
    }

    //displays found valid moves
    public void DisplayValidMoves(Unit unit) {
        foreach(Tile tile in unit.validMoves) {
            Vector2 curGridPos = tile.gridPos;
            displayedTiles.Add(curGridPos);
            if (tile.validMove)
                DisplayTile(curGridPos, valid, false);
            else if (tile.validAtk)
                DisplayTile(curGridPos, validAtk, false);          
        }            
    }

    //displays danger area of all units of one list,
    //called by every non-player list
    //doesn't use DisplayDanger, but adds them to displayedDangerUnits
    public void DisplayAllDanger(UnitList unitList) {
        allDanger = true;
        foreach (GameObject obj in unitList.mapUnits) {
            GridMovable unit = obj.GetComponent<GridMovable>();
            if (unit != null && !selectedDangerUnits.Contains(unit)) {
                DisplayDanger(unit);
            }
        }
        //also foreach map object like ballista
    }

    //called in gameManager, manages selecting/deselecting specific units
    public void ToggleUnitDanger(GridMovable unit) {
        if (selectedDangerUnits.Contains(unit)) 
            DeselectUnitDanger(unit);                     
        else
            SelectUnitDanger(unit);   
    }

    ///selectedUnits, called in game Manager when "selecting" danger unit
    private void SelectUnitDanger(GridMovable unit) {
        selectedDangerUnits.Add(unit);
        SetUnitColor(unit, Color.red);
        DisplayDanger(unit);
    }

    //sets unit color when selecting/deselecting
    private void SetUnitColor(GridMovable unit, Color color) {
        unit.GetComponent<SpriteRenderer>().color = color;
    }

    //adds new unit to displayedDangerUnits then displays tiles
    private void DisplayDanger(GridMovable unit) {
        //if already displayalled
        if (!displayedDangerUnits.ContainsKey(unit)) {
            List<Tile> validMovesCpy = new List<Tile>(); //edited in next function
            displayedDangerUnits.Add(unit, validMovesCpy);
            DisplayDangerTiles(unit);
        } 
        else if (selectedDangerUnits.Contains(unit)) {
            ReplaceDangerTiles(unit,selDanger);
        }
        
    }

    //displays valid attacks for single unit, if unit is already selected adds to displayedtiles value.
    //used above and in update
    private void DisplayDangerTiles(GridMovable unit) {
        unit.SetValidMoves();
        List<Tile> validMovesCpy = displayedDangerUnits[unit];
        bool selected = selectedDangerUnits.Contains(unit);
        foreach(Tile tile in unit.validMoves) {
            validMovesCpy.Add(tile);
            Vector2 tilePos = tile.gridPos;
            //if first time displaying tile display it, else just add one
            if (!displayedDanger.ContainsKey(tilePos)) {
                //display correct sprite if alldanger of not
                displayedDanger.Add(tilePos,0);
                if (selected) {
                    DisplayTile(tilePos, selDanger, true);
                } else {
                    DisplayTile(tilePos, danger, true);    
                }     
            }
            displayedDanger[tilePos] += 1; 
        }
        unit.ClearValidMoves();
    }

    //used for replacing existing display tiles, used when selecting/deslecting when alldanger
    //cannot replace a selDanger
    private void ReplaceDangerTiles(GridMovable unit, Sprite sprite) {
        foreach (Tile tile in displayedDangerUnits[unit]) {
            Vector2 pos = tile.gridPos;
            SpriteRenderer displayTile = dangerTiles[(int)pos.x,(int)pos.y];
            displayTile.sprite = sprite;
        }
    }

    //called on completeAction in turnManager
    //updates danger display for all displayedDangerUnits (doing otherwise could cause bugs)
    public void UpdateDisplayedDanger() {
        Debug.Log("Update");
        foreach(KeyValuePair<GridMovable,List<Tile>> pair in displayedDangerUnits) {
            GridMovable curUnit = pair.Key;
            UpdateDisplayedDanger(curUnit);
        }
        foreach (GridMovable unit in selectedDangerUnits) {
            ReplaceDangerTiles(unit, selDanger);
        }
    }

    //used in UpdateDisplayedDanger, updates a single unit's danger area
    private void UpdateDisplayedDanger(GridMovable unit) {
            if (allDanger && selectedDangerUnits.Contains(unit)) {
                ReplaceDangerTiles(unit, danger);
            }
            ClearUnitDangerDisplay(unit);
            DisplayDangerTiles(unit);
    }

    //displays path from unit to cursor
    public void DisplayPath(List<Tile> tiles) {
        foreach(var tile in displayedPath) {
            Destroy(tile);
        }
        displayedPath.Clear();
        foreach(var tile in tiles) {
            displayedPath.Add(Instantiate(path, tile.gridPos, Quaternion.identity));
        }
    }

    //stops all player display: ValidMoves/attack, path
    public void StopPlayerDisplay() {
        foreach(Vector2 tile in displayedTiles) {
            displayTiles[(int)tile.x,(int)tile.y].gameObject.SetActive(false);
        }
        displayedTiles.Clear();

        foreach(GameObject tile in displayedPath) {
            Destroy(tile);
        }
        displayedPath.Clear();
    }


    //stops allDanger danger tiles
    public void StopDisplayAllDanger() {
        List<GridMovable> toRemove = new List<GridMovable>();
        foreach(KeyValuePair<GridMovable, List<Tile>> pair in displayedDangerUnits) {
            GridMovable curUnit = pair.Key;
            if (!selectedDangerUnits.Contains(curUnit)) {
                toRemove.Add(curUnit);
                ClearUnitDangerDisplay(curUnit);
            }
        }
        foreach (GridMovable unit in toRemove) {
            displayedDangerUnits.Remove(unit);
        }
        allDanger = false;
    }

    //removes from selected units and stops display, called when pressing z on an a displayed danger unit in mapManager and in manager.RemoveUnit
    //if allDanger, replaces tiles rather than removing, but also has to update all selectedUnits. yab
    public void DeselectUnitDanger(GridMovable unit) {
        selectedDangerUnits.Remove(unit);
        SetUnitColor(unit, Color.white);
        if (!allDanger)
            StopDangerDisplay(unit);
        else {
            ReplaceDangerTiles(unit, danger);
            foreach (GridMovable otherUnit in selectedDangerUnits) {
                if (unit != otherUnit) 
                ReplaceDangerTiles(otherUnit, selDanger);
            }
        }

    }

    //called on unit death
    public void OnUnitDeath(GridMovable unit) {
        if (displayedDangerUnits.ContainsKey(unit)) {
            if (selectedDangerUnits.Contains(unit)) {
                DeselectUnitDanger(unit);
            }
            StopDangerDisplay(unit);
        }
    }


    //stops danger display for one unit, called when deselecting or unit dies
    private void StopDangerDisplay(GridMovable unit) {
        ClearUnitDangerDisplay(unit);
        displayedDangerUnits.Remove(unit);
    }

    //clears display tiles for unit, but doesn't remove unit from list
    //used in stopdangerdisplay and Updatedangerdisplay

    private void ClearUnitDangerDisplay(GridMovable unit) {
        List<Vector2> toRemove = new List<Vector2>();
        List<Tile> displayedTiles = displayedDangerUnits[unit];
        foreach(Tile tile in displayedTiles) {
            Vector2 tilePos = tile.gridPos;
            displayedDanger[tilePos] -= 1;
            if (displayedDanger[tilePos] <= 0) {
                toRemove.Add(tilePos);
            }
        }
        foreach(Vector2 tilePos in toRemove) {
            displayedDanger.Remove(tilePos);
            dangerTiles[(int)tilePos.x,(int)tilePos.y].gameObject.SetActive(false);
        }
        displayedTiles.Clear();
    }


}
