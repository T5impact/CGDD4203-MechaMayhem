using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerControlelr : MonoBehaviour
{
    [Header("Player Movement Settings")]
    [SerializeField]
    float jumpPower;
    [SerializeField]
    float fuelAmount;
    [SerializeField]
    float jumpHeightLimit;
    [SerializeField]
    float sideSpeed;
    [SerializeField]
    [Tooltip("Percentage of screen height to be the minimum drag distance")]
    float dragDistance;
    [SerializeField]
    bool pcControls;
    [SerializeField]
    Animator mech;
    [SerializeField]
    AudioSource footsteps;
    [SerializeField]
    AudioClip footfall;
    [SerializeField]
    AudioClip jump;
    [SerializeField]
    Slider fuelGauge;

    float startY;
    float currentFuel;
    [SerializeField]
    Transform playerT;
    Vector3 playerPos;
    [SerializeField]
    Rigidbody playerRb;
    float leftX = -10;
    float rightX = 10;
    //float upY = 8;
    bool moving = false;
    bool jumpStarted = false;
    string swipeType;

    //First and last position of the player's swipe
   Vector2 firstPos;
   Vector2 lastPos;

    // Start is called before the first frame update
    void Start()
    {
        playerPos = playerT.position;
        startY = playerPos.y;
        currentFuel = fuelAmount;
        fuelGauge.maxValue = fuelAmount;
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = playerT.position;
        if (pcControls)
        {
            pcInput();
        }
        else
        {
            inputCheck();
        }
        move();
        fuelGauge.value = currentFuel;
    }
    //Testing Purpose
    void pcInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            moving = true;
            swipeType = "LEFT";
            if (playerPos.x > 0) { leftX = 0; }
            else { leftX = -10; }
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            moving = true;
            swipeType = "RIGHT";
            if (playerPos.x < 0) { rightX = 0; }
            else { rightX = 10; }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            moving = true;
            swipeType = "STATIONARY";
            playerRb.useGravity = true;
        }
        else if (!Input.GetKey(KeyCode.Space) && jumpStarted) //Fly Release
        {
            moving = false;
            playerRb.useGravity = true; //Re-enables gravity
            mech.SetBool("Jump", false);
        }
    }
    void inputCheck()
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
            else if (touch.phase == TouchPhase.Moved)
            {
                lastPos = touch.position;
            }
            else if(touch.phase == TouchPhase.Stationary)
            {
                swipeType = "STATIONARY";
                moving = true;
                playerRb.useGravity = true;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
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
                            else { rightX = 10;  }
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
                    else //Fly Release
                    {
                        moving = false;
                        jumpStarted = false;
                        playerRb.useGravity = true; //Re-enables gravity
                    }
                    /*else //Vertical Swipe
                    {
                        if (lastPos.y > firstPos.y) //Up Swipe
                        {
                            moving = true;
                            swipeType = "UP";
                            mech.SetBool("Jump", true);
                            mech.SetBool("isGrounded", false);
                            Debug.Log(swipeType);
                        }
                    }*/
                }
            }
        }
    }
    void move()
    {
        if (moving)
        {
            if (swipeType.Equals("LEFT"))
            {
                if (playerPos.x > leftX)
                {
                    playerPos = new Vector3(playerPos.x - sideSpeed * Time.deltaTime, playerPos.y, playerPos.z);
                }
                else
                {
                    moving = false;
                }
                playerT.position = playerPos;
            }
            else if (swipeType.Equals("RIGHT"))
            {
                if (playerPos.x < rightX)
                {
                    playerPos = new Vector3(playerPos.x + sideSpeed * Time.deltaTime, playerPos.y, playerPos.z);
                }
                else
                {
                    moving = false;
                }
                playerT.position = playerPos;
            }
            else if (swipeType.Equals("STATIONARY"))
            {
                if (!jumpStarted)
                {
                    mech.SetBool("Jump", true);
                    mech.SetBool("isGrounded", false);
                    jumpStarted = true;
                    playerRb.AddForce(0f, jumpPower, 0f, ForceMode.Impulse);
                }
                else if (playerPos.y >= jumpHeightLimit && playerRb.useGravity && currentFuel > 0) //Turns off gravity and stops upward movement
                {
                    Debug.Log("Hover Mode");
                    mech.SetBool("Jump", false);
                    playerRb.useGravity = false;
                    playerRb.velocity = new Vector3(playerRb.velocity.x, 0f, playerRb.velocity.z);
                    currentFuel -= Time.deltaTime;
                }
                else if(currentFuel <= 0f)
                {
                    
                    moving = false;
                    playerRb.useGravity = true;
                }
                else
                {
                    currentFuel -= Time.deltaTime;
                }
                Debug.Log(playerPos.y);
            }
        }
        if (playerPos.y <= 0.51f && !mech.GetBool("Jump"))
        {
            mech.SetBool("isGrounded", true);
            jumpStarted = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Obstacle"))
        {
            SceneManager.LoadScene("Game Over");
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
