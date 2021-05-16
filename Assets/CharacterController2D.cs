using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
  [SerializeField] private float m_JumpForce = 700f;              // Amount of force added when the player jumps.
  [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;      // Amount of maxSpeed applied to crouching movement. 1 = 100%
  [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
  [SerializeField] private bool m_AirControl = false;             // Whether or not a player can steer while jumping;
  [SerializeField] private LayerMask m_WhatIsGround;              // A mask determining what is ground to the character
  [SerializeField] private LayerMask m_WhatIsEnemy;               // A mask determining what is enemy to the character
  [SerializeField] private Transform m_GroundCheck;             // A position marking where to check if the player is grounded.
  [SerializeField] private Transform m_CeilingCheck;              // A position marking where to check for ceilings
  [SerializeField] private Collider2D m_CrouchDisableCollider;        // A collider that will be disabled when crouching
  [SerializeField] private Collider2D m_JumpDisableCollider;        // A collider that will be disabled when Jump Rising.

  const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
  private bool m_Grounded;            // Whether or not the player is grounded.
  private bool m_Hurt;            // Whether or not the player is hurt.
  const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
  const float k_PlayerRadius = 1f;
  const float m_HurtForce = 200f;              // Amount of force added when the player is hurt.

  private Rigidbody2D m_Rigidbody2D;
  private bool m_FacingRight = true;  // For determining which way the player is currently facing.
  private Vector3 m_Velocity = Vector3.zero;

  [Header("Events")]
  [Space]

  public UnityEvent OnLandEvent;

  [System.Serializable]
  public class BoolEvent : UnityEvent<bool> { }

  public BoolEvent OnCrouchEvent;

  [System.Serializable]
  public class BoolEventFall : UnityEvent<bool> { }

  [System.Serializable]
  public class BoolEventRise : UnityEvent<bool> { }

  [System.Serializable]
  public class StringEventDie : UnityEvent<string> { }
  
  [System.Serializable]
  public class StringEventCollectible : UnityEvent<string> { }
  public StringEventDie onEnemyDeath;
  public StringEventCollectible onCollectible;
  public UnityEvent OnHurtEvent;
  public BoolEventFall IsFallingEvent;
  public BoolEventRise IsRisingEvent;
  private bool m_wasCrouching = false;

  private void Awake()
  {
    m_Rigidbody2D = GetComponent<Rigidbody2D>();

    if (OnLandEvent == null)
      OnLandEvent = new UnityEvent();

    if (OnHurtEvent == null)
      OnHurtEvent = new UnityEvent();

    if (OnCrouchEvent == null)
      OnCrouchEvent = new BoolEvent();

    if (IsFallingEvent == null)
      IsFallingEvent = new BoolEventFall();

    if (IsRisingEvent == null)
      IsRisingEvent = new BoolEventRise();

    if (onEnemyDeath == null)
      onEnemyDeath = new StringEventDie();
    
    if (onCollectible == null)
      onCollectible = new StringEventCollectible();
  }

  private void FixedUpdate()
  {
    bool wasGrounded = m_Grounded;
    m_Grounded = false;

    // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
    // This can be done using layers instead but Sample Assets will not overwrite your project settings.
    Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
    for (int i = 0; i < colliders.Length; i++) {

      if (colliders[i].gameObject.tag == "Enemy") {
        
        onEnemyDeath.Invoke(colliders[i].gameObject.name);

        // Add a vertical force to the player.
        m_Grounded = false;
        m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce / 2));
      } else if (colliders[i].gameObject.tag == "Gem" || colliders[i].gameObject.tag == "Cherry" ) {
        onCollectible.Invoke(colliders[i].gameObject.name);
      } 

      if (colliders[i].gameObject != gameObject)
      {
        m_Grounded = true;
        m_Hurt = false;
        if (m_JumpDisableCollider != null)
          m_JumpDisableCollider.enabled = false;
        if (!wasGrounded)
          OnLandEvent.Invoke();
      }
    }

    // Hurt logic
    Collider2D[] collider = Physics2D.OverlapCircleAll(gameObject.transform.position, k_PlayerRadius, m_WhatIsEnemy);
      for (int i = 0; i < collider.Length; i++) {
        if (collider[i].gameObject.tag == "Enemy") {
          m_Grounded = false;
          m_Hurt = true;
          if (collider[i].gameObject.transform.position.x > gameObject.transform.position.x)
          {
            // Player - Enemy 
            m_Rigidbody2D.AddForce(new Vector2(-m_HurtForce, m_HurtForce));
          }
          else
          {
            //  Enemy - Player
            m_Rigidbody2D.AddForce(new Vector2(m_HurtForce, m_HurtForce));
          }
          OnHurtEvent.Invoke();
        } else if (collider[i].gameObject.tag == "Gem" || collider[i].gameObject.tag == "Cherry") {
          onCollectible.Invoke(collider[i].gameObject.name);
        }
      }

      if (!m_Hurt) {
        if (!m_Grounded && m_Rigidbody2D.velocity.y < -0.1f)
        {
          IsFallingEvent.Invoke(true);
          IsRisingEvent.Invoke(false);
          if (m_JumpDisableCollider != null)
            m_JumpDisableCollider.enabled = false;
        }
        else if (!m_Grounded && m_Rigidbody2D.velocity.y > 0.1f)
        {
          IsRisingEvent.Invoke(true);
          IsFallingEvent.Invoke(false);
        }
        else
        {
          IsFallingEvent.Invoke(false);
          IsRisingEvent.Invoke(false);
        }
      }
  }


  public void Move(float move, bool crouch, bool jump, bool hurt)
  {
    // If crouching, check to see if the character can stand up
    if (!crouch)
    {
      // If the character has a ceiling preventing them from standing up, keep them crouching
      if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
      {
        crouch = true;
      }
    }

    //only control the player if grounded or airControl is turned on.
    if ((m_Grounded || m_AirControl) && !m_Hurt)
    {

      // If crouching
      if (crouch)
      {
        if (!m_wasCrouching)
        {
          m_wasCrouching = true;
          OnCrouchEvent.Invoke(true);
        }

        // Reduce the speed by the crouchSpeed multiplier
        move *= m_CrouchSpeed;

        // Disable one of the colliders when crouching
        if (m_CrouchDisableCollider != null)
          m_CrouchDisableCollider.enabled = false;
      }
      else
      {
        // Enable the collider when not crouching
        if (m_CrouchDisableCollider != null)
          m_CrouchDisableCollider.enabled = true;

        if (m_wasCrouching)
        {
          m_wasCrouching = false;
          OnCrouchEvent.Invoke(false);
        }
      }

      // Move the character by finding the target velocity
      Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
      // And then smoothing it out and applying it to the character
      m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

      // If the input is moving the player right and the player is facing left...
      if (move > 0 && !m_FacingRight)
      {
        // ... flip the player.
        Flip();
      }
      // Otherwise if the input is moving the player left and the player is facing right...
      else if (move < 0 && m_FacingRight)
      {
        // ... flip the player.
        Flip();
      }
    }

    // If the player should jump...
    if (m_Grounded && jump)
    {
      // Add a vertical force to the player.
      m_Grounded = false;
      m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
      if (m_JumpDisableCollider != null)
      m_JumpDisableCollider.enabled = true;
    }
  }


  private void Flip()
  {
    // Switch the way the player is labelled as facing.
    m_FacingRight = !m_FacingRight;

    // Multiply the player's x local scale by -1.
    Vector3 theScale = transform.localScale;
    theScale.x *= -1;
    transform.localScale = theScale;
  }
}