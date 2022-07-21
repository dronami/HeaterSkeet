using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BulletType = BulletManager.BulletType;

public class SliggaSlugger : MonoBehaviour
{
    public enum SliggaState {
        Waiting,
        Idle,
        Rotating,
        Running,
        Aiming,
        Cocking,
        Shooting
    }

    public SliggaState sliggaState;
    public Animator animator;
    public LookAtter aimLooker;
    public LookAtter[] eyeLookers;
    public Transform shootPoint;
    public Transform playerTransform;

    public HitObject[] hitObjects;
    public SliggaAction[] sliggaPatterns;

    private int actionIndex;
    private int stateCounter;
    private int currentDuration;
    private Vector3 startPosition;
    private Vector3 endPosition;

    private bool rotating = false;
    private int rotationCounter;
    private int rotationDuration;
    private Quaternion startRotation;
    private Quaternion endRotation;

    private bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void startPattern() {
        actionIndex = 0;
        nextAction();
        isActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive || sliggaPatterns.Length == 0) return;

        stateCounter++;

        if (sliggaState == SliggaState.Running) {
            transform.position = Vector3.Lerp(startPosition, endPosition, stateCounter / (float)currentDuration);
        }

        if (rotating) {
            rotationCounter++;
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, rotationCounter / (float)rotationDuration);

            if (rotationCounter >= rotationDuration) {
                rotating = false;
            }
        }

        stateCounter++;
        if (stateCounter >= currentDuration) {
            actionIndex++;
            if (actionIndex >= sliggaPatterns.Length) {
                actionIndex = 0;
                isActive = false;
            } else {
                nextAction();
            }
        }
    }

    private void nextAction() {
        if (sliggaPatterns.Length == 0) return;

        stateCounter = 0;
        sliggaState = sliggaPatterns[actionIndex].actionState;
        currentDuration = (int)(sliggaPatterns[actionIndex].duration * 60);
        if (sliggaPatterns[actionIndex].startTransform != null) {
            startPosition = sliggaPatterns[actionIndex].startTransform.position;
            transform.position = startPosition;
        }
        if (sliggaPatterns[actionIndex].endTransform != null) {
            endPosition = sliggaPatterns[actionIndex].endTransform.position;
        }

        if (sliggaPatterns[actionIndex].rotationDuration > 0.0f) {
            transform.rotation = Quaternion.Euler(0.0f, sliggaPatterns[actionIndex].startRotation, 0.0f);
            rotating = true;
            rotationCounter = 0;
            rotationDuration = (int)(sliggaPatterns[actionIndex].rotationDuration * 60);
            startRotation = Quaternion.Euler(0.0f, sliggaPatterns[actionIndex].startRotation, 0.0f);
            endRotation = Quaternion.Euler(0.0f, sliggaPatterns[actionIndex].endRotation, 0.0f);
        }
        if (sliggaState == SliggaState.Idle) {
            aimLooker.enableRotation(false);
        }
        
        if (sliggaState == SliggaState.Running) {
            for (int e = 0; e < eyeLookers.Length; e++) {
                eyeLookers[e].isActive = false;
                eyeLookers[e].transform.localRotation = Quaternion.identity;
            }

            animator.SetBool("isRunning", true);
            aimLooker.enableRotation(false);
        } else {
            for (int e = 0; e < eyeLookers.Length; e++) {
                eyeLookers[e].isActive = true;
            }

            animator.SetBool("isRunning", false);
        }

        if (sliggaState == SliggaState.Cocking) {
            animator.SetBool("isCocking", true);
            aimLooker.enableRotation(true);
        } else {
            animator.SetBool("isCocking", false);
        }

        if (sliggaState == SliggaState.Aiming) {
            animator.SetBool("isAiming", true);
        } else {
            animator.SetBool("isAiming", false);
        }

        if (sliggaState == SliggaState.Shooting) {
            InternalShit.bulletManager.initializeBullet(BulletType.RealBullet, shootPoint.position, playerTransform.position);
            animator.SetBool("isShooting", true);
        } else {
            animator.SetBool("isShooting", false);
        }
    }
}

[System.Serializable]
public class SliggaAction {
    public SliggaSlugger.SliggaState actionState;
    public float duration;
    public Transform startTransform;
    public Transform endTransform;
    public float rotationDuration;
    public float startRotation;
    public float endRotation;
}
