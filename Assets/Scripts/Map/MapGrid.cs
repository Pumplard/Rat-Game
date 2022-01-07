using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Creates grid array of GridObjects, takes in width, height based on map size
public class MapGrid { 
    public MapManager manager;
    public GridDisplayManager display = ScriptableObject.CreateInstance<GridDisplayManager>();

    private int width;
    private int height;
    private Tile[,] gridArray;

    //Default constructor, initializes dimensions and array of default tiles
    //later, instantiate tile from list of tiletypeS. Mapmanager will change specific tiles?
    public MapGrid(TileGenerator tileGen) {
        this.width = tileGen.width;
        this.height = tileGen.height;
        gridArray = tileGen.GenerateTileArray();
    }

    public void Init(MapManager newManager) {
        manager = newManager;
        display.Init(this, width, height);
    }
    public int MaxSize {
        get {
            return width * height;
        }
    }

    //may return null
    public Tile GetTile(Vector2 gridPos) {
        return gridArray[(int)gridPos.x, (int)gridPos.y];
    }

    //returns list of valid neighbor tiles. ignore this
    public List<Tile> GetNeighbors(Tile tile) {
        List<Tile> neighbors = new List<Tile>();
        for (int x = -1; x <= 1; x++) {
                if (x != 0) {
                    Vector2 gridPos = tile.gridPos;
                    Vector2 newPosx = new Vector2((gridPos.x + x), (gridPos.y));
                    Vector2 newPosy = new Vector2((gridPos.x), (gridPos.y + x));
                    if (IsValidPos(newPosx))
                        neighbors.Add(GetTile(newPosx));
                    if (IsValidPos(newPosy))
                        neighbors.Add(GetTile(newPosy));
            }          
        }
        return neighbors;
    }

    public GameObject GetObject(Vector2 gridPos) {
        int x = (int)gridPos.x;
        int y = (int)gridPos.y;
        return gridArray[x,y].tileObject;
    }

    public void Insert(Vector2 gridPos, GameObject unit) {
        int x = (int)gridPos.x;
        int y = (int)gridPos.y;
        gridArray[x,y].tileObject = unit;
    }

    public void Empty(Vector2 gridPos) {
        int x = (int)gridPos.x;
        int y = (int)gridPos.y;
        gridArray[x,y].tileObject = null;
    }
    
    public bool IsValidPos(Vector2 gridPos) {
        var x = gridPos.x;
        var y = gridPos.y;
        if (width > x && x >= 0 && height > y && y >= 0)
            return true;
        else
            return false;
    }

}
