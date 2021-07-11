using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    Rigidbody2D rigid;
    GameObject leftArrow;
    GameObject rightArrow;
    GameObject fadeIn;
    GameObject fadeOut;
    bool isFadingOut;
    GameManager gameManager;
    // Input
    float inputHorizontal;
    float inputVertical;
    bool inputJump;
    // Collision
    public bool isCollideRope;
    bool isCollideLever;
    bool isHorizontalWind;
    bool isVerticalWind;
    // Walk
    public float walkSpeed;
    // Jump
    public float jumpPower;
    float jumpTimer;
    public bool isJumping;
    // Rope
    Rope rope;
    public bool isRoping;
    public float ropeSpeed;
    // Gravity
    public GameManager.GravityDirection gravityDirection;
    bool isSelectingGravity;
    bool isUsingVertical;
    Vector2 startingPos;
    public float rotateDirection;
    public bool afterRotating;
    // Elevator
    public bool inElevator;

    void Awake() {
        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        rigid = GetComponent<Rigidbody2D>();
        GameObject canvas = GameObject.FindWithTag("Canvas");
        fadeIn = canvas.transform.GetChild(0).gameObject;
        fadeOut = canvas.transform.GetChild(1).gameObject;
        leftArrow = transform.GetChild(0).gameObject;
        rightArrow = transform.GetChild(1).gameObject;
    }

    void Start() {
        fadeIn.SetActive(true);
        leftArrow.SetActive(false);
        rightArrow.SetActive(false);
        // Just Next Scene
        if (!gameManager.isDie) {
            transform.position = gameManager.nextPos;
            transform.rotation = gameManager.nextRot;
            Physics2D.gravity = gameManager.nextGravity;
            gravityDirection = gameManager.nextGravityDir;
            afterRotating = gameManager.nextAfterRotating;
            rigid.gravityScale = gameManager.nextGravityScale;
            isJumping = gameManager.nextIsJumping;
            isRoping = gameManager.nextIsRoping;
            isCollideRope = gameManager.nextIsCollideRope;
        }
        // Respawn after Dying
        else {
            int curStage = gameManager.curStage;
            int curState = gameManager.curState;
            transform.position = gameManager.respawnPos[curStage, curState];
            transform.rotation = gameManager.respawnRot[curStage, curState];
            Physics2D.gravity = gameManager.respawnGravity[curStage, curState];
            gravityDirection = gameManager.respawnGravityDir[curStage, curState];
            gameManager.isDie = false;
        }
    }

    void Update()
    {
        // Input for walking
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        // Input for jumping
        inputJump = Input.GetKeyDown(KeyCode.Space);
        // Input for hanging on the rope
        inputVertical = Input.GetAxisRaw("Vertical");

        if (!gameManager.isDie) {
            if (!isSelectingGravity && rotateDirection == 0 && !afterRotating) {
                if ((!isHorizontalWind || Physics2D.gravity.x != 0) && (!isVerticalWind || Physics2D.gravity.y != 0)) {
                    Walk();
                }
                Jump();
                Rope();
            }
            UsingLever();
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Spike" || other.gameObject.tag == "Arrow" || other.gameObject.tag == "Cannon") {
            gameManager.isDie = true;
            fadeOut.SetActive(true);
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Rope")) {
            isCollideRope = true;
            rope = other.GetComponent<Rope>();
        }
        if (other.CompareTag("Lever")) {
            isCollideLever = true;
        }
        if (other.CompareTag("RightWind") || other.CompareTag("LeftWind")) {
            isHorizontalWind = true;
        }
        if (other.CompareTag("UpWind") || other.CompareTag("DownWind")) {
            isVerticalWind = true;
        }
    }

    void OnTriggerStay2D(Collider2D other) {
        if (!isRoping) {
            if (other.CompareTag("UpWind")) {
                rigid.AddForce(Vector2.up * 50.0f, ForceMode2D.Force);
            }
            else if (other.CompareTag("DownWind")) {
                rigid.AddForce(Vector2.down * 50.0f, ForceMode2D.Force);
            }
            else if (other.CompareTag("RightWind")) {
                rigid.AddForce(Vector2.right * 50.0f, ForceMode2D.Force);
            }
            else if (other.CompareTag("LeftWind")) {
                rigid.AddForce(Vector2.left * 50.0f, ForceMode2D.Force);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Rope")) {
            isCollideRope = false;
        }
        if (other.CompareTag("Lever")) {
            isCollideLever = false;
        }
        if (other.CompareTag("RightWind") || other.CompareTag("LeftWind")) {
            isHorizontalWind = false;
        }
        if (other.CompareTag("UpWind") || other.CompareTag("DownWind")) {
            isVerticalWind = false;
        }
    }

    void UsingLever() {
        // Decide wheter to use the lever
        if (rotateDirection == 0 && isCollideLever && !isJumping && !isRoping && !afterRotating) {
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

        // Select gravity direction
        if (isSelectingGravity && inputHorizontal != 0) {
            leftArrow.SetActive(false);
            rightArrow.SetActive(false);
            rotateDirection = inputHorizontal;
            isSelectingGravity = false;
            isUsingVertical = false;
            startingPos = transform.position;
        }

        // Change gravity direction
        if (rotateDirection != 0) {
            switch (gravityDirection) {
                case GameManager.GravityDirection.left:
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 90 * rotateDirection - 90), Time.deltaTime * 5.0f);
                    if (startingPos.x - transform.position.x > 0.28f) {
                        rigid.gravityScale = 0;
                    }
                    if (transform.rotation == Quaternion.Euler(0, 0, 90 * rotateDirection - 90)) {
                        Physics2D.gravity = new Vector2(0, -9.8f * rotateDirection);
                        rigid.gravityScale = 2;
                        if (rotateDirection == 1) {
                            gravityDirection = GameManager.GravityDirection.down;
                        }
                        else {
                            gravityDirection = GameManager.GravityDirection.up;
                        }
                        rotateDirection = 0;
                        afterRotating = true;
                    }
                    break;
                case GameManager.GravityDirection.right:
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 90 * rotateDirection + 90), Time.deltaTime * 5.0f);
                    if (transform.position.x - startingPos.x > 0.28f) {
                        rigid.gravityScale = 0;
                    }
                    if (transform.rotation == Quaternion.Euler(0, 0, 90 * rotateDirection + 90)) {
                        Physics2D.gravity = new Vector2(0, 9.8f * rotateDirection);
                        rigid.gravityScale = 2;
                        if (rotateDirection == 1) {
                            gravityDirection = GameManager.GravityDirection.up;
                        }
                        else {
                            gravityDirection = GameManager.GravityDirection.down;
                        }
                        rotateDirection = 0;
                        afterRotating = true;
                    }
                    break;
                case GameManager.GravityDirection.up:
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, -90 * rotateDirection), Time.deltaTime * 5.0f);
                    if (transform.position.y - startingPos.y > 0.28f) {
                        rigid.gravityScale = 0;
                    }
                    if (transform.rotation == Quaternion.Euler(0, 0, -90 * rotateDirection)) {
                        Physics2D.gravity = new Vector2(-9.8f * rotateDirection, 0);
                        rigid.gravityScale = 2;
                        if (rotateDirection == 1) {
                            gravityDirection = GameManager.GravityDirection.left;
                        }
                        else {
                            gravityDirection = GameManager.GravityDirection.right;
                        }
                        rotateDirection = 0;
                        afterRotating = true;
                    }
                    break;
                default:
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 90 * rotateDirection), Time.deltaTime * 5.0f);
                    if (startingPos.y - transform.position.y > 0.28f) {
                        rigid.gravityScale = 0;
                    }
                    if (transform.rotation == Quaternion.Euler(0, 0, 90 * rotateDirection)) {
                        Physics2D.gravity = new Vector2(9.8f * rotateDirection, 0);
                        rigid.gravityScale = 2;
                        if (rotateDirection == 1) {
                            gravityDirection = GameManager.GravityDirection.right;
                        }
                        else {
                            gravityDirection = GameManager.GravityDirection.left;
                        }
                        rotateDirection = 0;
                        afterRotating = true;
                    }
                    break;
            }
        }

        // Finish applying the changed gravity direction
        if (afterRotating && IsGrounded()) {
            afterRotating = false;
        }
    }

    void Walk() {
        Vector2 locVel = transform.InverseTransformDirection(rigid.velocity);
        locVel.x = inputHorizontal * walkSpeed;
        rigid.velocity = transform.TransformDirection(locVel);
    }

    void Jump() {
        // Start jumping
        if (inputJump && !isJumping) {
            rigid.velocity = Vector2.zero;
            rigid.AddForce(transform.up * jumpPower, ForceMode2D.Impulse);
            isJumping = true;
        }
        // Landing on the ground -> Finish jumping
        Vector2 locVel = transform.InverseTransformDirection(rigid.velocity);
        if (locVel.y <= 0 && IsGrounded()) {
            isJumping = false;
        }
    }

    void Rope() {
        // Start to hang on the rope
        if (!isRoping) {
            if (inputVertical != 0 && isCollideRope) {
                rigid.gravityScale = 0;
                Vector2 ropePos = rope.transform.position;
                if (rope.isVertical) {
                    transform.position = new Vector2(ropePos.x - transform.up.x * 0.7f, transform.position.y);
                }
                else {
                    transform.position = new Vector2(transform.position.x, ropePos.y - transform.up.y * 0.7f);
                }
                isJumping = false;
                isRoping = true;
            }
        }
        else {
            // Moving on the rope
            if (isCollideRope && !isJumping) {
                if (rope != null) {
                    switch (gravityDirection) {
                        case GameManager.GravityDirection.left:
                            if (rope.isVertical) {
                                rigid.velocity = new Vector2(0, -inputHorizontal * ropeSpeed);
                            }
                            else {
                                // End of the rope -> Stop moving
                                if (transform.position.x >= rope.transform.position.x && inputVertical >= 0) {
                                    rigid.velocity = Vector2.zero;
                                }
                                else {
                                    rigid.velocity = new Vector2(inputVertical * ropeSpeed, 0);
                                }
                            }
                            break;
                        case GameManager.GravityDirection.right:
                            if (rope.isVertical) {
                                rigid.velocity = new Vector2(0, inputHorizontal * ropeSpeed);
                            }
                            else {
                                // End of the rope -> Stop moving
                                if (transform.position.x <= rope.endPos && inputVertical >= 0) {
                                    rigid.velocity = Vector2.zero;
                                }
                                else {
                                    rigid.velocity = new Vector2(-inputVertical * ropeSpeed, 0);
                                }
                            }
                            break;
                        case GameManager.GravityDirection.up:
                            if (rope.isVertical) {
                                // End of the rope -> Stop moving
                                if (transform.position.y <= rope.endPos && inputVertical >= 0) {
                                    rigid.velocity = Vector2.zero;
                                }
                                else {
                                    rigid.velocity = new Vector2(0, -inputVertical * ropeSpeed);
                                }
                            }
                            else {
                                rigid.velocity = new Vector2(-inputHorizontal * ropeSpeed, 0);
                            }
                            break;
                        case GameManager.GravityDirection.down:
                            if (rope.isVertical) {
                                // End of the rope -> Stop moving
                                if (transform.position.y >= rope.transform.position.y && inputVertical >= 0) {
                                    rigid.velocity = Vector2.zero;
                                }
                                else {
                                    rigid.velocity = new Vector2(0, inputVertical * ropeSpeed);
                                }
                            }
                            else {
                                rigid.velocity = new Vector2(inputHorizontal * ropeSpeed, 0);
                            }
                            break;
                    }
                }
            }
            // Finish hanging on the rope
            else {
                rigid.gravityScale = 2;
                isRoping = false;
            }
        }
    }
/*
    void OnDrawGizmos() {
        RaycastHit2D rayHit;
        if (Physics2D.gravity.x == 0) {
            rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.9f, 0.1f), 0, -transform.up, 0.8f, 1 << 3 | 1 << 6 | 1 << 8 | 1 << 12 | 1 << 15 | 1 << 16 | 1 << 17);
            Gizmos.DrawRay(transform.position, -transform.up * rayHit.distance);
            Gizmos.DrawWireCube(transform.position - transform.up * rayHit.distance, new Vector2(0.9f, 0.1f));
        }
        else {
            rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.1f, 0.9f), 0, -transform.up, 0.8f, 1 << 3 | 1 << 6 | 1 << 8 | 1 << 12 | 1 << 15 | 1 << 16 | 1 << 17);
            Gizmos.DrawRay(transform.position, -transform.up * rayHit.distance);
            Gizmos.DrawWireCube(transform.position - transform.up * rayHit.distance, new Vector2(0.1f, 0.9f));
        }
        
    }
*/
    bool IsGrounded() {
        // Jump
        RaycastHit2D rayHit;
        if (Physics2D.gravity.x == 0) {
            rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.9f, 0.1f), 0, -transform.up, 0.8f
            , 1 << 3 | 1 << 6 | 1 << 8 | 1 << 12 | 1 << 15 | 1 << 16); // Platform, ArrowHome, CannonHome, WindHome, Stone, ElevatorFloor
        }
        else {
            rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.1f, 0.9f), 0, -transform.up, 0.8f
            , 1 << 3 | 1 << 6 | 1 << 8 | 1 << 12 | 1 << 15 | 1 << 16);
        }

        // Elevator
        RaycastHit2D rayHitElevator;
        if (Physics2D.gravity.x == 0) {
            rayHitElevator = Physics2D.BoxCast(transform.position, new Vector2(0.9f, 0.1f), 0, -transform.up, 0.8f, 1 << 16);
        }
        else {
            rayHitElevator = Physics2D.BoxCast(transform.position, new Vector2(0.1f, 0.9f), 0, -transform.up, 0.8f, 1 << 16);
        }
        if (rayHitElevator.collider != null) {
            transform.parent = rayHitElevator.collider.transform;
            inElevator = true;
        }
        else {
            transform.parent = null;
            inElevator = false;
        }
        
        // KeyBox
        RaycastHit2D rayHitKeyBox;
        if (Physics2D.gravity.x == 0) {
            rayHitKeyBox = Physics2D.BoxCast(transform.position, new Vector2(0.9f, 0.1f), 0, -transform.up, 0.8f, 1 << 17);
        }
        else {
            rayHitKeyBox = Physics2D.BoxCast(transform.position, new Vector2(0.1f, 0.9f), 0, -transform.up, 0.8f, 1 << 17);
        }
        if (rayHitKeyBox.collider != null) {
            Transform keyBox = rayHitKeyBox.collider.transform.parent;
            if (keyBox.childCount == 5) {
                if (keyBox.GetChild(2).CompareTag("Key1")) {
                    gameManager.isOpenKeyBox1 = true;
                }
                else if (keyBox.GetChild(2).CompareTag("Key2")) {
                    gameManager.isOpenKeyBox2 = true;
                }
                // activate key
                keyBox.GetChild(2).gameObject.SetActive(true);
            }
        }
        
        return rayHit.collider != null;
    }
}