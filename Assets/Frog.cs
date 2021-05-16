using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : MonoBehaviour
{

  public playerMovement PlayerMovement;
    public Animator animator;

  bool facingLeft = true;
  private void FixedUpdate()
  {
    if ((PlayerMovement.transform.position.x > gameObject.transform.position.x) && facingLeft)
    {
      Flip();
    }
    else if ((PlayerMovement.transform.position.x < gameObject.transform.position.x) && !facingLeft)
    {
      Flip();
    }
  }
  private void Flip()
  {
    facingLeft = !facingLeft;
    // Multiply the player's x local scale by -1.
    Vector3 theScale = gameObject.transform.localScale;
    theScale.x *= -1;
    gameObject.transform.localScale = theScale;
  }
  public void OnTrampoline()
  {
      animator.SetTrigger("Death");
  }

  private void Death(){
      Destroy(this.gameObject, 0f);
  }
}
