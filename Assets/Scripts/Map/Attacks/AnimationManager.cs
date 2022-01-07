using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimationManager : MonoBehaviour
{
    public RectTransform bg;
    public Image canvasAttacker, canvasDefender; //inspector, blank display templates used to show animations
    public TMP_Text aMissText;
    public TMP_Text dMissText;

    public GameObject aHit, dHit; //objects to display hits

    //private bool active; //used for a coroutine
    private Animator attackerAnim, defenderAnim;
    private Vector2 aInitPos, dInitPos;


    //use for map fighting animations
    void Awake() {
        bg.gameObject.SetActive(false);
        canvasAttacker.gameObject.SetActive(false);
        canvasDefender.gameObject.SetActive(false);
        aMissText.gameObject.SetActive(false);
        dMissText.gameObject.SetActive(false);
        aHit.SetActive(false);
        dHit.SetActive(false);
        //characterTemp.gameObject.SetActive(false);
    }

    //initializes animator, used every attack animation, called by attackmanager
    public void InitializeAnimation(Unit attacker, Unit defender) {
       // active = true;
        SpriteRenderer attackerSpriteR = attacker.GetComponent<SpriteRenderer>();
        SpriteRenderer defenderSpriteR = defender.GetComponent<SpriteRenderer>();
        canvasAttacker.sprite = attackerSpriteR.sprite;
        canvasDefender.sprite = defenderSpriteR.sprite;

       // attackerAnim = attacker.animator;
       // defenderAnim = defender.animator;

        attackerAnim = canvasAttacker.GetComponent<Animator>(); //didn't want to put it in the inspector
        defenderAnim = canvasDefender.GetComponent<Animator>();
        attackerAnim.runtimeAnimatorController = attacker.animator.runtimeAnimatorController;
        defenderAnim.runtimeAnimatorController = defender.animator.runtimeAnimatorController;


        bg.gameObject.SetActive(true);
        canvasAttacker.gameObject.SetActive(true);
        canvasDefender.gameObject.SetActive(true);

        //start Idle animations
        if (attackerAnim != null)
            attackerAnim.Play("unit_attack_idle");
        if (defenderAnim != null)
            defenderAnim.Play("unit_attack_idle");



        //Sets transforms to correct sizes and sets initial positions
        //updates animations to get sprite data
        attackerAnim.Update(0);
        canvasAttacker.SetNativeSize();
        aInitPos = canvasAttacker.GetComponent<RectTransform>().anchoredPosition;


        defenderAnim.Update(0);
        canvasDefender.SetNativeSize();
        dInitPos = canvasDefender.GetComponent<RectTransform>().anchoredPosition;
        
    }

    //takes in a or d (attacker or defender), and if they will hit.
    public IEnumerator PlayAttack(bool isAttacker, bool hit) {
        //sets animator
        Animator animator;
        if (isAttacker) {
            animator  = attackerAnim;
        }
        else animator = defenderAnim;

        //sets layer above defender
        animator.GetComponent<RectTransform>().SetAsLastSibling();
        
        //starts attack animation after delay
        yield return new WaitForSeconds(0.5f);
        if (isAttacker)
            animator.Play("unit_attack");
        else
            animator.Play("unit_attack_mirror");
        animator.Update(0); //without update, animator still thinks idle is playing since frame hasn't updated

        //waits for attack to finish
        float attackLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(attackLength);

        //shows miss text if missed
        if (!hit) {
            if (isAttacker) {aMissText.gameObject.SetActive(true);}
            else dMissText.gameObject.SetActive(true);
        }

        //sets animations back
        animator.Play("unit_attack_idle");

        //delay 2
        yield return new WaitForSeconds(0.5f);

        //removes miss text
        if (!hit) {
            if (isAttacker) {aMissText.gameObject.SetActive(false);}
            else dMissText.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(0.5f); 
    }

    public void EndAnimation() {
        //sets animations back
        //defenderAnim.Play("unit_acted");
        //attackerAnim.Play("unit_acted");


        //sets inactive, ends syncAnimations
        bg.gameObject.SetActive(false);
        canvasAttacker.GetComponent<RectTransform>().anchoredPosition = aInitPos; 
        canvasDefender.GetComponent<RectTransform>().anchoredPosition = dInitPos;
        //active = false;
    }

    //old solution to use spriterender images rather than canvas
    //public IEnumerator SyncAnimations(Image canvasUnit, SpriteRenderer gameUnit) {
    //    while (active == true) {
    //        canvasUnit.sprite = gameUnit.sprite;
    //        yield return null;
    //   }
    //}


}
