using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//used to call tileGenScripts. It was required due to previous method of loading tiles. Should be replaced/ combined with a different script

[CreateAssetMenu(fileName = "tileGen", menuName = "TileGenerator")]
public class TileGenerator : ScriptableObject {
    public int width;
    public int height;
    public List<Tile> tileTypes;
    public TileGenScript tileScript;

    internal Tile[,] gridArray;

    //tile script modifies grid array in this script
    public Tile[,] GenerateTileArray() {
        gridArray = new Tile[width, height];
        tileScript.SetTiles();
        return gridArray;
    }

    //public void RemoveTiles() {
   //     foreach (Tile tile in gridArray) {
    //        Destroy(tile);
    //    }
        //gridarray cleared upon new generation
    //    tileScript.ClearDisplay();
    //}
}
