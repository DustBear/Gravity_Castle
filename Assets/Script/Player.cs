using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    Rigidbody2D rigid;
    public GameObject leftArrow;
    public GameObject rightArrow;
    public GameObject stone;
    public GameObject fadeIn;
    public GameObject fadeOut;
    FadeIn fadeInScript;
    FadeOut fadeOutScript;
    // input variable
    float inputHorizontal;
    float inputVertical;
    bool inputVerticalRope;
    bool inputJump;
    // walk
    public float walkSpeed;
    // jump
    public float jumpPower;
    float jumpTimer;
    bool isJumping;
    // rope
    Rope rope;
    bool isRoping;
    public float ropeSpeed;
    // collision
    bool isCollideRope;
    bool isCollideLever;
    bool isCollideKey1;
    bool isCollideKey2;
    bool isDie;
    // gravity
    public enum GravityDirection {left, down, right, up};
    public GravityDirection gravityDirection;
    bool isSelectingGravity;
    bool isUsingVertical;
    public float rotateDirection;
    bool afterRotating;
    // key
    //BoxBody boxBody;

    public GameObject elevator;
    Elevator elevatorScript;

    SpriteRenderer sr;
    BoxCollider2D bc;

    void Awake() {
        Init();
        fadeIn.SetActive(true);
        fadeOut.SetActive(false);
        fadeInScript = fadeIn.GetComponent<FadeIn>();
        fadeOutScript = fadeOut.GetComponent<FadeOut>();
        elevatorScript = elevator.GetComponent<Elevator>();
    }

    void Start() {
        int sceneIdx = SceneManager.GetActiveScene().buildIndex;
        if (sceneIdx == 1) {
            StartCoroutine(EnableStone());
        }
    }

    void Update()
    {
        // input for walking
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        // input for jumping
        inputJump = Input.GetKeyDown(KeyCode.Space);
        // input for moving on rope
        inputVertical = Input.GetAxisRaw("Vertical");
        // input for hanging on rope
        inputVerticalRope = Input.GetKeyDown(KeyCode.UpArrow);

        if (elevatorScript.isStart) {
            transform.parent = elevator.transform;
        }
        else {
            transform.parent = null;
        }

        if (isDie) {
            Die();
        }
        else {
            if (!isSelectingGravity && rotateDirection == 0 && !afterRotating) {
                Walk();
                Jump();
                Rope();
            }
            NextScene();
        }
    }

    void FixedUpdate() {
        RotateGravity();
        //GetKey();
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Spike" || other.gameObject.tag == "Arrow" || other.gameObject.tag == "Cannon") {
            isDie = true;
        }
        if (other.gameObject.tag == "Door1" && isCollideKey1 || other.gameObject.tag == "Door2" && isCollideKey2) {
            bc = other.gameObject.GetComponent<BoxCollider2D>();
            sr = other.gameObject.GetComponent<SpriteRenderer>();
            StartCoroutine("FadeOut");
        }
    }

    void OnTriggerStay2D(Collider2D other) {
        if (other.CompareTag("Rope")) {
            isCollideRope = true;
            rope = other.GetComponent<Rope>();
        }
        if (other.CompareTag("Lever")) {
            isCollideLever = true;
        }
        if (other.CompareTag("Key1")) {
            isCollideKey1 = true;
        }
        if (other.CompareTag("Key2")) {
            isCollideKey2 = true;
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

    void Init() {
        rigid = GetComponent<Rigidbody2D>();
        stone.SetActive(false);
        fadeOut.SetActive(false);
        fadeIn.SetActive(true);
        leftArrow.SetActive(false);
        rightArrow.SetActive(false);
        transform.position = new Vector3(-110, -11, 0);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        Physics2D.gravity = new Vector2(0, -9.8f);
        gravityDirection = GravityDirection.down;
        afterRotating = false;
        isCollideRope = false;
        isCollideLever = false;
        isCollideKey1 = false;
        isCollideKey2 = false;
        isDie = false;
        isSelectingGravity = false;
        isUsingVertical = false;
    }
    /*
    void GetKey() {
        if (isCollideKey && !isJumping && !afterRotating) {
            boxBody.isGetKey = true;
        }
        else {
            boxBody.isGetKey = false;
        }
    }
*/
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
            rigid.gravityScale = 0;
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
                        rigid.gravityScale = 2;
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
                        rigid.gravityScale = 2;
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
                        rigid.gravityScale = 2;
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
                        rigid.gravityScale = 2;
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
        Vector2 locVel = transform.InverseTransformDirection(rigid.velocity);
        locVel.x  = inputHorizontal * walkSpeed;
        rigid.velocity = transform.TransformDirection(locVel);
    }

    void Jump() {
        // start jumping
        if (inputJump && !isJumping) {
            rigid.velocity = Vector2.zero;
            rigid.AddForce(transform.up * jumpPower, ForceMode2D.Impulse);
            isJumping = true;
        }
        // landing on the ground -> finish jumping
        Vector2 locVel = transform.InverseTransformDirection(rigid.velocity);
        if (locVel.y <= 0 && IsGrounded()) {
            isJumping = false;
        }
    }

    void Rope() {
        if (!isRoping) {
            if (inputVerticalRope && isCollideRope) {
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
            // hanging on the rope
            if (isCollideRope && !isJumping) {
                switch (gravityDirection) {
                    case GravityDirection.left:
                        if (rope.isVertical) {
                            rigid.velocity = new Vector2(0, -inputHorizontal * ropeSpeed);
                        }
                        else {
                            // end of rope -> stop moving
                            if (transform.position.x >= rope.transform.position.x && inputVertical >= 0) {
                                rigid.velocity = Vector2.zero;
                            }
                            else {
                                rigid.velocity = new Vector2(inputVertical * ropeSpeed, 0);
                            }
                        }
                        break;
                    case GravityDirection.right:
                        if (rope.isVertical) {
                            rigid.velocity = new Vector2(0, inputHorizontal * ropeSpeed);
                        }
                        else {
                            // end of rope -> stop moving
                            if (transform.position.x <= rope.endPos && inputVertical >= 0) {
                                rigid.velocity = Vector2.zero;
                            }
                            else {
                                rigid.velocity = new Vector2(-inputVertical * ropeSpeed, 0);
                            }
                        }
                        break;
                    case GravityDirection.up:
                        if (rope.isVertical) {
                            // end of rope -> stop moving
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
                    case GravityDirection.down:
                        if (rope.isVertical) {
                            // end of rope -> stop moving
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
            // finish hanging on the rope
            else {
                rigid.gravityScale = 2;
                isRoping = false;
            }
        }
    }

    void Die() {
        fadeIn.SetActive(false);
        fadeOut.SetActive(true);
        if (fadeOutScript.alpha >= 1) {
            fadeOut.SetActive(false);
            fadeIn.SetActive(true);
            Init();
        }
    }

    bool IsGrounded() {
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, -transform.up, 0.7f, 1 << 3 | 1 << 6 | 1 << 8 | 1 << 15);
        return rayHit.collider != null;
    }

    void NextScene() {
        int sceneIdx = SceneManager.GetActiveScene().buildIndex;
        if (transform.position.y > 9 && sceneIdx == 0) {
            SceneManager.LoadScene(1);
        } 
        else if (transform.position.y > 11 && sceneIdx == 1) {
            SceneManager.LoadScene(2);
        }
    }

    IEnumerator EnableStone() {
        while (transform.position.x > -150) {
            yield return null;
        }
        stone.SetActive(true);
    }

    IEnumerator FadeOut() {
        for (int i = 10; i >= 0; i--) {
            Color c = sr.material.color;
            c.a = i / 10.0f;
            sr.material.color = c;
            yield return new WaitForSeconds(0.1f);
        }
        bc.isTrigger = true;
    }
}