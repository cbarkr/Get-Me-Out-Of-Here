using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour{
    public Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // Determine which way the vehicle is currently facing.
    public Vector2 Movement;
    public float SlowVehicleSpeed = 2f;
    public float FastVehicleSpeed = 5f;
    public float FasterVehicleSpeed = 8f;
    public float ImpactForce = 100f;
    public float ImpactForceDown = -50f;

    void Start(){
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update(){
        // Vehicle is on the right of the map
        if (m_Rigidbody2D.position.x > 200){
            if (gameObject.tag == "FasterVehicle"){
                Movement = new Vector2(-FasterVehicleSpeed, 0f);
                MoveVehicle(Movement, FasterVehicleSpeed);
            }
            else if (gameObject.tag == "FastVehicle"){
                Movement = new Vector2(-FastVehicleSpeed, 0f);
                MoveVehicle(Movement, FastVehicleSpeed);
            }
            else if (gameObject.tag == "SlowVehicle"){
                Movement = new Vector2(-SlowVehicleSpeed, 0f);
                MoveVehicle(Movement, SlowVehicleSpeed);
            }
        }

        // Vehicle is on the left of the map
        else if (m_Rigidbody2D.position.x < -200){
            if (gameObject.tag == "FasterVehicle"){
                Movement = new Vector2(FasterVehicleSpeed, 0f);
                MoveVehicle(Movement, FasterVehicleSpeed);
            }
            else if (gameObject.tag == "FastVehicle"){
                Movement = new Vector2(FastVehicleSpeed, 0f);
                MoveVehicle(Movement, FastVehicleSpeed);
            }
            else if (gameObject.tag == "SlowVehicle"){
                Movement = new Vector2(SlowVehicleSpeed, 0f);
                MoveVehicle(Movement, SlowVehicleSpeed);
            }
        }

        //	Flip if input moves player right but they are facing left
        if (m_Rigidbody2D.velocity.x < 0 && !m_FacingRight)
        {
            Flip();
        }
        // Flip if input moves player left but they are facing right
        else if (m_Rigidbody2D.velocity.x > 0 && m_FacingRight)
        {
            Flip();
        }
    }

    void MoveVehicle(Vector2 VehicleDirection, float VehicleSpeed){
        m_Rigidbody2D.velocity = VehicleDirection * VehicleSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision){
        // Knock player away when hit by vehicle
        Rigidbody2D m_PlayerRigidbody2D = collision.collider.GetComponent<Rigidbody2D>();
        m_PlayerRigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x * ImpactForce, ImpactForceDown);
    }

    private void Flip(){
        // Switch the way the vehicle is labelled as facing
        m_FacingRight = !m_FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
