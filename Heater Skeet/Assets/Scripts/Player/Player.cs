using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

using BulletType = BulletManager.BulletType;

public class Player : MonoBehaviour
{
    [SerializeField]
    LayerMask layerMask;

    public Camera reticleCamera;

    private MainInputActions controls;

    public TextMeshProUGUI display;

    public Transform playerTransform;
    public Transform reticleTransform;
    public Transform placerReticle;
    public Transform laserSightTransform;
    public SpriteRenderer reticleSprite;

    public float reticleSpeed = 0.45f;

    private float PLAYER_SPEED = 0.05f;
    // private float RETICLE_SPEED = 0.45f;

    private float PLAYER_MAX_X = 3.1f;
    private readonly Vector2 RETICLE_MAX = new Vector2(20.0f, 7.0f);

    private Vector2 moveVelocity;
    private Vector2 reticleVelocity;

    private Quaternion aimRotation = Quaternion.identity;
    private Vector3 shootTarget;

    private float reticleDistance = 1.0f;

    private Vector2 diff;

    private bool jumping = false;
    private int jumpCount = 0;
    private const int MAX_JUMPS = 2;
    private float jumpVelocity;
    private const float GROUND_Y = -1.0f;
    private const float JUMP_VELOCITY_START = 0.15f;
    private const float JUMP_GRAVITY = -0.0125f;

    private bool shooting = false;
    private int shotCooldownTimer;
    private const int SHOT_COOLDOWN = 2;

    RaycastHit hitInfo;

    public Color hitColor = Color.red;
    public Color missColor = Color.green;

    private const float LASER_SIGHT_SCALE = 50.0f;

    // Start is called before the first frame update
    void Awake()
    {
        InternalShit.bulletManager = FindObjectOfType<BulletManager>();
        InternalShit.fxManager = FindObjectOfType<FXManager>();

        Application.targetFrameRate = 60;

        // Instantiate and enable controls object
        controls = new MainInputActions();
        controls.Enable();
        
        // Setup player movement callback
        controls.Playa.MovePlayer.performed += context => movePlaya(context.ReadValue<Vector2>());
        controls.Playa.MovePlayer.canceled += context => stopMovePlaya();

        // Setup reticle movement callback
        controls.Playa.MoveReticle.performed += context => moveReticle(context.ReadValue<Vector2>());
        controls.Playa.MoveReticle.canceled += context => stopMoveReticle();

        // Setup jump callback
        controls.Playa.Jump.performed += context => startJump();

        // Setup shoot callback
        controls.Playa.Shoot.performed += context => startShoot();
        controls.Playa.Shoot.canceled += context => endShoot();

        shootTarget = playerTransform.position + new Vector3(0.0f, 0.0f, 50.0f);
    }

    private void movePlaya(Vector2 direction) {
        moveVelocity = direction;
    }

    private void stopMovePlaya() {
        moveVelocity = Vector2.zero;
    }

    private void moveReticle(Vector2 direction) {
        reticleVelocity = direction;
    }

    private void stopMoveReticle() {
        reticleVelocity = Vector2.zero;
    }

    private void startJump() {
        if (jumping && jumpCount >= MAX_JUMPS) return;

        jumping = true;
        jumpCount++;
        jumpVelocity = JUMP_VELOCITY_START;
    }

    private void startShoot() {
        if (!shooting) {
            InternalShit.bulletManager.initializeBullet(BulletType.PlayerBasic, playerTransform.position, shootTarget /*reticleTransform.position*/);
            shooting = true;
            shotCooldownTimer = 0;
        }
    }

    private void endShoot() {
        shooting = false;
    }

    // Update is called once per frame
    void Update() {
        display.text = "" + laserSightTransform.rotation.eulerAngles + ": " +
            Vector3.Distance(playerTransform.position, shootTarget /*reticleTransform.position*/).ToString("0.00");
        // reticleSprite.transform.Rotate(0.0f, 0.0f, 5.0f);

        // Reticle Shit =================================================================

        if (reticleVelocity != Vector2.zero) {
            laserSightTransform.Rotate(-reticleVelocity.y * reticleSpeed, reticleVelocity.x * reticleSpeed, 0.0f);
            if (Physics.Raycast(playerTransform.position,
            laserSightTransform.rotation * Vector3.forward,
            out hitInfo, 100.0f, layerMask, QueryTriggerInteraction.Collide)) {
                shootTarget = hitInfo.point;
            }
        } else {
            laserSightTransform.LookAt(shootTarget);
        }
        if (Physics.Raycast(playerTransform.position,
            laserSightTransform.rotation * Vector3.forward,
            /*(reticleTransform.position - playerTransform.position),*/
            out hitInfo, 100.0f, layerMask, QueryTriggerInteraction.Collide)) {

            reticleDistance = Vector3.Distance(playerTransform.position,
                hitInfo.point);
            reticleTransform.position = // Camera.main.WorldToScreenPoint(shootTarget);
                hitInfo.point;
            laserSightTransform.localScale = new Vector3(laserSightTransform.localScale.x,
                laserSightTransform.localScale.y, reticleDistance * LASER_SIGHT_SCALE);

            if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
                
                laserSightTransform.GetComponent<Renderer>().material.color = hitColor;
                reticleSprite.color = hitColor;
            } else {
                laserSightTransform.GetComponent<Renderer>().material.color = missColor;
                reticleSprite.color = missColor;
            }

            shootTarget = hitInfo.point;
        } else {
            // Debug.DrawRay(playerTransform.position, (reticleTransform.position - playerTransform.position), Color.green);
            laserSightTransform.GetComponent<Renderer>().material.color = missColor;
            reticleSprite.color = missColor;
        }

        // Player Shit =================================================================

        playerTransform.localPosition = new Vector3(playerTransform.localPosition.x + moveVelocity.x * PLAYER_SPEED,
            playerTransform.localPosition.y, playerTransform.localPosition.z);
        // Enforce player bounds
        if (playerTransform.localPosition.x < -PLAYER_MAX_X) {
            playerTransform.localPosition = new Vector3(-PLAYER_MAX_X, playerTransform.localPosition.y, 
                playerTransform.localPosition.z);
        } else if (playerTransform.localPosition.x > PLAYER_MAX_X) {
            playerTransform.localPosition = new Vector3(PLAYER_MAX_X, playerTransform.localPosition.y,
                playerTransform.localPosition.z);
        }

        /*
        reticleTransform.localPosition = new Vector3(reticleTransform.localPosition.x + reticleVelocity.x * reticleSpeed,
            reticleTransform.localPosition.y + reticleVelocity.y * reticleSpeed, reticleTransform.localPosition.z);
        */

        
        
        // Enforce reticle bounds
        
        /*
        if (reticleTransform.localPosition.x < -RETICLE_MAX.x) {
            reticleTransform.localPosition = new Vector3(-RETICLE_MAX.x, reticleTransform.localPosition.y,
                reticleTransform.localPosition.z);
        } else if (reticleTransform.localPosition.x > RETICLE_MAX.x) {
            reticleTransform.localPosition = new Vector3(RETICLE_MAX.x, reticleTransform.localPosition.y,
                reticleTransform.localPosition.z);
        }
        if (reticleTransform.localPosition.y < -RETICLE_MAX.y) {
            reticleTransform.localPosition = new Vector3(reticleTransform.localPosition.x, -RETICLE_MAX.y,
                reticleTransform.localPosition.z);
        } else if (reticleTransform.localPosition.y > RETICLE_MAX.y) {
            reticleTransform.localPosition = new Vector3(reticleTransform.localPosition.x, RETICLE_MAX.y,
                reticleTransform.localPosition.z);
        }
        */

        if (jumping) {
            playerTransform.localPosition = new Vector3(playerTransform.localPosition.x, playerTransform.localPosition.y + jumpVelocity,
                playerTransform.localPosition.z);

            jumpVelocity += JUMP_GRAVITY;

            if (jumpVelocity < 0.0f && playerTransform.localPosition.y <= GROUND_Y) {
                jumping = false;
                jumpCount = 0;
                playerTransform.localPosition = new Vector3(playerTransform.localPosition.x, GROUND_Y, playerTransform.localPosition.z);
            }
        } 
        /*
        else if (reticleTransform.position.y < 0.0f) {
            reticleTransform.position = new Vector3(reticleTransform.position.x, 0.0f, reticleTransform.position.z);
        }
        */

        if (shooting) {
            shotCooldownTimer++;
            if (shotCooldownTimer > SHOT_COOLDOWN) {
                InternalShit.bulletManager.initializeBullet(BulletType.PlayerBasic, playerTransform.position,
                    shootTarget);
                /*
                InternalShit.bulletManager.initializeBullet(BulletType.PlayerBasic, playerTransform.position + new Vector3(0.5f, -0.5f, 0.0f), 
                    shootTarget);
                InternalShit.bulletManager.initializeBullet(BulletType.PlayerBasic, playerTransform.position + new Vector3(-0.5f, -0.5f, 0.0f),
                    shootTarget);
                */
                shotCooldownTimer = 0;
            }
        }
    }
}
