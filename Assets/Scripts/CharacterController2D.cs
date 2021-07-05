using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour{
	[SerializeField] private float m_JumpForce = 550f;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

	const float k_GroundedRadius = .1f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;           // Whether or not the player is grounded.
	const float k_CeilingRadius = .1f;  // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	public Animator animator;
	private bool m_FacingRight = true;  // Determine which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;

	private bool isFloating = false;
	private float DefaultMoveSpeed;
	private float DefaultGravityScale;
	public float LevitatingGravityScale = 0.5f;
	public float LevitatingTime = 2.5f;	//	Levitate for 2.5 seconds by default
	private float LevitatingTimeCounter;
	public float SlippySpeed = 5f;
	public float ForceDown = -5f;
	public float MaxYSpeed = 15f;

	private void Awake(){
		// Get player rigidbody
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		// Get default gravity scale
		DefaultGravityScale = m_Rigidbody2D.gravityScale;
	}

	private void FixedUpdate(){
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++){
			if (colliders[i].gameObject != gameObject){
				m_Grounded = true;
			}
		}
	}

	public void Move(float move, bool crouch, bool jump){
		// Get default move speed
		DefaultMoveSpeed = move;

		// Only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl){
			// If crouching
			if (crouch){
				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Trigger crouching animation
				animator.SetBool("isCrouch", true);

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null){
					m_CrouchDisableCollider.enabled = false;
				}
			}
			else{
				// Reset move speed to default
				move = DefaultMoveSpeed;

				// Disable crouching animation
				animator.SetBool("isCrouch", false);

				// If the character has a ceiling preventing them from standing up, keep them crouching
				if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround)){
				}
				// Enable the collider when not crouching
				else if (m_CrouchDisableCollider != null){
					m_CrouchDisableCollider.enabled = true;
				}
			}

			if (m_Rigidbody2D.velocity.y > MaxYSpeed){
				// Move the character by finding the target velocity
				// Check if y velocity greater than maximum
				m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, MaxYSpeed);
            }

			// Move the character by finding the target velocity
			m_Rigidbody2D.velocity = new Vector2(move * Time.fixedDeltaTime, m_Rigidbody2D.velocity.y);

			//	Flip if input moves player right but they are facing left
			if (move > 0 && !m_FacingRight){
				Flip();
			}
			// Flip if input moves player left but they are facing right
			else if (move < 0 && m_FacingRight){
				Flip();
			}
		}
		
		// Hang time + jump timeout (otherwise player can take advantage of higher velocity the insant they begin jump)
		if (m_Grounded){
			isFloating = false;
			m_Rigidbody2D.gravityScale = DefaultGravityScale;

			LevitatingTimeCounter = LevitatingTime;

			// Disable jumping animation
			animator.SetBool("isJump", false);
		}

		//	If jumping (jump 1)
		if (jump && m_Grounded && !isFloating){
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
			m_Grounded = false;

			// Trigger jumping animation
			animator.SetBool("isJump", true);
		}

		// Toggle player levitating physics (jump 2)
		else if (jump && !m_Grounded && !isFloating){
			m_Rigidbody2D.gravityScale = LevitatingGravityScale;	//	Disable gravity on player
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0f);	//	Start levitating at position that they press space again
			isFloating = true;

			// Toggle falling animation
			animator.SetBool("isFall", true);
		}
		
		// Disable player levitating physics (jump 3)
		else if (jump || LevitatingTimeCounter <= 0 && !m_Grounded && isFloating){
			m_Rigidbody2D.gravityScale = DefaultGravityScale;   //	Re-enable gravity on player

			// Disable falling animation
			animator.SetBool("isFall", false);
		}
        else{
			// Count down time that player is levitating
			LevitatingTimeCounter -= Time.deltaTime;
        }

		if (m_Rigidbody2D.velocity.x != 0){
			// Trigger walking animation
			animator.SetBool("isWalk", true);
		}
		else{
			// Disable walking animation
			animator.SetBool("isWalk", false);
		}
	}

	private void Flip(){
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

    private void OnCollisionStay2D(Collision2D collision){
		// Force player down slippery slope
        if (collision.gameObject.tag == "Slippy"){
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, ForceDown) * SlippySpeed;
        }
    }
}