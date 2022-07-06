using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Player : MonoBehaviour
{
    private MainInputActions controls;

    public TextMeshProUGUI display;
    public BulletManager bulletManager;

    public Transform playerTransform;
    public Transform reticleTransform;

    private float PLAYER_SPEED = 0.05f;
    private float RETICLE_SPEED = 0.45f;

    private float PLAYER_MAX_X = 3.1f;
    private readonly Vector2 RETICLE_MAX = new Vector2(12.0f, 5.0f);

    private Vector2 moveVelocity;
    private Vector2 reticleVelocity;

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

    // Start is called before the first frame update
    void Awake()
    {
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
            bulletManager.initializeBullet(playerTransform.position, reticleTransform.position);
            shooting = true;
            shotCooldownTimer = 0;
        }
    }

    private void endShoot() {
        shooting = false;
    }

    // Update is called once per frame
    void Update()
    {
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

        reticleTransform.localPosition = new Vector3(reticleTransform.localPosition.x + reticleVelocity.x * RETICLE_SPEED,
            reticleTransform.localPosition.y + reticleVelocity.y * RETICLE_SPEED, reticleTransform.localPosition.z);
        // Enforce reticle bounds
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

        if (shooting) {
            shotCooldownTimer++;
            if (shotCooldownTimer > SHOT_COOLDOWN) {
                bulletManager.initializeBullet(playerTransform.position, reticleTransform.position);
                shotCooldownTimer = 0;
            }
        }
    }
}
