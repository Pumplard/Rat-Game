using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Animations;
using UnityEngine;


//Contains stats and assets for unit (everything that isn't only map-relevanbt)
//instantiated from player's unit list to their map unit list on start of level
//copied back to playerlist on level end
[CreateAssetMenu(fileName = "New UnitData", menuName = "UnitData")]
[System.Serializable]
public class UnitData : ScriptableObject {
    //public AnimatorController aController;
    public RuntimeAnimatorController aController;
    
    public string unitName;
    public int maxHp = 0;
    public int hp = 0;

    private int baseMrl = 60;
    internal int mrl = 60;
    //public int baseAtk = 6;    
    public Stat atk, def, spd, hit, avo, mv, minRange, maxRange;
    public Weapon weapon;
    public MoraleScript moraleScript;

    public int playerGroup = -1;//player group number when not on the map, for data
    public AIType ai;//
    public Sprite frame, portrait, initSprite, mvSprite;//

    //shop
    public int cost = 100;


    public void SetPlayerGroup(int newGroup) {
        playerGroup = newGroup;
    }

    //restores units status to normal (restores hp, clears status effects, resets morale)
    //used post map
    public void RestoreStatus() {
        hp = maxHp;
        mrl = baseMrl;
        //imagine having a list
        atk.ClearModifiers();
        def.ClearModifiers();
        spd.ClearModifiers();
        hit.ClearModifiers();
        avo.ClearModifiers();
        mv.ClearModifiers();
        minRange.ClearModifiers();
        maxRange.ClearModifiers();
    }

    //called in mapgen to get AI from unitList
    public void SetAI(AIType newScript) {
        ai = newScript;
    }

    //right now, weapons are only equipped in unitdata and mapGen
    //Weapon modifiers are the only ones applied before the map
    //makes sure no duplicate
    public void EquipWeapon(Weapon newWeapon) {
        weapon.UnEquip(this);
        newWeapon.Equip(this);
    }

    //right now used in unit.initialize during mapgen, equips and unequips all weapons to make sure mt is correct
    public void UnEquipWeapon() {
        weapon.UnEquip(this);
    }
}
