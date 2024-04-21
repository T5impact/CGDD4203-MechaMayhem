using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour, IHealth
{
    int currentHealth;

    [Header("Player Movement Settings")]
    [SerializeField] int maxHealth = 10;
    [SerializeField] int sparksStartHealthLimit = 5;
    [SerializeField] float jumpPower;
    [SerializeField] float fuelAmount;
    [SerializeField] float jumpHeightLimit;
    [SerializeField] float minNotGrounedHeight = 1;
    [SerializeField] float sideSpeed;
    [SerializeField] [Tooltip("Percentage of screen height to be the minimum drag distance")] float dragDistance;
    [SerializeField] bool pcControls;

    [Header("References")]
    [SerializeField] GameManager gameManager;
    [SerializeField] Launcher missileLauncher;
    [SerializeField] Transform playerT;
    [SerializeField] Rigidbody playerRb;
    [SerializeField] Animator mech;
    [SerializeField] Slider fuelGauge;
    [SerializeField] GameObject sparksEffect;
    [SerializeField] Sprite[] missileIcons;
    [SerializeField] Image missileIcon;
    [SerializeField] TMP_Text missileNameText;

    [Header("Audio")]
    [SerializeField] AudioSource footsteps;
    [SerializeField] AudioSource music;
    [SerializeField] AudioClip backgroundMusic;
    [SerializeField] AudioClip footfall;
    [SerializeField] AudioClip jump;
    [SerializeField] AudioClip explode;
    [SerializeField] AudioClip slide;

    float startY;
    float currentFuel;
    Vector3 playerPos;
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

    bool reachedMinThreshold;
    bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        playerPos = playerT.localPosition;
        startY = playerPos.y;
        currentFuel = fuelAmount;
        fuelGauge.maxValue = fuelAmount;
        missile = 0;
        missileNameText.text = "None";

        isGrounded = true;
        jumpStarted = false;

        currentHealth = maxHealth;
        sparksEffect.SetActive(false);

        if (missileLauncher == null)
            Debug.LogError("MissileLauncher has not assigned a Launcher.");

        if (gameManager == null)
            Debug.LogError("No reference to Game Manager on Player Controller.");
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
            if (playerPos.x - 1 > 0) 
            { 
                leftX = 0; 
            }
            else 
            { 
                leftX = -10;
            }

            if (playerPos.x - 1 > leftX)
            {
                mech.SetTrigger("ShiftLeft");
            }
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            moving = true;
            swipeType = "RIGHT";
            if (playerPos.x + 1 < 0) 
            { 
                rightX = 0; 
            }
            else 
            { 
                rightX = 10;
            }

            if (playerPos.x + 1 < rightX)
            {
                mech.SetTrigger("ShiftRight");
            }
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
                            if (playerPos.x + 1 < 0)
                            {
                                rightX = 0;
                            }
                            else
                            {
                                rightX = 10;
                            }

                            if(playerPos.x + 1 < rightX)
                            {
                                mech.SetTrigger("ShiftRight");
                            }

                            Debug.Log(swipeType);
                        }
                        else //Left Swipe
                        {
                            moving = true;
                            swipeType = "LEFT";
                            if (playerPos.x - 1 > 0)
                            {
                                leftX = 0;
                            }
                            else
                            {
                                leftX = -10;
                            }

                            if (playerPos.x - 1 > leftX)
                            {
                                mech.SetTrigger("ShiftLeft");
                            }
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
        if(!isGrounded && !reachedMinThreshold) reachedMinThreshold = playerPos.y >= minNotGrounedHeight;

        if (moving)
        {
            if(isGrounded)
            {
                playerRb.velocity = new Vector3(playerRb.velocity.x, 0f, playerRb.velocity.z);
            }

            if (swipeType.Equals("LEFT") && isGrounded)
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
                playerRb.velocity = new Vector3(playerRb.velocity.x, 0f, playerRb.velocity.z);
            }
            else if (swipeType.Equals("RIGHT") && isGrounded)
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
                playerRb.velocity = new Vector3(playerRb.velocity.x, 0f, playerRb.velocity.z);
            }
            else if (swipeType.Equals("UP"))
            {
                if (currentFuel > 0 && playerPos.y < jumpHeightLimit )
                {
                    playerRb.useGravity = true;
                    if(isGrounded)
                        mech.SetBool("Jump", true);
                    isGrounded = false;
                    mech.SetBool("isGrounded", false);

                    reachedMinThreshold = !(playerPos.y < minNotGrounedHeight);

                    jumpStarted = true;
                    playerRb.AddForce(playerT.up * jumpPower, ForceMode.Impulse);
                }
            }
            else if (jumpStarted && swipeType.Equals("STATIONARY"))
            {
                if (currentFuel > 0 && playerPos.y >= jumpHeightLimit) //Turns off gravity and stops upward movement
                {
                    Debug.Log("Hover Mode");
                    mech.SetBool("Jump", false);
                    playerRb.useGravity = false;

                    playerRb.velocity = new Vector3(playerRb.velocity.x, 0f, playerRb.velocity.z);
                }
                else if(currentFuel <= 0f)
                {
                    Debug.Log("Ouf of fuel");
                    moving = false;
                    playerRb.useGravity = true;
                }

                /*if(!playerRb.useGravity && playerPos.y >= jumpHeightLimit)
                {
                    playerRb.velocity = new Vector3(playerRb.velocity.x, 0f, playerRb.velocity.z);
                }*/

                if (!footsteps.isPlaying)
                {
                    footsteps.Play();
                }
            }

            if(!isGrounded)
            {
                currentFuel -= Time.deltaTime;
            }
        }
        
    }

    public void LaunchMissile()
    {
        if (missile != 0)
        {
            missileLauncher.LaunchObject(missile - 1);
            missile = 0;
            missileIcon.enabled = false;
            missileNameText.text = "None";
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.parent && collision.transform.parent.tag.Equals("Obstacle"))
        {
            Obstacle obstacle = collision.transform.parent.GetComponent<Obstacle>();
            if (obstacle) 
            { 
                TakeDamage(obstacle.GetDamageAmount());
                obstacle.SpawnDestroyEffect();
            }
            Destroy(collision.transform.parent.gameObject);
            music.PlayOneShot(explode);
        }
        else if (collision.transform.tag.Equals("Obstacle"))
        {
            Obstacle obstacle = collision.transform.GetComponent<Obstacle>();
            if (obstacle)
            {
                TakeDamage(obstacle.GetDamageAmount());
                obstacle.SpawnDestroyEffect();
            }
            Destroy(collision.gameObject);
            music.PlayOneShot(explode);
        }
        
        if (collision.gameObject.tag.Equals("Ground") && !isGrounded && reachedMinThreshold)
        {
            Debug.Log("touchdown");
            mech.SetBool("isGrounded", true);
            footsteps.Stop();
            isGrounded = true;
            jumpStarted = false;
            swipeType = " ";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Normal"))
        {
            if (missile == 0)
            {
                Debug.Log("Missile 1");
                missile = 1;
                missileIcon.enabled = true;
                missileIcon.sprite = missileIcons[missile - 1];
                missileNameText.text = "Normal";
                if (GameManager.arMode) missileLauncher.ShowFOVMissileAR(missile - 1);
            }
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag.Equals("Homing"))
        {
            if (missile == 0)
            {
                Debug.Log("Missile 2");
                missile = 2;
                missileIcon.enabled = true;
                missileIcon.sprite = missileIcons[missile - 1];
                missileNameText.text = "Homing";
                if (GameManager.arMode) missileLauncher.ShowFOVMissileAR(missile - 1);
            }
            Destroy(other.gameObject);
        }

        if (other.gameObject.tag.Equals("Fuel"))
        {
            Debug.Log("Fuel");
            currentFuel = fuelAmount;
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag.Equals("Orb"))
        {
            Debug.Log("Orb");
            gameManager.AddOrb();
            Destroy(other.gameObject);
        }
    }

    //The following methods are used to activate the sound effects from the animator
    public void footstepSFX()
    {
        if (music.volume <= 0) { footsteps.PlayOneShot(footfall); }
    }
    public void jumpSFX()
    {
        footsteps.PlayOneShot(jump);
    }
    public void slideSFX()
    {
        footsteps.PlayOneShot(slide);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        sparksEffect.SetActive(false);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            SceneManager.LoadScene("Game Over");
        } else if (currentHealth <= sparksStartHealthLimit)
        {
            sparksEffect.SetActive(true);
        }
    }
}
