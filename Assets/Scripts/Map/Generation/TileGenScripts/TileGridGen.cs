using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//generates tiles from unity grid
public class TileGridGen : TileGenScript {

    //grid prefab must contain tiles
    public Grid gridPrefab;
    private Transform tileParent; //contains tiles in hierarchy

    //instantiates grid into scene, inserts all tilescripts in grid into mapGrid

    internal override void SetTiles() {
        //initialization
        Transform parent = gridPrefab.GetComponent<Transform>();
        List<Transform> children = new List<Transform>();//tile tranforms
        Random.InitState(1945);//sets seed
        //sets hierarchy parent
        GameObject parentObject = new GameObject("mapTileParent");
        tileParent = parentObject.GetComponent<Transform>();

        //instantiates tiles into scene/grid
        int childCount = parent.childCount;
        for (int i = 0; i < childCount; ++i) {
            Transform curChild = parent.GetChild(i);
            int x = (int)curChild.position.x;
            int y = (int)curChild.position.y;
            DisplayTile curTile = curChild.GetComponent<DisplayTile>();
            
            //Instantiates displayTile
            SpriteRenderer curDisplayTile = Instantiate(curTile.tileDisplay, new Vector3(x,y,0), Quaternion.identity, tileParent);
            //sets instance of tileData in grid
            SetTile(x, y, Instantiate(curTile.tileData));

            //changes sprite of tile if random tile
            if (curTile.isRandom) {
                int r = Random.Range(0,curTile.spriteCount);
                curDisplayTile.sprite = curTile.GetSprite(r);
            }
        }
    }
}
