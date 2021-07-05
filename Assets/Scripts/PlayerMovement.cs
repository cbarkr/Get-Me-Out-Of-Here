using UnityEngine;

public class PlayerMovement : MonoBehaviour{
    public CharacterController2D controller;
    public float horizontalMove = 0f;
    public float runSpeed = 40f;
    bool isJumping = false;
    bool isCrouching = false;

    void Update(){
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if (Input.GetButtonDown("Jump")){
            isJumping = true;
        }
        else{
            isJumping = false;
        }

        if (Input.GetButton("Crouch")){
            isCrouching = true;
        }
        else{
            isCrouching = false;
        }
        controller.Move(horizontalMove, isCrouching, isJumping);
    }
}
