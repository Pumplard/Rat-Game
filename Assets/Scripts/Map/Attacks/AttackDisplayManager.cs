using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//this has nothing to do with the actual attack at the moment, just the info
//I should really move that here though.
public class AttackDisplayManager : MonoBehaviour {
    public MapUnitInfoDisplay player, target;
    public TMP_Text pHitText, tHitText, pAtk, tAtk, px2, tx2;

    public Transform mapEffectTemplate; //transform with sprite to be instantiated for map effects
    private List<Transform> mapEffectDisplay; //list of transforms displayed for AOE attacks, set by attackScript, needs to be cleared

    //called by attackManager init, whihc is called by mapmanager
    public void Init() {
        gameObject.SetActive(false);
        player.Init();
        target.Init();
    }


    //displays unit info, first unit is player second is enemy
    //also displays additional mapEffect and UIeffects (status effects)
    //called in attackmanager
    public void Display(Unit unit1, int newHP1, Unit unit2, int newHP2) {
        ClearEffects();
        player.DisplayUnitInfo(unit1, "HP: " + unit1.data.hp + " -> " + newHP1);
        target.DisplayUnitInfo(unit2, "HP: " + unit2.data.hp + " -> " + newHP2);
        pAtk.text = "Atk:" + unit1.data.atk.GetValue();
        tAtk.text = "Atk:" + unit2.data.atk.GetValue();
        pHitText.text = unit1.GetHitPercent(unit2) + "%";
        tHitText.text = unit2.GetHitPercent(unit1) + "%";

        //follow up display
        if (unit1.CheckFollowUp(unit2)) {
            px2.gameObject.SetActive(true);
        }
        else px2.gameObject.SetActive(false);
 
        if (unit2.CheckFollowUp(unit1)) {
            tx2.gameObject.SetActive(true);
        }
        else tx2.gameObject.SetActive(false);

        //unit weapon UI/map effects
        //most likely null
        mapEffectDisplay = unit1.data.weapon.attackScript.MapEffect(mapEffectTemplate, unit1);

        gameObject.SetActive(true);
    }

    public void Clear() {
        gameObject.SetActive(false);
        ClearEffects();
    }

    //called in clear and every display
    //Clears status effects and map effects
    private void ClearEffects() {
        if (mapEffectDisplay != null) {
            foreach (Transform sprite in mapEffectDisplay) {
                Destroy(sprite.gameObject);
            }
            mapEffectDisplay.Clear();
            mapEffectDisplay = null;
        }
    }
}
