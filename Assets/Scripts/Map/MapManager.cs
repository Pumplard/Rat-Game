using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // only for text


public enum GameState { PlayerPrepMenu, PlayerPrepMap, PlayerNoSelection, PlayerHasSelection, PlayerViewingInfo, PlayerChoosingUnitAction, PlayerChoosingMenuAction, PlayerChoosingTarget, NoControl, EnemyPhase, GameOver };
//Initializes grid (creates, sets this as manager), adds units
//Initializes assigns cursor and units to grid
//Contains pathfinding script reference

//class used for managing map generation and map gameplay
//Also calls other classes on map end
public class MapManager : MonoBehaviour {
    public List<UnitList> initialPlayerUnits; //units from player barracks that must be loaded in, have to add these manually for now.
    public List<UnitList> dataPlayerUnits; //Lists that units must be transferred to on map end
    public MapGenSelector mapGenSelect;
    private MapGenerator mapGen;
    public CursorManager cursor;
    public ActionSelectManager actionSelect;
    public TargetSelectManager targetSelect;
    public MapGrid grid;
    
    public GameObject gameEndUI; //ui

    public SpriteRenderer displayTilePrefab;
    public GameObject path;
    public Sprite valid, validAtk, selDanger, danger;
    public AudioManager audioManager;
    public MapMenuManager mapMenu;
    public PathManager pathfinding;
    public AttackManager attackManager;
    public AttackDisplayManager attackDisplayManager;
    public TurnManager turnManager;
    public AnimationManager animationManager;
    public MapUnitInfoDisplay hoverInfoDisplay, unitInfoDisplay;


    public GameState curState;
    public CircularList<UnitList> unitLists;

    public int playerTeam; //index of player's team, right now always 0
    public UnitList curUnitList;

    public int turnCount; //used for events, initialized at 0
    public List<MapEvent> mapEvents; //list of possible events, checked each turn
    


    
    public void SetState(GameState newState) {
        curState = newState;
    }

    public void NextGroup() {
        //Groups are still organized as if two groups in the same team can perform after eachoter
        //Has to ensure next group is of a different team
        int curTeam = curUnitList.team;
        curUnitList = unitLists.NextItem();
        while ( curUnitList.team == curTeam || curUnitList.isActive == false) {
           curUnitList = unitLists.NextItem(); 
        }
    }

    

    void Start() {
        mapGen = mapGenSelect.mapGen;      
        mapGen.AddPlayerUnits(initialPlayerUnits);
        //what if no playerUnits?
        grid = mapGen.GenerateGrid();
        unitLists = mapGen.unitLists;
        //must add manager to grid, must initialize gridDisplay
        pathfinding.Init(grid);
        attackManager.Init(this, attackDisplayManager);
        grid.Init(this);

        cursor.Init(grid);
        actionSelect.display.Init();//makes sure action select is inactive

        if (unitLists.Any())
            curUnitList = unitLists[0];
        playerTeam = 0;
        turnManager = new TurnManager(this); //should probably start with this already initialized
        turnCount = 0;

        StartPrep();

        //List<Tile> test = grid.GetNeighbors(grid.GetTile(Vector2.zero));
        //foreach(var tile in test) {
        //     Debug.Log(tile.x + ", " + tile.y);
        //}
    }

    void StartPrep() {
        cursor.SetPos(unitLists[0].mapUnits[0].GetComponent<Unit>().gridPos);
        if (mapMenu != null) {
            mapMenu.Display();
            audioManager.Play("menu");
            PrepMenu();
        }
        else
            StartLevel();
    }

    public void PrepMenu() {
        SetState(GameState.PlayerPrepMenu);
        hoverInfoDisplay.Clear();
        unitInfoDisplay.Clear();
    }

    public void ViewMap() {
        SetState(GameState.PlayerPrepMap);
    }

    public void StartLevel() {
        audioManager.Stop();
        if (mapMenu != null) {
            mapMenu.StopDisplay();
        }
        audioManager.Play("bgm");
        cursor.SetPos(curUnitList.mapUnits[0].GetComponent<Unit>().gridPos);
        turnManager.SetUnitVals(unitLists);
        foreach (UnitList unitlist in unitLists) {
            if (unitlist.team == curUnitList.team) {
                unitlist.OnTurnStart();
            }
            else unitlist.OnTurnEnd();
            
        }
        SetState(GameState.PlayerNoSelection);
    }


    //handles all killing, removing unit from list, removing display
    //called in attackmanager
    //also in some weaponscripts
    public void RemoveUnit(UnitList list, Unit toRemove) {
        toRemove.ClearValidMoves();
        grid.display.OnUnitDeath(toRemove);
        grid.Empty(toRemove.gridPos);
        list.RemoveUnit(toRemove.gameObject);
        Debug.Log("unit remover");
        Object.Destroy(toRemove.gameObject);
    }
    public void GameEnd() {
        SetState(GameState.GameOver);
        audioManager.Stop();
        audioManager.Play("end");
        //add bad end
        gameEndUI.GetComponent<Text>().text = "勝利";                
        gameEndUI.SetActive(true);
    }

    //clears map and returns, called by the return button
    public void ClearMap() {
        foreach (UnitList unitList in unitLists) {
            if (unitList.team == playerTeam) {
                int g = unitList.playerGroup; //curGroup
                dataPlayerUnits[g].ReturnMapUnits(unitList);
            } else unitList.RemoveMapUnits();
        }
        //Unity automatically clears cloned components
        //mapGen.ClearMap();
        SceneManager.LoadScene("Hub");
    }


    void Update() {   
        switch (curState) {
            case GameState.PlayerPrepMenu:
                break;

            case GameState.PlayerPrepMap:
                cursor.ManageMovement();
                GameObject obj = cursor.grid.GetObject(cursor.gridPos);
                hoverInfoDisplay.manageHoverInfo(obj);
                break;

            case GameState.PlayerNoSelection:
                GameObject curTileObj = cursor.grid.GetObject(cursor.gridPos);
                //selector
                if (Input.GetKeyDown(KeyCode.Z) && !cursor.isMoving) {
                    if (curTileObj != null) {
                        //State changed in selector
                        Unit unitSelected = curTileObj.GetComponent<Unit>();
                        if (curUnitList.team == unitSelected.team && !unitSelected.hasActed) {
                            cursor.selector.Select(curTileObj);
                        }
                        else if (curUnitList.team != unitSelected.team) {
                            grid.display.ToggleUnitDanger(unitSelected);    
                        }
                    }                        
                    else {
                        actionSelect.Init(); //if no unit, brings up menu
                        SetState(GameState.PlayerChoosingMenuAction);
                    }

                }

                //view unit info
                if (Input.GetKeyDown(KeyCode.C) && !cursor.isMoving) {
                    if (curTileObj != null) {
                        Unit unitSelected = curTileObj.GetComponent<Unit>();
                        if (unitSelected != null) {
                            SetState(GameState.PlayerViewingInfo);
                            hoverInfoDisplay.Clear();
                            unitInfoDisplay.DisplayUnitInfo(unitSelected, unitSelected.data.hp + "/" + unitSelected.data.maxHp);
                            break;
                        }
                    }
                }

                if (Input.GetKeyDown(KeyCode.V)) {
                    if (!grid.display.allDanger) {
                        foreach (UnitList unitList in unitLists) {
                        //if (unitList.team != playerTeam)
                            grid.display.DisplayGridDetails(unitList);
                        }
                    } else {
                        grid.display.StopAllDisplay();
                    }
                }

                if (Input.GetKeyDown(KeyCode.B)) {
                    Unit moraleTest = unitLists[0].mapUnits[0].GetComponent<Unit>();
                    moraleTest.data.moraleScript.UpdateUnitMorale(moraleTest,10);
                    Debug.Log(moraleTest.data.mrl);
                }

                if (Input.GetKeyDown(KeyCode.N)) {
                    Unit moraleTest = unitLists[0].mapUnits[0].GetComponent<Unit>();
                    moraleTest.data.moraleScript.UpdateUnitMorale(moraleTest,-10);
                    Debug.Log(moraleTest.data.mrl);
                }

                //other
                cursor.ManageMovement();
                hoverInfoDisplay.manageHoverInfo(curTileObj);

                break;
            
            case GameState.PlayerViewingInfo:
                if (Input.GetKeyDown(KeyCode.X)) {
                    SetState(GameState.PlayerNoSelection);
                    unitInfoDisplay.Clear();
                }
                break;

            case GameState.PlayerHasSelection: 
                //hover info
                GameObject curTileOb = cursor.grid.GetObject(cursor.gridPos);
                hoverInfoDisplay.manageHoverInfo(curTileOb);

                if (Input.GetKeyDown(KeyCode.Z) && !cursor.isMoving && !(cursor.selection.isMoving)) {

                    if (cursor.IsOnValidTarget) {
                        hoverInfoDisplay.Clear();
                        Vector2 unitEndPos = cursor.path[cursor.path.Count-1].gridPos;
                        cursor.selectedUnit.SetValidAttacks(unitEndPos);
                        targetSelect = new TargetSelectManager(cursor.selectedUnit, cursor.gridPos, unitEndPos);                                                   
                    }
                    else if (cursor.selector.MoveSelection(cursor.gridPos)) {
                        SetState(GameState.PlayerChoosingUnitAction);
                        StartCoroutine(actionSelect.DelayedInit(cursor.selection));    
                    }
                }

                if (Input.GetKeyDown(KeyCode.X) && !cursor.isMoving) {
                    cursor.SetPos(cursor.GetInitPos());
                    cursor.selector.Deselect();
                    curState = GameState.PlayerNoSelection;       
                }
                
                cursor.ManageMovement();
                break;

            case GameState.PlayerChoosingUnitAction: {
                //option to wait, set that unit to finished
                if (!cursor.selectedUnit.isMoving) {
                    if (Input.GetKeyDown(KeyCode.Z)) {
                        actionSelect.ChooseAction();     
                    }
                    if (Input.GetKeyDown(KeyCode.X)) {
                        cursor.selector.UndoMoveSelection();
                        curState = GameState.PlayerHasSelection; 
                        actionSelect.ClearDisplay();      
                    }
                    actionSelect.ManageMovement();
                    //actionSelect.Display();
                }
                break;
            }

            case GameState.PlayerChoosingMenuAction: {
                //Like UnitAction, but no unit
                if (Input.GetKeyDown(KeyCode.Z)) {
                    actionSelect.ChooseAction();     
                }
                if (Input.GetKeyDown(KeyCode.X)) {
                    curState = GameState.PlayerNoSelection; 
                    actionSelect.ClearDisplay();      
                }
                actionSelect.ManageMovement();
                break;
            }

            case GameState.PlayerChoosingTarget: {
                if (Input.GetKeyDown(KeyCode.Z)) {
                    StartCoroutine(targetSelect.ChooseTarget());     
                }
                //clears attack display, undoes movement
                if (Input.GetKeyDown(KeyCode.X)) {
                    attackManager.ClearDisplay();
                    cursor.selector.UndoMoveSelection();
                    curState = GameState.PlayerHasSelection;       
                }
                targetSelect.ManageMovement();
                break;
            }

            case GameState.NoControl: {
                //used for watching animations
                break;
            }

            case GameState.EnemyPhase: {
                //for each enemy, have them calculate actions, move them, switch states
                //debug not having ai script
                //may need a toRemove
                //prevent any possibility of not switching to playerPhase
                break;
            }

            case GameState.GameOver: {
                break;
            }

            default: {
                Debug.Log("Invalid GameState");
                break;
            }
        } 
        
    }


    
}
