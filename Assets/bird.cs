using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bird : MonoBehaviour
{
    public playerMovement PlayerMovement;
    Rigidbody2D enemyRigidBody2D;
    public Animator animator;
    bool facingLeft = true;
    private float _startPos;
    private float _endPos;
    public int UnitsToMove = 5;
    public float EnemySpeed = 500;
    public bool _moveUp = true;
  public void Awake()
  {
    enemyRigidBody2D = GetComponent<Rigidbody2D>();
    _startPos = transform.position.y;
    _endPos = _startPos + UnitsToMove;
  }
    private void FixedUpdate() {

      if (_moveUp)
      {
        enemyRigidBody2D.AddForce(Vector2.up * EnemySpeed * Time.deltaTime);
      }

      if (enemyRigidBody2D.position.y >= _endPos)
        _moveUp = false;

      if (!_moveUp)
      {
        enemyRigidBody2D.AddForce(-Vector2.up * EnemySpeed * Time.deltaTime);
      }

      if (enemyRigidBody2D.position.y <= _startPos)
        _moveUp = true;


        if ((PlayerMovement.transform.position.x > gameObject.transform.position.x) && facingLeft) {
            Flip();
        } else if((PlayerMovement.transform.position.x < gameObject.transform.position.x) && !facingLeft) {
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

  private void Death()
  {
    Destroy(this.gameObject, 0f);
  }
}
