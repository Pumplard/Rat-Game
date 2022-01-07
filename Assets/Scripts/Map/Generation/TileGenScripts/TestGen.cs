using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGen : TileGenScript
{
    public Transform bg;
    private Transform bgClone;
    //Tile = 0
    //Rock = 1
    //Water = 2
    //Sand = 3
    internal override void SetTiles() {
        List<Tile> tileTypes = tileGen.tileTypes;

        //bg
        bgClone = Instantiate(bg);

        for (int x = 0; x < tileGen.width; x++) {
            for (int y = 0; y < tileGen.height; y++) {
                if  ((x < 10 || x > 12) && y >= 10 && y <= 14) {
                    if (y == 10 || y == 14)
                        SetTile(x, y, Instantiate(tileTypes[3]));
                    else
                        SetTile(x, y, Instantiate(tileTypes[2]));                        
                }
                else {
                    SetTile(x, y, Instantiate(tileTypes[0]));
                }
            }
        }
        Destroy(tileGen.gridArray[4,6]);
        Destroy(tileGen.gridArray[4,8]);
        Destroy(tileGen.gridArray[7,7]);
        SetTile(4, 6, Instantiate(tileTypes[1]));
        SetTile(4, 8, Instantiate(tileTypes[1]));
        SetTile(7, 7, Instantiate(tileTypes[1]));
        
    }

    //internal override void ClearDisplay() {
    //    Destroy(bgClone.gameObject);
    //}
}
