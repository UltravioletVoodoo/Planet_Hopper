using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovementScript : MonoBehaviour {
    public float movementSpeed, gravityPower;
    public float jumpForce = 20f;
    public Rigidbody2D rb;

    float mx;
    Vector2 movement = new Vector2(0, 0);

    private void Update() {
        mx = Input.GetAxisRaw("Horizontal");

        processGravity();
        processRotation();
        processMovement();
        if (Input.GetKeyDown(KeyCode.Space)) {
            jump();
        }

        rb.velocity = movement + rb.velocity;
    }

    private float calculateRotation() {
        return Mathf.Rad2Deg * Mathf.Atan(transform.position.y / transform.position.x);
    }

    private void processRotation() {
        float newAngle = calculateRotation();
        transform.rotation = Quaternion.Euler(0, 0, newAngle);
    }

    private void processGravity() {
        Vector2 planetCenter = new Vector2(0, 0);
        Vector2 playerPosition = new Vector2(transform.position.x, transform.position.y);
        Vector2 playerGravity = (planetCenter - playerPosition);
        playerGravity.Normalize();
        playerGravity = playerGravity * gravityPower;
        Debug.DrawLine(getPlayerPosition(), planetCenter, Color.yellow);
        movement = playerGravity;
    }

    private Vector2 getGravityVector() {
        Vector2 planetCenter = new Vector2(0, 0);
        Vector2 playerPosition = getPlayerPosition();
        Vector2 playerGravity = (planetCenter - playerPosition);
        playerGravity.Normalize();
        playerGravity = playerGravity * gravityPower;
        return playerGravity;
    }

    private Vector2 getPlayerPosition() {
        return new Vector2(transform.position.x, transform.position.y);
    }

    private void processMovement() {
        if (mx == 0) return;
        Vector2 movementVector;
        movementVector = Perpendicular(getGravityVector());
        movementVector.Normalize();
        movementVector = movementVector * -movementSpeed;
        if (mx > 0) {
            movementVector = -movementVector;
        }
        Debug.DrawLine(getPlayerPosition(),getPlayerPosition() + (movementVector * 100), Color.red);
        movement = movementVector;
    }

    private Vector2 Perpendicular(Vector2 vector2)
    {
        return new Vector2(-vector2.y, vector2.x);
    }

    private void jump() {
        Vector2 jumpVector = -1 * getGravityVector();
        jumpVector.Normalize();
        jumpVector = jumpVector * jumpForce;
        
        Debug.DrawLine(getPlayerPosition(), getPlayerPosition() + jumpVector, Color.green, 0.05f);
        movement = jumpVector;
    }
}
