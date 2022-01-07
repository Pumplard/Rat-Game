using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//tiles used in tilegenscripts to instantiate attatched prefab into world
//attatched "tile" is inserted into grid
public class DisplayTile : MonoBehaviour {
    public SpriteRenderer tileDisplay;
    public Tile tileData;

    public bool isRandom = false; //bool is checked during generation to tell if this is tile should be randomized or not
    public List<Sprite> randomSprites;

    public int spriteCount { get {return randomSprites.Count;} }

    //only used for random Tiles
    public Sprite GetSprite(int i) {
        return randomSprites[i];
    }


}
