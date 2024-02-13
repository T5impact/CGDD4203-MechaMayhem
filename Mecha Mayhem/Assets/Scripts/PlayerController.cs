using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControlelr : MonoBehaviour
{
    [Header("Player Movement Settings")]
    [SerializeField]
    float jumpPower;
    [SerializeField]
    float gravityValue;
    [SerializeField]
    float sideSpeed;
    [SerializeField]
    [Tooltip("Percentage of screen height to be the minimum drag distance")]
    float dragDistance;
    [SerializeField]
    bool pcControls;
    [SerializeField]
    Animator mech;

    float startY;
    Transform playerT;
    Vector3 playerPos;
    float leftX = -10;
    float rightX = 10;
    float upY = 8;
    bool moving = false;
    string swipeType;

    //First and last position of the player's swipe
   Vector2 firstPos;
   Vector2 lastPos;

    // Start is called before the first frame update
    void Start()
    {
        playerT = this.gameObject.transform;
        playerPos = playerT.position;
        startY = playerPos.y;
    }

    // Update is called once per frame
    void Update()
    {
        if(!moving)
        {
            if (pcControls)
            {
                pcInput();
            }
            else
            {
                inputCheck();
            }
        }
        
        move();
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
        else if (Input.GetKeyDown(KeyCode.W))
        {
            moving = true;
            swipeType = "UP";
            mech.SetBool("Jump", true);
            mech.SetBool("isGrounded", false);
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
                    else //Vertical Swipe
                    {
                        if (lastPos.y > firstPos.y) //Up Swipe
                        {
                            moving = true;
                            swipeType = "UP";
                            mech.SetBool("Jump", true);
                            mech.SetBool("isGrounded", false);
                            Debug.Log(swipeType);
                        }
                    }
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
            }
            else if (swipeType.Equals("UP"))
            {
                if (playerPos.y < upY)
                {
                    playerPos = new Vector3(playerPos.x, playerPos.y + jumpPower * Time.deltaTime, playerPos.z);
                }
                else
                {
                    moving = false;
                }
            }
        }
        else
        {
            if (playerPos.y > 0.4f)
            {
                playerPos = new Vector3(playerPos.x, playerPos.y - gravityValue * Time.deltaTime, playerPos.z);
            }
            else
            {
                mech.SetBool("isGrounded", true);
            }
        }
        playerT.position = playerPos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Obstacle"))
        {
            SceneManager.LoadScene("Game Over");
        }
    }
}
