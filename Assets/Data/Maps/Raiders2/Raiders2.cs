using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raiders2 : TileGenScript
{
    public Transform bg;
    private Transform bgClone;
    //Tile = 0
    //Water = 1
    
    private int w = 25;
    private List<Tile> tileTypes;
    
    internal override void SetTiles() {
        tileTypes = tileGen.tileTypes;

        //bg
        bgClone = Instantiate(bg);

        //used for setWater
        SetLine(24,0,2);
        SetLine(23,0,3);
        SetLine(22,0,4);
        SetLine(21,0,5);
        SetLine(20,1,6);
        SetLine(19,2,7);
        SetLine(18,4,8);
        SetLine(17,5,9);
        SetLine(16,6,8);
        SetLine(15,7,7);

        SetLine(14,12,12);
        SetLine(13,11,12);
        SetLine(12,10,13);
        SetLine(11,11,14);
        SetLine(10,12,15);    
        SetLine(9,13,16);    
        SetLine(8,14,17);              
        SetLine(7,16,19); 
        SetLine(6,17,20); 
        SetLine(5,18,21); 
        SetLine(4,19,23); 
        SetLine(3,19,24); 
        SetLine(2,21,24);
        SetLine(1,22,24); 
        SetLine(0,23,24);  
    }

    //for doing map line by line from top left based on xMax/Min and y
    private void SetLine(int y, int xMin, int xMax) {
        for(int x = 0; x < w; x++) {
            if (x >= xMin && x <= xMax) {
                SetTile(x, y, Instantiate(tileTypes[1]));
            }
            else SetTile(x, y, Instantiate(tileTypes[0]));
        }
    }

}
