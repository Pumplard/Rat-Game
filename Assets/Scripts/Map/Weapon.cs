using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "baseWeapon", menuName = "Weapon")]
[System.Serializable]
public class Weapon : ScriptableObject {
   public string weaponName;
   public int mt = 5;
   public int radius = 0; //radius if AOE
   public Sprite weaponSprite;
   public AttackScript attackScript; //specifies any additional effects when attacking



   //right now equipped in unitshop
   public void Equip(UnitData unitData) {
      unitData.atk.SetWeaponVal(mt);
   }

   //called from unitdata
   public void UnEquip(UnitData unitData) {
      unitData.atk.SetWeaponVal(0);
   }
   
}
