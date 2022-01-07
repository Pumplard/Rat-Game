using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Things that can edit player groups
//need to have groups initialized at start of scene
//initialiing them still doesn't cause them to awake
//shop, teamdisplay, unitswap, leaderdropdown
public class GroupEditor : MonoBehaviour
{
    public List<UnitList> playerGroups;   
    public void SetGroups(List<UnitList> newGroups) {
        playerGroups = newGroups;
    }
}
