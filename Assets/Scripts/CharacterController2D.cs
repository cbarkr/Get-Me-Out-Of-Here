using UnityEngine;

public class CharacterController2D : MonoBehaviour{
	[SerializeField] private float m_JumpForce = 550f;							// Force added when the player jumps
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// Mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// Position marking where to check if the player is grounded
	[SerializeField] private Transform m_CeilingCheck;							// Position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;                // Collider that is disabled when crouching

	const float k_GroundedRadius = .1f;											// Radius of the overlap circle to determine if grounded
	private bool m_Grounded;													// Whether or not the player is grounded
	const float k_CeilingRadius = .1f;											// Radius of the overlap circle to determine if the player can stand up
	private bool m_FacingRight = true;											// Determine which way the player is currently facing

	private float DefaultMoveSpeed;
	private float DefaultGravityScale;
	private bool isLevitating = false;                                          // Determine if player is currently levitating
	public float LevitatingGravityScale = 0.5f;									// Gravity scale enabled when levitating
	public float LevitatingTime = 2.5f;											// Time in seconds that player can levitate
	private float LevitatingTimeCounter;
	public float SlippySpeed = 5f;												// Speed on angled tiles
	public float ForceDown = -5f;												// Force applied down on player when hit by vehicle
	public float MaxYSpeed = 15f;												// Maximum Y speed applied to player for bounce pads

	private Rigidbody2D m_Rigidbody2D;
	public Animator animator;

	private void Awake(){
		// Get player rigidbody
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		// Get default gravity scale
		DefaultGravityScale = m_Rigidbody2D.gravityScale;
	}

	private void FixedUpdate(){
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

		// Control the player if on ground or in air if airControl is turned on
		if (m_Grounded || m_AirControl){
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

				// If a ceiling prevents character from standing, keep them crouching
				if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround)){
					crouch = true;
				}
				// Enable collider when not crouching
				else if (m_CrouchDisableCollider != null){
					m_CrouchDisableCollider.enabled = true;
				}
			}

			if (m_Rigidbody2D.velocity.y > MaxYSpeed){
				// Move player by finding target velocity
					// Replace Y speed if player exceeds the maximum
				m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, MaxYSpeed);
            }

			// Move player by finding target velocity
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
		
		if (m_Grounded){
			// Player not levitating if they are grounded
			isLevitating = false;
			
			// Get default gravity scale 
			m_Rigidbody2D.gravityScale = DefaultGravityScale;

			// Set levitating timer to default
			LevitatingTimeCounter = LevitatingTime;

			// Disable jumping animation
			animator.SetBool("isJump", false);
		}

		//	If jumping (jump 1)
		if (jump && m_Grounded && !isLevitating){
			// Add jump force in Y
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
			m_Grounded = false;

			// Trigger jumping animation
			animator.SetBool("isJump", true);
		}

		// Toggle player levitating physics (jump 2)
		else if (jump && !m_Grounded && !isLevitating){
			// Lower gravity while player is levitating
			m_Rigidbody2D.gravityScale = LevitatingGravityScale;
			
			// Levitate at the position that they press space
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0f);
			isLevitating = true;

			// Toggle falling animation
			animator.SetBool("isFall", true);
		}
		
		// Disable player levitating physics (jump 3)
		else if (jump || LevitatingTimeCounter <= 0 && !m_Grounded && isLevitating){
			// Enable normal gravity when player stops levitating
			m_Rigidbody2D.gravityScale = DefaultGravityScale;

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