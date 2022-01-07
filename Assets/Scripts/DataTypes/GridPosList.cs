using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GridPosList", menuName = "GridPosList")]
[System.Serializable]
public class GridPosList : ScriptableObject {
    //last element in list is the leadUnit spawn
    public Vector2[] coords = new Vector2[15];
}
