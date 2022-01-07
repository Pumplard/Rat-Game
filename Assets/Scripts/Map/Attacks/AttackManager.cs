using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A single instance of attackmanager is used for mapmanager and unitAI
[CreateAssetMenu(fileName = "AttackManager", menuName = "AttackManager")]
public class AttackManager : ScriptableObject {

  public AttackDisplayManager display;
  internal MapManager manager;



  //private int playerHP, targetHP; //used as hp results


  //called by mapmanager
  public void Init(MapManager newManager, AttackDisplayManager displayManager) {
    manager = newManager;
    display = displayManager;
    display.Init();
  }


  //clears display, does attack, 
  //remember unit may equal null after the attack is done
  public IEnumerator DoAttacks(Unit attacker, Unit defender) {
    //clears unit info
    ClearDisplay();
    //initializes animator w/attacker and defender
    AnimationManager aManager = manager.animationManager;
    aManager.InitializeAnimation(attacker, defender);

    //List<bool> combatChecks = GetCombatChecks(attacker,defender);
    bool canCounter = CheckCounterAttack(attacker, defender);

    //calculates damage done with each hit
    Tuple<int,int> damage = CalcDamage(attacker, defender, canCounter);
    
    //first attack. checks if attacker will hit. ends if defender dies.
    bool willHit = WillHit(attacker, defender);
    yield return aManager.StartCoroutine(aManager.PlayAttack(true, willHit));
    if (willHit) {
      defender.TakeDamage(damage.Item2);
      attacker.data.weapon.attackScript.AttackEffect(attacker, defender);
      if (!defender.isAlive || !attacker.isAlive) {
        aManager.EndAnimation();
        EndAttack(attacker, defender);
        yield break;
      }
    } 

    //counter attack
    if (canCounter) {
      willHit = WillHit(defender,attacker);
      yield return aManager.StartCoroutine(aManager.PlayAttack(false, willHit));
      if (willHit) {
        attacker.TakeDamage(damage.Item1);
        defender.data.weapon.attackScript.AttackEffect(defender, attacker);
      } 

      if (!defender.isAlive || !attacker.isAlive) {
        aManager.EndAnimation();
        EndAttack(attacker, defender);
        yield break;
      }
    }

    //followup attack
    if (attacker.CheckFollowUp(defender)) {
      willHit = WillHit(attacker, defender);
      yield return aManager.StartCoroutine(aManager.PlayAttack(true, willHit));
      if (willHit) {
        defender.TakeDamage(damage.Item2);
        attacker.data.weapon.attackScript.AttackEffect(attacker, defender);
        if (!defender.isAlive || !attacker.isAlive) {
          aManager.EndAnimation();
          EndAttack(attacker, defender);
          yield break;
        }
      }
    }

    //followup counter
    if (canCounter && defender.CheckFollowUp(attacker)) {
      willHit = WillHit(defender,attacker);
      yield return aManager.StartCoroutine(aManager.PlayAttack(false, willHit));
      if (willHit) {
        attacker.TakeDamage(damage.Item1);
        defender.data.weapon.attackScript.AttackEffect(defender, attacker);
      } 

      if (!defender.isAlive || !attacker.isAlive) {
        aManager.EndAnimation();
        EndAttack(attacker, defender);
        yield break;
      }
    }

    //ends attack
    aManager.EndAnimation();
    EndAttack(attacker, defender);
  }


  //called when the attack is over
  //removes dead units
  private void EndAttack(Unit attacker, Unit defender) {
    if (!attacker.isAlive) {
      manager.RemoveUnit(manager.unitLists[attacker.group], attacker);
    }
    if (!defender.isAlive) {
      manager.RemoveUnit(manager.unitLists[defender.group], defender);
    }
  }

  //checks if defender can counterattack, retruns bool
  public bool CheckCounterAttack(Unit attacker, Unit defender) {
    return CheckCounterAttack(attacker, defender, attacker.gridPos);
  }

  //version for attacking without performing moving animation first
  public bool CheckCounterAttack(Unit attacker, Unit defender, Vector2 attackerPos) {
    Vector2 diff = defender.gridPos - attackerPos;
    int d = (int)(Mathf.Abs(diff.x) + Mathf.Abs(diff.y));
    if (defender.data.minRange.GetValue() <= d && d <= defender.data.maxRange.GetValue()) {
      return true;
    }
    else return false;
  }

  //Calculates damage without followup attacks
  public Tuple<int,int> CalcDamage(Unit attacker, Unit defender, bool canCounter) {
    int aTaken = 0;
    if (canCounter) {
      aTaken = (defender.data.atk.GetValue() - attacker.data.def.GetValue());
    }

    int dTaken = (attacker.data.atk.GetValue() - defender.data.def.GetValue());

    if (aTaken <0) aTaken = 0;
    if (dTaken <0) dTaken = 0;
    return new Tuple<int,int>(aTaken, dTaken);
  }

  //tells if the unit will hit
  public bool WillHit(Unit attacker, Unit defender) {
    int hitPercent = attacker.GetHitPercent(defender);
    float r = UnityEngine.Random.Range(0.0f,100.0f);
    Debug.Log(r);
    if (r <= hitPercent) {
      return true;
    }
    else return false;
  }



  //Display//

  //called in targetselectManagager
  //gets attack result for display and calls display, not used on enemyphase
  public void DisplayAttackInfo(Unit player, GameObject targetObject, Vector2 endPos) {
    Unit target = targetObject.GetComponent<Unit>();
    if (target != null) {

      bool canCounter = CheckCounterAttack(player, target, endPos);
      Tuple<int,int> damage = CalcDamage(player, target, canCounter);



      //player attack
      int playerHP = player.data.hp;
      int targetHP = (target.data.hp - damage.Item2);
      playerHP = player.data.weapon.attackScript.ResultHitEffect(playerHP);

      //counter
      bool targetFollowUp = false;
      if (canCounter && targetHP > 0) {
        playerHP -= damage.Item1;
        targetFollowUp = target.CheckFollowUp(player);

      }

      //player followup
      if (player.CheckFollowUp(target) && playerHP > 0) {
        targetHP -= damage.Item2;
        playerHP = player.data.weapon.attackScript.ResultHitEffect(playerHP);
      }

      //target followup
      if (targetFollowUp && targetHP > 0) {
          playerHP -= damage.Item1;
      }

      if (playerHP < 0) {
        playerHP = 0;
      }
      if (targetHP < 0) {
        targetHP = 0;
      }

      display.Display(player, playerHP, target, targetHP);
    } 
  }

  public void ClearDisplay() {
    display.Clear();
  }

}
