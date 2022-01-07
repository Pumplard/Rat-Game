using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    
    public MapGrid grid;
    //cursordisplay?
    public SelectManager selector;

    public MapCamera mapCamera;
    public float camSpeed = 10f;
    
    private SpriteRenderer spriteRenderer;
    public Sprite cursorSprite;
    public Vector2 gridPos;
    internal bool hasSelection = false;

    internal bool isMoving = false;
    private float timetoMove = 0.1f;

    //path for pathfindings
    public PathManager pathfinding;
    internal List<Tile> path; //not a reference?
    

    void Awake() {
        gridPos = transform.position;
        path = new List<Tile>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = cursorSprite;
    }

    public void Init(MapGrid newGrid) {
        grid = newGrid;
    }

    internal GridMovable selection {
        get { return selector.selection; }
    }

    internal Unit selectedUnit {
        get { return selector.selection.GetComponent<Unit>(); }
    }

    internal GridMovable curPosMovable {
        get { return grid.GetObject(gridPos).GetComponent<GridMovable>(); }
    }

    internal Unit curPosUnit {
        get { return grid.GetObject(gridPos).GetComponent<Unit>(); }
    }

    public bool IsOnValidMove { 
        get { return grid.GetTile(gridPos).validMove; }
    }

    public bool IsOnValidAtk { 
        get { return grid.GetTile(gridPos).validAtk; }
    }

    public bool IsOnValidTarget { 
        get {
            if (grid.GetObject(gridPos) != null && IsOnValidAtk && !IsOnSelection)
                return true;
            else
                return false;
        }
    }
        
    public bool IsOnSelection { 
        get {
            if (gridPos == selector.selection.gridPos)
                return true;
            else
                return false;
        }
    }


    public void SetPos(Vector2 newPos) {
        transform.position = newPos;
        gridPos = newPos;
        SetCameraPos(newPos);
        //UpdatePath(); //deselect clears display
    }

    public void SetCameraPos(Vector2 newPos){
        mapCamera.SetPos(newPos);
    }

    public void UpdatePath() {
        if (hasSelection == true) {
            //if (IsOnValidTarget) {                   
            //    path = pathfinding.ToTargetPath(selectedUnit, gridPos);
           //     grid.display.DisplayPath(path);
            //    return;  
            //}
            //if (IsOnValidMove && !IsOnSelection && !grid.GetTile(gridPos).IsEmpty()) {
           //     grid.display.DisplayPath(path);
            //    return;
            //}
            //can make foundpath change this path?
            pathfinding.FindPath(selectedUnit.initPos, gridPos);
            path = pathfinding.foundPath;
            grid.display.DisplayPath(path);
        }      
    }

    //Gets initPos of cursor based on initPos of its selection
    public Vector2 GetInitPos() {
        if (selector.selection != null)
            return selector.selection.initPos;
        else 
            return gridPos;
    }

    internal void ManageMovement() {
        if (!isMoving) {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                StartCoroutine(MovePlayer(Vector2.up));
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                StartCoroutine(MovePlayer(Vector2.left));
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                StartCoroutine(MovePlayer(Vector2.down));
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                StartCoroutine(MovePlayer(Vector2.right)); 
        }
    }

    //Movement for player cursor
    internal IEnumerator MovePlayer(Vector2 direction) {
        Vector2 startPos = transform.position;
        Vector2 endPos = startPos + direction;

        if (grid.IsValidPos(endPos)) {
            isMoving = true;
            float elapsedTime = 0;
            gridPos = gridPos + direction;
            transform.position = endPos;
            SetCameraPos(gridPos);

            while(elapsedTime < timetoMove)
            {
                //transform.position = Vector2.Lerp(startPos, endPos, (elapsedTime/ timetoMove));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            if (IsOnValidAtk || IsOnValidMove) {
                UpdatePath();
            }
            isMoving = false;
        }
    }
}
