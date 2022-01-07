using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Not bought, playerGroup set differently
//Leaders have groupicons that are assigned to Memebers
[CreateAssetMenu(fileName = "New LeadUnit", menuName = "LeadUnitData")]
[System.Serializable]
public class LeadUnitData : UnitData {
    public LeadUnitBuff teamBuff;
    public Sprite groupIcon;

}
