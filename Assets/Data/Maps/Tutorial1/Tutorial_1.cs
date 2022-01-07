using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_1 : TileGenScript
{
    public Transform bg;
    private Transform bgClone;
    //Tile = 0
    internal override void SetTiles() {
        List<Tile> tileTypes = tileGen.tileTypes;

        //bg
        bgClone = Instantiate(bg);
        int y = 0;
        int w = 18;
        
        //bottom portion
        while (y < 8) {
            for (int x = 0; x < w; x++) {
                if (y > 0) {
                    if (y == 2 || y == 3) {
                        if (x >= 1) SetTile(x, y, Instantiate(tileTypes[0]));      
                        else SetTile(x, y, Instantiate(tileTypes[1]));            
                    } else if (x >= 1 && x <=11) {
                        SetTile(x, y, Instantiate(tileTypes[0]));
                    } else SetTile(x, y, Instantiate(tileTypes[1]));
                } else SetTile(x, y, Instantiate(tileTypes[1]));
            }
            y++;
        }        
        //y == 8
        //left, mid road, connecting area
        while (y < 19) {
            for (int x = 0; x < 8; x++) {
                if (x >= 3) { 
                    if (x <= 6 || (y >= 12 && y<=15)) {
                        SetTile(x, y, Instantiate(tileTypes[0]));
                    } 
                    else SetTile(x, y, Instantiate(tileTypes[1]));
                }
                else SetTile(x, y, Instantiate(tileTypes[1]));
            }
            y++;
        }
        y=8;
        //right mid
        while (y < 19) {
            for (int x = 8; x < w; x++) {
                if (x < 16 && (y >= 10 && y<=16)) { 
                    SetTile(x, y, Instantiate(tileTypes[0]));
                }
                else SetTile(x, y, Instantiate(tileTypes[1]));
            }
            y++;
        }
        //top, y = 19;
        while (y < 24) {
            for (int x = 0; x < w; x++) {
                if (x >= 2 && x <= 16) { 
                    SetTile(x, y, Instantiate(tileTypes[0]));
                }
                else SetTile(x, y, Instantiate(tileTypes[1]));
            }
            y++;
        }     
    }

    //clears bg, since that's all there is
    //internal override void ClearDisplay() {
    //    Destroy(bgClone.gameObject);
    //}
}
