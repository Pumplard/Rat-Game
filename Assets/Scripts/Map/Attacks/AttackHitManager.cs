using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//one manager on unit animation for each hit object
//plays the animation on referenced object, not this.gameObject
public class AttackHitManager : MonoBehaviour {
    public GameObject hitObject;
    public void PlayHitAnimation() {
        StartCoroutine(HitAnimation());
    } 

    private IEnumerator HitAnimation() {
        Debug.Log("hitting");
        Animator animator = hitObject.GetComponent<Animator>();
        hitObject.SetActive(true);
        animator.Play("hit");
        animator.Update(0);
        float attackLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(attackLength);
        hitObject.SetActive(false);
    }
}
