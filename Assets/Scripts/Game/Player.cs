using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Player : MonoBehaviour {
    [Header("Visuals")]
    public GameObject model;
    public GameObject normalModel;
    public GameObject powerUpModel;


    [Header("Acceleration")]
    public float acceleration = 2.5f;
    public float deacceleration = 5.0f;

    [Header("Movement Variables")]
    public float movementSpeed = 4f;
    public float movementSpeedRight = 8f;
    public float movementSpeedLeft = 2f;

    [Header("Jumping Variables")]
    [Range(4,9)]
    public float normalJumpingSpeed = 6f;
    public float LongJumpingSpeed = 10f;
    public float jumpDuration = .75f;
    public float verticalWallJumpingSpeed = 4f;
    public float horizontalWallJumpingSpeed = 3.5f;

    [Header("Power Ups")]
    public float invincibilityDuration = 5f;

    public Action onCollectCoin;

    private float speed = 0f;
    private float jumpingSpeed = 0f;
    private float jumpingTimer = 0f;

    private bool dead = false;
    private bool paused = false;
    private bool canJump = false;
    private bool jumping = false;
    private bool canWallJump = false;
    private bool wallJumpLeft = false;
    private bool onSpeedAreaLeft = false;
    private bool onSpeedAreaRight = false;
    private bool onLongJumpBlock = false;
    private bool finished = false;

    private bool hasPowerUp = false;
    private bool hasInvicibility = false;   

    public bool Dead {
        get {
            return dead;
        }
    }

    public bool Finished {
        get {
            return finished;
        }
    }

    // Use this for initialization
	void Start () {
        jumpingSpeed = normalJumpingSpeed;
        normalModel.SetActive(true);
        powerUpModel.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if (dead) {
            return;
        }

        float targetMovementSpeed = movementSpeed;
        // Accelerate the Player
        speed += acceleration * Time.deltaTime;
        if (onSpeedAreaLeft) {
            targetMovementSpeed = movementSpeedLeft;
        } else if (onSpeedAreaRight) {
            targetMovementSpeed = movementSpeedRight;
        }

       if (speed > targetMovementSpeed){
            speed -= deacceleration * Time.deltaTime;
        }
        
       // Move Horizontal (right)
        GetComponent<Rigidbody>().velocity = new Vector3(
          paused ? 0 : speed,
            GetComponent<Rigidbody>().velocity.y,
            GetComponent<Rigidbody>().velocity.z);

        //Check for input
        bool pressingJumpButton = Input.GetMouseButton(0) || Input.GetKey("space");
        
        if (pressingJumpButton) {
            if (canJump) {
                Jump();
            }
        }


        //check for unpause
        if (paused && pressingJumpButton) {
            paused = false;
        }

        // Having the player Jump
        if (jumping) {
            jumpingTimer += Time.deltaTime;

            if (pressingJumpButton && jumpingTimer < jumpDuration)
            {
                if (onLongJumpBlock) {
                    jumpingSpeed = LongJumpingSpeed;
                }

                  GetComponent<Rigidbody>().velocity = new Vector3(
                    GetComponent<Rigidbody>().velocity.x,
                    jumpingSpeed,
                    GetComponent<Rigidbody>().velocity.z
               );
            }
        }

        // Have the player Wall Jump
        if (canWallJump) {
            speed = 0;
            if (pressingJumpButton){

                canWallJump = false;

                speed = wallJumpLeft ? -horizontalWallJumpingSpeed : horizontalWallJumpingSpeed;

                GetComponent<Rigidbody>().velocity = new Vector3(
                  GetComponent<Rigidbody>().velocity.x,
                verticalWallJumpingSpeed,
                  GetComponent<Rigidbody>().velocity.z
                );
            }
        }
    }


    public void Pause() {
        paused = true;
    }


    void OnTriggerEnter(Collider otherCollider){
        // Collect Coins
        if (otherCollider.transform.GetComponent<Coin>() != null){
            Destroy(otherCollider.gameObject);
            onCollectCoin();
        }
        // Touch Speed Area, Speed Up Or Down
        if (otherCollider.GetComponent<SpeedArea>() != null){
            SpeedArea speedArea = otherCollider.GetComponent<SpeedArea>();
            if (speedArea.direction == Direction.Left){
                onSpeedAreaLeft = true;
            }
            else if (speedArea.direction == Direction.Right){
                onSpeedAreaRight = true;
            }
        }
        // Long Jump
        if(otherCollider.GetComponent<LongJumpBlock>() != null) {
            onLongJumpBlock = true;
        }
        // Kill The Player When the Touch the Enemy
        if (otherCollider.GetComponent<Enemy> () != null) {
            Enemy enemy = otherCollider.GetComponent<Enemy>();
            if (hasInvicibility == false && enemy.Dead == false) {
                if (hasPowerUp == false) {
                    Kill();
                } else{
                    hasPowerUp = false;
                    normalModel.SetActive(true);
                    powerUpModel.SetActive(false);
                    ApplyInvicibility();
                }
            }
        }
        // Collect the PowerUp
        if(otherCollider.GetComponent<PowerUp> () != null) {
            PowerUp powerUp = otherCollider.GetComponent<PowerUp>();
            powerUp.Collect();
            ApplyPowerUp();
        }

        // Reach the FinishLine
        if(otherCollider.GetComponent<FinishLine>() != null) {
            hasInvicibility = true;
            finished = true;
        }
    }


    void OnTriggerStay(Collider otherCollider) {
       if (otherCollider.tag == "JumpingArea") {
            canJump = true;
            jumping = false;
            jumpingSpeed = normalJumpingSpeed;
            jumpingTimer = 0f;
        } else if (otherCollider.tag == "WallJumpingArea") {
            canWallJump = true;
            wallJumpLeft = transform.position.x < otherCollider.transform.position.x;
        }
    }

    void OnTriggerExit(Collider otherCollider){
        if (otherCollider.tag == "WallJumpingArea") {
            canWallJump = false;
}
        if (otherCollider.GetComponent<SpeedArea>() != null){
            SpeedArea speedArea = otherCollider.GetComponent<SpeedArea>();
            if (speedArea.direction == Direction.Left) {
                onSpeedAreaLeft = false;
            } else if (speedArea.direction == Direction.Right){
                onSpeedAreaRight = false;
            }
        }
        if (otherCollider.GetComponent<LongJumpBlock>() != null){ 
            onLongJumpBlock = false;
        }
    }

    void Kill() {
        dead = true;
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().AddForce(new Vector3(0f, 500f, -200f));
    }

    public void Jump(bool forced = false) {
        jumping = true;
        if (forced) {
            GetComponent<Rigidbody>().velocity = new Vector3(
                      GetComponent<Rigidbody>().velocity.x,
                      jumpingSpeed,
                      GetComponent<Rigidbody>().velocity.z
                 );
        }
    }

   void ApplyPowerUp() {
        hasPowerUp = true;
        normalModel.SetActive(false);
        powerUpModel.SetActive(true);
    }

    void ApplyInvicibility()
    {
        hasInvicibility = true;
        StartCoroutine(InvincibiltyRoutine());
    }

    private IEnumerator InvincibiltyRoutine () {
        //Slow Blinks
        float initialWaitingTime = invincibilityDuration * 0.75f;
        int initialBlinks = 20;

        for(int i = 0; i < initialBlinks; i++) {
            model.SetActive(!model.activeSelf);
            yield return new WaitForSeconds(initialWaitingTime / initialBlinks);
        }
         yield return new WaitForSeconds(invincibilityDuration);

        //Fast Blinks
        float finalWaitingTime = invincibilityDuration * 0.25f;
        int finalBlinks = 35;

        for (int  i = 0; i < finalBlinks; i++) {
            model.SetActive(!model.activeSelf);
            yield return new WaitForSeconds(finalWaitingTime / finalBlinks);
        }
        model.SetActive(true);

        hasInvicibility = false;

    }

    public void OnDestroyBrick() {
        GetComponent<Rigidbody>().velocity = new Vector3(
            GetComponent<Rigidbody>().velocity.x,
            0,
            GetComponent<Rigidbody>().velocity.z
            );
        canJump = false;
        jumping = false;
    }
}
