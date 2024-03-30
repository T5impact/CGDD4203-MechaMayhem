using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Player Movement Settings")]
    [SerializeField] float jumpPower;
    [SerializeField] float fuelAmount;
    [SerializeField] float jumpHeightLimit;
    [SerializeField] float sideSpeed;
    [SerializeField] [Tooltip("Percentage of screen height to be the minimum drag distance")] float dragDistance;
    [SerializeField] bool pcControls;
    [SerializeField] Animator mech;
    [SerializeField] AudioSource footsteps;
    [SerializeField] AudioClip footfall;
    [SerializeField] AudioClip jump;
    [SerializeField] Slider fuelGauge;

    float startY;
    float currentFuel;
    [SerializeField] Transform playerT;
    Vector3 playerPos;
    [SerializeField] Rigidbody playerRb;
    float leftX = -10;
    float rightX = 10;
    //float upY = 8;
    bool moving = false;
    bool jumpStarted = false;
    string swipeType;
    int missile = 0;
    //First and last position of the player's swipe
    Vector2 firstPos;
    Vector2 lastPos;

    // Start is called before the first frame update
    void Start()
    {
        playerPos = playerT.localPosition;
        startY = playerPos.y;
        currentFuel = fuelAmount;
        fuelGauge.maxValue = fuelAmount;
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = playerT.localPosition;
        if (pcControls)
        {
            PCInput();
        }
        else
        {
            InputCheck();
        }
        Move();
        fuelGauge.value = currentFuel;
    }
    //Testing Purpose
    void PCInput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            moving = true;
            swipeType = "LEFT";
            if (playerPos.x > 0) { leftX = 0; }
            else { leftX = -10; }
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            moving = true;
            swipeType = "RIGHT";
            if (playerPos.x < 0) { rightX = 0; }
            else { rightX = 10; }
        }
        else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            moving = true;
            swipeType = "UP";
            playerRb.useGravity = true;
        }
        else if(Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            swipeType = "STATIONARY";
            moving = true;
        }
        else if (!Input.GetKey(KeyCode.Space) && jumpStarted) //Fly Release
        {
            moving = false;
            playerRb.useGravity = true; //Re-enables gravity
            mech.SetBool("Jump", false);
        }
    }
    void InputCheck()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                firstPos = touch.position;
                lastPos = touch.position;
                Debug.Log("Touched");
            }
            else if(touch.phase == TouchPhase.Stationary)
            {
                swipeType = "STATIONARY";
                moving = true;
                //playerRb.useGravity = true;
                Debug.Log(swipeType);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                lastPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                Debug.Log("Touch ended");
                lastPos = touch.position;
                float deltaX = Mathf.Abs(lastPos.x - firstPos.x);
                float deltaY = Mathf.Abs(lastPos.y - firstPos.y);
                //Check to see if they swiped further than the minimum distance
                if (deltaX > dragDistance || deltaY > dragDistance)
                {
                    if (deltaX >= deltaY) //Horizontal Swipe
                    {
                        if (lastPos.x > firstPos.x) //Right Swipe
                        {
                            moving = true;
                            swipeType = "RIGHT";
                            if (playerPos.x < 0) { rightX = 0; }
                            else { rightX = 10; }
                            Debug.Log(swipeType);
                        }
                        else //Left Swipe
                        {
                            moving = true;
                            swipeType = "LEFT";
                            if (playerPos.x > 0) { leftX = 0; }
                            else { leftX = -10; }
                            Debug.Log(swipeType);
                        }
                    }
                    else //Vertical Swipe
                    {
                        if (lastPos.y > firstPos.y) //Up Swipe
                        {
                            moving = true;
                            swipeType = "UP";
                            Debug.Log(swipeType);
                        }
                    }
                }
                else //Fly Release
                {
                    moving = false;
                    playerRb.useGravity = true; //Re-enables gravity
                    footsteps.Stop();
                }
            }
        }
    }
    void Move()
    {
        if (moving)
        {
            if (swipeType.Equals("LEFT") && mech.GetBool("isGrounded"))
            {
                if (playerPos.x > leftX)
                {
                    playerPos = new Vector3(playerPos.x - sideSpeed * Time.deltaTime, playerPos.y, playerPos.z);
                }
                else
                {
                    moving = false;
                }
                playerT.localPosition = playerPos;
            }
            else if (swipeType.Equals("RIGHT") && mech.GetBool("isGrounded"))
            {
                if (playerPos.x < rightX)
                {
                    playerPos = new Vector3(playerPos.x + sideSpeed * Time.deltaTime, playerPos.y, playerPos.z);
                }
                else
                {
                    moving = false;
                }
                playerT.localPosition = playerPos;
            }
            else if (swipeType.Equals("UP"))
            {
                if (!jumpStarted && playerPos.y <= 0.51f)
                {
                    playerRb.useGravity = true;
                    mech.SetBool("Jump", true);
                    mech.SetBool("isGrounded", false);
                    jumpStarted = true;
                    playerRb.AddForce(playerT.up * jumpPower, ForceMode.Impulse);
                }
            }
            else if (jumpStarted && swipeType.Equals("STATIONARY") && playerPos.y > 8)
            {
                if (playerRb.useGravity && currentFuel > 0) //Turns off gravity and stops upward movement
                {
                    Debug.Log("Hover Mode");
                    mech.SetBool("Jump", false);
                    playerRb.useGravity = false;
                    playerRb.velocity = new Vector3(playerRb.velocity.x, 0f, playerRb.velocity.z);
                    currentFuel -= Time.deltaTime;
                }
                else if(currentFuel <= 0f)
                {
                    Debug.Log("Ouf of fuel");
                    moving = false;
                    playerRb.useGravity = true;
                }
                else
                {
                    currentFuel -= Time.deltaTime;
                }

                if (!footsteps.isPlaying)
                {
                    footsteps.Play();
                }
                //Debug.Log(playerPos.y);
            }
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Obstacle"))
        {
            SceneManager.LoadScene("Game Over");
        }
        else if (collision.gameObject.tag.Equals("Ground") && !mech.GetBool("isGrounded"))
        {
            Debug.Log("touchdown");
            mech.SetBool("isGrounded", true);
            jumpStarted = false;
            swipeType = " ";
        }
        else if (collision.gameObject.tag.Equals("Normal")) 
        {
            Debug.Log("Missile 1");
            missile = 1;
            GameObject.Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag.Equals("Homing")) 
        {
            Debug.Log("Missile 2");
            missile = 2;
            GameObject.Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag.Equals("Fuel")) 
        {
            Debug.Log("Fuel");
            currentFuel = fuelAmount;
            GameObject.Destroy(collision.gameObject);
        }
    }
    //The following methods are used to activate the sound effects from the animator
    public void footstepSFX()
    {
        footsteps.PlayOneShot(footfall);
    }
    public void jumpSFX()
    {
        footsteps.PlayOneShot(jump);
    }
}
