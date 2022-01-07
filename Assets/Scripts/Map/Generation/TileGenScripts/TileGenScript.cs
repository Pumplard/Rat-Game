using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//BG types: instantiate one BG (tetMapGen)
//TileType: instantiate many tiles based on tile sprite list
public class TileGenScript : MonoBehaviour {

    //tile generator this script is placed in
    public TileGenerator tileGen;

    //instantiates tiles, to use actual tiles attatc them to Gameobject, make sure these are in a list then setpos
    //ignore that, add visual component to it, after setting the tiles call createObject on all of them
    //visual element stored within tilegen, not individual tiles. all tiles have space fo tilegen
    internal virtual void SetTiles() {
        for (int x = 0; x < tileGen.width; x++) {
            for (int y = 0; y < tileGen.height; y++) {
                Tile newTile = Instantiate(tileGen.tileTypes[0]);
                newTile.SetPos(x,y);
                tileGen.gridArray[x,y] = newTile;
            }
        }
    } 

    //clears any visual elements from the script
    //internal virtual void ClearDisplay() {}

    protected void SetTile(int x, int y, Tile newTile) {
        newTile.SetPos(x,y);
        tileGen.gridArray[x,y] = newTile;
    }
}
