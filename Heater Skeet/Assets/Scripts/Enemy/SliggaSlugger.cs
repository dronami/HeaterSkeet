using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        actionIndex = 0;
        nextAction();
    }

    // Update is called once per frame
    void Update()
    {
        stateCounter++;

        if (sliggaState == SliggaState.Running) {
            transform.position = Vector3.Lerp(startPosition, endPosition, stateCounter / (float)currentDuration);
        }

        if (rotating) {
            Debug.Log(startRotation + " vs. " + endRotation);
            Debug.Log("Rat: " + rotationCounter / (float)rotationDuration);
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
            }

            nextAction();
        }
    }

    private void nextAction() {
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
        if (sliggaState == SliggaState.Running) {
            animator.SetBool("isRunning", true);
        } else {
            animator.SetBool("isRunning", false);
        }

        if (sliggaState == SliggaState.Cocking) {
            animator.SetBool("isCocking", true);
        } else {
            animator.SetBool("isCocking", false);
        }

        if (sliggaState == SliggaState.Aiming) {
            animator.SetBool("isAiming", true);
        } else {
            animator.SetBool("isAiming", false);
        }

        if (sliggaState == SliggaState.Shooting) {
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
