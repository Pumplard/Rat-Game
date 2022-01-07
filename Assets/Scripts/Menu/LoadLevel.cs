using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{   
    public MapGenSelector mapGenSelector;
    public MapGenerator mapGenerator;
    public List<UnitList> playerGroups;
    public List<UnitList> mapPlayerGroups;
    //public string levelName;

    //sets the map for the instance of mapmanager in the next scene to load
    //There should only be one instance of mapManager, this script edits the override prefab
    //Takes in lists of playergroups to transfer to mapPlayerGroups
    public void Load() {

        //resets mapGroups
        foreach (UnitList group in mapPlayerGroups) {
            group.Reset();
        }

        //set mapGroups
        int i = 0;
        foreach (UnitList group in playerGroups) {
            mapPlayerGroups[i].SetLeadUnit(group.leadUnit);
            mapPlayerGroups[i].AddUnits(group.units);
            i++;
        }
        mapGenSelector.SetMap(mapGenerator);
        SceneManager.LoadScene("Map");
    }
}
