using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A single instance scriptable object that contains mapGen data that is accessed by the MapManager.
//This is used to transfer mapGen data between scenes
//whenever a map is selected, mapGen is set, and mapGen is used by MapManager in the new scene
[CreateAssetMenu(fileName = "MapGenSelector", menuName = "MapGenSelector")]
public class MapGenSelector : ScriptableObject {
    public MapGenerator mapGen;

    public void SetMap(MapGenerator newMap){
        mapGen = newMap;
    }

}
