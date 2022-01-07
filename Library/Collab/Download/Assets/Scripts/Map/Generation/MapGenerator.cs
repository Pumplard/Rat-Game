using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//Generates map, also responsible for removing the map
public class MapGenerator : MonoBehaviour {

    public string levelName;
    public int width, height;

    public List<UnitList> initialUnitLists; //list of unitlists for each player
    public CircularList<UnitList> unitLists;
    public List<GridPosList> initialCoordLists;
    internal List<GridPosList> startCoords;
    public TileGenerator tileGen;
    
    //Sets units in unitLists[0]
    public void AddPlayerUnits(List<UnitList> playerUnitLists) {        
        if (playerUnitLists == null) {
            Debug.Log("Failed to set playerUnits");
            return;
        }
        
        int i = 0; //index in playerUnitList
        int playerListCount = 0;
        foreach (UnitList list in playerUnitLists) {
            if (list.unitTotal > 0) {
                playerListCount++;
            }
        }
        for (int j = 0; j < initialUnitLists.Count; j++) {
            if ((i < playerListCount) && (initialUnitLists[j] == null || initialUnitLists[j].team == playerUnitLists[i].team)) {
                initialUnitLists[j] = Instantiate(playerUnitLists[i]);
                i++;
            }
        }
    }

    //sets one initial unit list
    public void SetInitialUnitList(UnitList newUnitList, int index) {

    }

    //set potential player groups, make sure to remove if null

    //creates new lists based on turn order, removes empty or null lists
    private void SetLists() {
        unitLists = new CircularList<UnitList>();
        startCoords = new List<GridPosList>();
        for (int i = 0; i < initialUnitLists.Count; i++) {
            UnitList curUnitList = initialUnitLists[i];
            if (curUnitList != null && !(curUnitList.IsEmpty())) {
                unitLists.Add(Instantiate(curUnitList));
                startCoords.Add(initialCoordLists[i]);
            }
        }
    }
    //Generates and returns grid with units, used in mapmanager
    public MapGrid GenerateGrid() {

        MapGrid grid = new MapGrid(tileGen);
        SetLists();

        //For each list of units, instantiates each unit according to the matching index in the matching list of startCoords
        for(int i = 0; i < unitLists.Count; i++) {
            UnitList curUnitList = unitLists[i];
            curUnitList.Initialize();
            curUnitList.group = i;
            //lead unit stuff
            bool hasLeadUnit = curUnitList.hasLeadUnit; //just so loop doesn't have to access every time
            curUnitList.AddLeadUnit();
            int unitCt = unitLists[i].units.Count;
            int leadIndex = unitCt - 1; //lead unit index
            for(int j = 0; j < unitCt; j++) {
                //if/else makes sure lead unit get leadUnit coords
                Vector2 curCoords;
                if (j == leadIndex && hasLeadUnit) {
                    curCoords = startCoords[i].coords.Last();
                }

                else curCoords = startCoords[i].coords[j];
                GameObject unit = curUnitList.CreateUnitObject(j, curCoords);
                Unit unitScript = unit.GetComponent<Unit>();
                unitScript.Initialize(grid, curCoords);
                grid.Insert(unitScript.gridPos, unit);
                unitScript.SetTeam(unitLists[i].team);
                unitScript.SetGroup(i);
                curUnitList.mapUnits.Add(unit);       
            }
            if (hasLeadUnit) {
                curUnitList.leadUnit.teamBuff.Apply(curUnitList);
            }
        }
        return grid;
    }

    //Clears the map, deletes tiles, transfers unitLists to initialunitLists
    public void ClearMap() {
        tileGen.RemoveTiles();
    }
}
