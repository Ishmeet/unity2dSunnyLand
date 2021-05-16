using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
  public Animator animator;
  public void OnTouch()
  {
    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Gem_Fx")) {
        animator.SetTrigger("Touch");
    }
  }

  private void Took()
  {
    Destroy(this.gameObject, 0f);
  }
}
