using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//default attack script (not abstract), no effects.
public class AttackScript : MonoBehaviour {
    

    //called in AttackDisplayManager, for additional map display when selecting target (like AOE damage)
    //returns list of anything displayed so it can be removed
    public virtual List<Transform> MapEffect(Transform template, Unit unit) { return null;}

    //called in AttackDisplayManager, for additional UI display when selecting target (like status effects)    
    public virtual void UIEffect() {}


    //called in attack manager every time attacking
    public virtual void AttackEffect(Unit unit, Unit target) {}


    //called in attack manager for display HP, doesn't take missing into account
    //changes return hp for unit holding weapon or adds UIeffects in results
    //Right now used for bomb rats
    public virtual int ResultHitEffect(int curHp) { return curHp; }

}
