using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//displays unit info and activates on hover, deactivates on click, no clearing right now
public class UnitInfoDisplay : MonoBehaviour {

    public Transform bg;
    public RawImage bgImage;
    public GameObject statParent; //make it the same as bg if none
    public TMP_Text unitName,hp,atk,def,spd,rng,mvt,hit,avo;
    public Image portrait;
  
    

    public List<Texture> hoverTextures; //hover textures sorted by team

    public bool showStats,presetHp; //whether to show stats, or if the hp string is already set

    public bool showWeapon;
    public Image weapon;
    public TMP_Text weaponText;
    public bool showMorale;
    public MoraleDisplay morale;

    public bool showType;
    public Image mvType;

    public void Init() {
        gameObject.SetActive(false);
    }

    //used to set custom hp string, not currently used
    //sets hpPreset to true
    public void DisplayUnitInfo(UnitData unitData, string hpString) {
        hp.text = hpString;
        presetHp = true; //safety
        DisplayUnitInfo(unitData);
    }
    public virtual void DisplayUnitInfo(UnitData unitData) {
        //if (gameObject.activeSelf) {
        //    return;
        //}; //this would be optimal but this is fine
        portrait.sprite = unitData.portrait;
        unitName.text = unitData.unitName;
        if (!presetHp) {hp.text =   "HP: " + unitData.maxHp;}
        if (showStats) {
            //could make a list out of these eventually. Probably won't.
            statParent.SetActive(true);   
            atk.text = unitData.atk.GetValue().ToString();
            atk.color = unitData.atk.GetStatColor();
            def.text = unitData.def.GetValue().ToString();
            def.color = unitData.def.GetStatColor();
            spd.text = unitData.spd.GetValue().ToString();
            rng.text = unitData.minRange.GetValue() + " - " + unitData.maxRange.GetValue();
            mvt.text = unitData.mv.GetValue().ToString();
            hit.text = unitData.hit.GetValue().ToString();
            hit.color = unitData.hit.GetStatColor();
            avo.text = unitData.avo.GetValue().ToString();
            avo.color = unitData.avo.GetStatColor();
        }
        else {statParent.SetActive(false);}
        if (showWeapon) {
            if (unitData.weapon != null) {
                weapon.gameObject.SetActive(true); 
                weapon.sprite = unitData.weapon.weaponSprite;
                weaponText.text = unitData.weapon.weaponName;
            } else {weapon.gameObject.SetActive(false);}
        } 
        if (showMorale) {
            morale.Display(unitData.mrl);
        }
        if (showType) {
            mvType.gameObject.SetActive(true); 
            mvType.sprite = unitData.mvSprite;
        }
        gameObject.SetActive(true);
    }

    //"clear"
    public void Clear() {
        gameObject.SetActive(false);
    }


}
