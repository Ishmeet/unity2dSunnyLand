using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cherry : MonoBehaviour
{
  public Animator animator;
  public void OnTouch()
  {
    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Cherry_Fx"))
    {
      animator.SetTrigger("Fx");
    }
  }

  private void Took()
  {
    Destroy(this.gameObject, 0f);
  }
}
