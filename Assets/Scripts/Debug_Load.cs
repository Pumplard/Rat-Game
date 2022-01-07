using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//class for directly loading map with set units
public class Debug_Load : MonoBehaviour {
    //public MapGenerator curMap;
    public LoadLevel loader;
    void Start() {
        //if (loader.mapGenerator == null)
        //    loader.mapGenerator = curMap;
        loader.Load();
    }

}
