using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    Rigidbody2D rigid;
    public GameObject leftArrow;
    public GameObject rightArrow;
    // input variable
    float inputHorizontal;
    float inputVertical;
    bool inputJump;
    // walk
    public float walkSpeed;
    // jump
    public float jumpPower;
    float jumpTimer;
    bool isJumping;
    // rope
    Rope rope;
    enum RopeState {normal, moveToRope, hangOnRope};
    RopeState ropeState;
    public float ropeSpeed;
    // collision
    Vector3 ropePos;
    bool isCollideRope;
    bool isCollideLever;
    bool isCollideEnemy;
    // gravity
    public enum GravityDirection {left, down, right, up};
    public GravityDirection gravityDirection;
    bool isSelectingGravity;
    bool isUsingVertical;
    public float rotateDirection;
    bool afterRotating;

    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        leftArrow.SetActive(false);
        rightArrow.SetActive(false);
        transform.position = new Vector3(-22, -12, 0);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        Physics2D.gravity = new Vector2(0, -9.8f);
        gravityDirection = GravityDirection.down;
        afterRotating = false;
        ropeState = RopeState.normal;
        isCollideRope = false;
        isCollideLever = false;
        isCollideEnemy = false;
        isSelectingGravity = false;;
        isUsingVertical = false;
    }

    void Update()
    {
        // input for walk
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        // input for rope
        inputVertical = Input.GetAxisRaw("Vertical");
        // input for jump
        if (Input.GetKey(KeyCode.Space)) {
            inputJump = true;
        }
        else {
            inputJump = false;
        }
        
        if (transform.position.y > 38) {
            int sceneIdx = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(sceneIdx + 1);
        }
    }

    void FixedUpdate() {
        if (!isSelectingGravity && rotateDirection == 0 && !afterRotating) {
            Walk();
            Jump();
            Rope();
        }
        RotateGravity();
        Die();
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Enemy") {
            isCollideEnemy = true;
        }
    }

    void OnTriggerStay2D(Collider2D other) {
        if (other.CompareTag("Rope")) {
            isCollideRope = true;
            ropePos = other.transform.position;
            rope = other.GetComponent<Rope>();
        }
        if (other.CompareTag("Lever")) {
            isCollideLever = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Rope")) {
            isCollideRope = false;
        }
        if (other.CompareTag("Lever")) {
            isCollideLever = false;
        }
    }

    void RotateGravity() {
        if (rotateDirection == 0 && isCollideLever && !isJumping && rigid.velocity == Vector2.zero && !afterRotating) {
            if (inputVertical == 1) {
                if (!isUsingVertical) {
                    isSelectingGravity = !isSelectingGravity;
                    leftArrow.SetActive(isSelectingGravity);
                    rightArrow.SetActive(isSelectingGravity);
                    isUsingVertical = true;
                }
            }
            else {
                isUsingVertical = false;
            }
        }

        if (isSelectingGravity && inputHorizontal != 0) {
            leftArrow.SetActive(false);
            rightArrow.SetActive(false);
            rotateDirection = inputHorizontal;
            isSelectingGravity = false;
            isUsingVertical = false;
        }

        if (rotateDirection != 0) {
            switch (gravityDirection) {
                case GravityDirection.left:
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 90 * rotateDirection - 90), Time.deltaTime * 5.0f);
                    if (transform.rotation == Quaternion.Euler(0, 0, 90 * rotateDirection - 90)) {
                        Physics2D.gravity = new Vector2(0, -9.8f * rotateDirection);
                        if (rotateDirection == 1) {
                            gravityDirection = GravityDirection.down;
                        }
                        else {
                            gravityDirection = GravityDirection.up;
                        }
                        rotateDirection = 0;
                        afterRotating = true;
                    }
                    break;
                case GravityDirection.right:
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 90 * rotateDirection + 90), Time.deltaTime * 5.0f);
                    if (transform.rotation == Quaternion.Euler(0, 0, 90 * rotateDirection + 90)) {
                        Physics2D.gravity = new Vector2(0, 9.8f * rotateDirection);
                        if (rotateDirection == 1) {
                            gravityDirection = GravityDirection.up;
                        }
                        else {
                            gravityDirection = GravityDirection.down;
                        }
                        rotateDirection = 0;
                        afterRotating = true;
                    }
                    break;
                case GravityDirection.up:
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, -90 * rotateDirection), Time.deltaTime * 5.0f);
                    if (transform.rotation == Quaternion.Euler(0, 0, -90 * rotateDirection)) {
                        Physics2D.gravity = new Vector2(-9.8f * rotateDirection, 0);
                        if (rotateDirection == 1) {
                            gravityDirection = GravityDirection.left;
                        }
                        else {
                            gravityDirection = GravityDirection.right;
                        }
                        rotateDirection = 0;
                        afterRotating = true;
                    }
                    break;
                default:
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 90 * rotateDirection), Time.deltaTime * 5.0f);
                    if (transform.rotation == Quaternion.Euler(0, 0, 90 * rotateDirection)) {
                        Physics2D.gravity = new Vector2(9.8f * rotateDirection, 0);
                        if (rotateDirection == 1) {
                            gravityDirection = GravityDirection.right;
                        }
                        else {
                            gravityDirection = GravityDirection.left;
                        }
                        rotateDirection = 0;
                        afterRotating = true;
                    }
                    break;
            }
        }
        if (afterRotating) {
            if (IsGrounded()) {
                afterRotating = false;
            }
        }
    }

    void Walk() {
        switch (gravityDirection) {
            case GravityDirection.left:
                rigid.velocity = new Vector2(rigid.velocity.x, -inputHorizontal * walkSpeed);
                return;
            case GravityDirection.right:
                rigid.velocity = new Vector2(rigid.velocity.x, inputHorizontal * walkSpeed);
                return;
            case GravityDirection.up:
                rigid.velocity = new Vector2(-inputHorizontal * walkSpeed, rigid.velocity.y);
                return;
            default:
                rigid.velocity = new Vector2(inputHorizontal * walkSpeed, rigid.velocity.y);
                return;
        }
    }

    void Jump() {
        switch (gravityDirection) {
            case GravityDirection.left:
                if (inputJump && !isJumping) {
                    // start jumping
                    rigid.AddForce(Vector2.right * jumpPower, ForceMode2D.Impulse);
                    isJumping = true;
                }
                if (rigid.velocity.x <= 0 && IsGrounded()) {
                    // landing on the ground -> finish jumping
                    isJumping = false;
                }
                return;
            case GravityDirection.right:
                if (inputJump && !isJumping) {
                    // start jumping
                    rigid.AddForce(Vector2.left * jumpPower, ForceMode2D.Impulse);
                    isJumping = true;
                }
                if (rigid.velocity.x >= 0 && IsGrounded()) {
                    // landing on the ground -> finish jumping
                    isJumping = false;
                }
                return;
            case GravityDirection.up:
                if (inputJump && !isJumping) {
                    // start jumping
                    rigid.AddForce(Vector2.down * jumpPower, ForceMode2D.Impulse);
                    isJumping = true;
                }
                if (rigid.velocity.y >= 0 && IsGrounded()) {
                    // landing on the ground -> finish jumping
                    isJumping = false;
                }
                return;
            default:
                if (inputJump && !isJumping) {
                    // start jumping
                    rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
                    isJumping = true;
                }
                if (rigid.velocity.y <= 0 && IsGrounded()) {
                    // landing on the ground -> finish jumping
                    isJumping = false;
                }
                return;
        }
    }

    void Rope() {
        switch (ropeState) {
            case RopeState.moveToRope:
                if (rope.isVertical) {
                    switch (gravityDirection) {
                        case GravityDirection.left:
                            transform.position = new Vector2(ropePos.x - 0.7f, transform.position.y);
                            break;
                        case GravityDirection.right:
                            transform.position = new Vector2(ropePos.x + 0.7f, transform.position.y);
                            break;
                        default:
                            transform.position = new Vector2(ropePos.x, transform.position.y);
                            break;
                    }
                }
                else {
                    switch (gravityDirection) {
                        case GravityDirection.down:
                            transform.position = new Vector2(transform.position.x, ropePos.y - 0.7f);
                            break;
                        case GravityDirection.up:
                            transform.position = new Vector2(transform.position.x, ropePos.y + 0.7f);
                            break;
                        default:
                            transform.position = new Vector2(transform.position.x, ropePos.y);
                            break;
                    }
                }
                isJumping = false;
                ropeState = RopeState.hangOnRope;
                return;
            case RopeState.hangOnRope:
                // finish hanging on the rope
                if (!isCollideRope || isJumping || IsGrounded()) {
                    rigid.gravityScale = 2;
                    ropeState = RopeState.normal;
                }
                // move on the rope
                else if (isCollideRope) {
                    switch (gravityDirection) {
                        case GravityDirection.left:
                            if (rope.isVertical) {
                                rigid.velocity = new Vector2(0, -inputHorizontal * ropeSpeed);
                            }
                            else {
                                rigid.velocity = new Vector2(inputVertical * ropeSpeed, 0);
                            }
                            break;
                        case GravityDirection.right:
                            if (rope.isVertical) {
                                rigid.velocity = new Vector2(0, inputHorizontal * ropeSpeed);
                            }
                            else {
                                rigid.velocity = new Vector2(-inputVertical * ropeSpeed, 0);
                            }
                            break;
                        case GravityDirection.up:
                            if (rope.isVertical) {
                                rigid.velocity = new Vector2(0, -inputVertical * ropeSpeed);
                            }
                            else {
                                rigid.velocity = new Vector2(-inputHorizontal * ropeSpeed, 0);
                            }
                            break;
                        default:
                            if (rope.isVertical) {
                                rigid.velocity = new Vector2(0, inputVertical * ropeSpeed);
                            }
                            else {
                                rigid.velocity = new Vector2(inputHorizontal * ropeSpeed, 0);
                            }
                            break;
                    }
                }
                return;
            default:
                if (inputVertical != 0 && isCollideRope) {
                    // start to access rope
                    ropeState = RopeState.moveToRope;
                    rigid.gravityScale = 0;
                }
                return;
        }
    }

    void Die() {
        if (isCollideEnemy) {
            int sceneIdx = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(sceneIdx);
        }
    }

    bool IsGrounded() {
        RaycastHit2D rayHit;
        switch (gravityDirection) {
            case (GravityDirection.left):
                rayHit = Physics2D.Raycast(rigid.position, Vector3.left, 0.6f, 1 << 3);
                break;
            case (GravityDirection.right):
                rayHit = Physics2D.Raycast(rigid.position, Vector3.right, 0.6f, 1 << 3);
                break;
            case (GravityDirection.up):
                rayHit = Physics2D.Raycast(rigid.position, Vector3.up, 0.6f, 1 << 3);
                break;
            default:
                rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 0.6f, 1 << 3);
                break;
        }
        return rayHit.collider != null;
    }
}