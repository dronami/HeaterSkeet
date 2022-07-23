using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sligga : MonoBehaviour
{
    public enum SliggaState {
        Waiting,
        Idle,
        Rotating,
        Running,
        Aiming,
        Cocking,
        Shooting,

        Dying,
        DyingShrinking,
        Dead
    }

    public SliggaState sliggaState;
    public Animator animator;

    public LookAtter aimLooker;

    public Transform shootPoint;
    public Transform playerTransform;

    public HitObject[] hitObjects;
    public SliggaAction[] sliggaPatterns;

    protected Vector3 startPos;

    protected int actionIndex;
    protected int stateCounter;
    protected int currentDuration;
    protected Vector3 startPosition;
    protected Vector3 endPosition;

    protected bool rotating = false;
    protected int rotationCounter;
    protected int rotationDuration;
    protected Quaternion startRotation;
    protected Quaternion endRotation;

    protected bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    protected void basicNextAction() {
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
    }

    public virtual void resetEnemy() {
        isActive = true;
        transform.localScale = Vector3.one;
        for (int h = 0; h < hitObjects.Length; h++) {
            hitObjects[h].gameObject.SetActive(true);
            hitObjects[h].resetHitObject();
        }
    }

    protected virtual void nextAction() {
        if (sliggaPatterns.Length == 0) return;

        basicNextAction();
    }

    protected void basicUpdate() {
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
                gameObject.SetActive(false);
            } else {
                nextAction();
            }
        }
    }

    public virtual void startDying() {
        sliggaState = SliggaState.Dying;
    }

    public virtual void startShrinking() {
        stateCounter = 0;
        sliggaState = SliggaState.DyingShrinking;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
