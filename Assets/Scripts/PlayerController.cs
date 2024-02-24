using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Swipe { None, Up, Down, Left, Right };

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;

    public AudioSource hitSound;
    public AudioSource boostSound;

    private float movX;
    private float movY;
    private Vector3 prevRot;
    private Vector3 currRot;
    private Vector3 targetRot;

    private bool touchingGround;
    private bool canTurn;
    private bool isDead;

    public Animator anim;
    public LayerMask groundLayer;
    public EnemyController enemy;

    private float acc;
    public float boost;
    public float forwardSpeed;
    public float horizontalSpeed;
    public float maxSpeed;
    public float jumpForce;
    public float groundCheckRadius;

    public float minSwipeLength = 5f;
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    Vector2 firstClickPos;
    Vector2 secondClickPos;

    public static Swipe swipeDirection;

    public int boostDelay;
    private Coroutine boostCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        int c = PlayerPrefs.GetInt("character", 0);

        canTurn = false;
        currRot = transform.eulerAngles;
        isDead = false;
        acc = forwardSpeed;
        boostCoroutine = null;
    }

    // Update is called once per frame
    void Update()
    {
        //input
        if (Application.platform == RuntimePlatform.Android)
        {
            movX = Input.acceleration.x * 2;
        }
        else
        {
            movX = Input.GetAxis("Horizontal");
        }

        movY = Input.GetAxis("Vertical");

        touchingGround = Physics.CheckSphere(transform.position, groundCheckRadius, groundLayer);

        if (!isDead)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SpeedUp();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SlowDown();
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                Turn(1);
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                Turn(-1);
            }
            else if (Input.GetKeyUp(KeyCode.E) || Input.GetKeyUp(KeyCode.Q))
            {
                //targetWheelRot = 0;
            }

            DetectSwipe();
        }

        if (transform.position.y <= -10)
        {
            Restart();
        }
    }

    void FixedUpdate()
    {
        if (!isDead)
        {
            if (Mathf.Abs(movX) > 0)
            {
                transform.position += transform.right * movX * horizontalSpeed * Time.deltaTime;
            }

            //forward movement
            if (rb.velocity.magnitude < maxSpeed)
            {
                transform.position += transform.forward * acc * Time.deltaTime;
            }

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(currRot), Time.deltaTime * 7);

            GameManager.instance.UpdateScore(Time.deltaTime);
        }  
    }

    public void DetectSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            firstClickPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        else
        {
            swipeDirection = Swipe.None;
        }
        if (Input.GetMouseButtonUp(0))
        {
            secondClickPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            currentSwipe = new Vector3(secondClickPos.x - firstClickPos.x, secondClickPos.y - firstClickPos.y);

            // Make sure it was a legit swipe, not a tap
            if (currentSwipe.magnitude < minSwipeLength)
            {
                swipeDirection = Swipe.None;
                return;
            }

            currentSwipe.Normalize();

            //Swipe directional check
            // Swipe up
            if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
            {
                swipeDirection = Swipe.Up;
                Jump();
            }
            // Swipe down
            else if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
            {
                swipeDirection = Swipe.Down;
            }
            // Swipe left
            else if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
            {
                swipeDirection = Swipe.Left;
                Turn(-1);
            }
            // Swipe right
            else if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
            {
                swipeDirection = Swipe.Right;
                Turn(1);
            }
        }
    }

    void Jump()
    {
        if (touchingGround)
        {
            anim.SetTrigger("jump");
            //rb.AddForce(0, jumpForce, 0, ForceMode.Impulse);
            rb.velocity = Vector3.up * jumpForce;
            enemy.Jump(rb.velocity.magnitude);
        }       
    }

    void Turn(int x)
    {
        if (!canTurn)
            return;

        StartCoroutine(CanTurn());

        prevRot = currRot;
        //right
        if (x == 1)
        {
            //transform.forward = new Vector3(transform.eulerAngles.x + 90, 0, 0);
            targetRot = currRot + new Vector3(0, 90, 0);
        }
        //left
        else if (x == -1)
        {
            //transform.forward = new Vector3(transform.eulerAngles.x - 90, 0, 0);
            targetRot = currRot + new Vector3(0, -90, 0);
        }
        currRot = targetRot;
    }

    IEnumerator CanTurn()
    {
        //float ogAcc = acc;

        canTurn = false;
        //acc = acc/2;

        yield return new WaitForSeconds(0.3f);

        canTurn = true;
        //acc = ogAcc;
    }

    void SpeedUp()
    {
        acc = boost;
        anim.speed = 1.2f;
    }

    void SlowDown()
    {
        acc = forwardSpeed;
        anim.speed = 1;
    }

    IEnumerator Boost()
    {
        SpeedUp();
        yield return new WaitForSeconds(boostDelay);
        SlowDown();
        boostCoroutine = null;
    }

    public void Die()
    {
        isDead = true;
        anim.SetTrigger("die");
        rb.isKinematic = true;
        col.enabled = false;
    }

    void Restart()
    {
        GameManager.instance.RestartScene();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            hitSound.Play();
            GameManager.instance.LoseLife();
        }
        else if (other.CompareTag("Turn"))
        {
            canTurn = true;
        }
        else if (other.CompareTag("Coin"))
        {
            GameManager.instance.CollectCoin(other.transform.position);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Boost"))
        {
            if (boostCoroutine == null)
            {
                boostCoroutine = StartCoroutine(Boost());
            }
            else
            {
                StopCoroutine(boostCoroutine);
                SlowDown();
                boostCoroutine = StartCoroutine(Boost());
            }
            boostSound.Play();
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Turn"))
        {
            canTurn = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            hitSound.Play();
            Die();
            GameManager.instance.Die();
        }
    }
}
