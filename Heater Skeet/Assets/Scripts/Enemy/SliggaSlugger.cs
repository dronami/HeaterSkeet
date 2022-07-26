using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BulletType = BulletManager.BulletType;

public class SliggaSlugger : Sligga
{ 
    public enum SliggaSluggerType {
        OneGunner,
        FlipOffer
    }

    public SliggaSluggerType sliggaType;

    public LookAtter[] eyeLookers;
    public Transform gunTransformLeft;

    private const float shrinkDuration = 30.0f;
    private float currentScale;

    private int stalkCount;

    private readonly Dictionary<SliggaSluggerType, string> type2InitialAnimation 
        = new Dictionary<SliggaSluggerType, string>() {
        { SliggaSluggerType.OneGunner, "1Gun-Idle" },
        { SliggaSluggerType.FlipOffer, "NoGun-Idle" },
    };

    private readonly Dictionary<SliggaSluggerType, string> type2DyingAnimation
        = new Dictionary<SliggaSluggerType, string>() {
        { SliggaSluggerType.OneGunner, "1Gun-DyingA" },
        { SliggaSluggerType.FlipOffer, "NoGun-DyingA" },
    };

    private readonly Dictionary<SliggaSluggerType, string> type2StalkDyingAnimation
        = new Dictionary<SliggaSluggerType, string>() {
        { SliggaSluggerType.OneGunner, "1Gun-StalkDyingA" },
        { SliggaSluggerType.FlipOffer, "NoGun-StalkDyingA" },
    };

    /*
    private readonly string INITIAL_ANIMATION_NAME = "1Gun-Idle";
    private readonly string[] DEATH_ANIMATION_NAMES = new string[] {
        "1Gun-DyingA"
    };
    private readonly string[] STALK_DEATH_ANIMATION_NAMES = new string[] {
        "1Gun-StalkDyingA"
    };
    */

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive || sliggaPatterns.Length == 0) return;

        if (sliggaState == SliggaState.Dying) {

        } else if (sliggaState == SliggaState.DyingShrinking) {
            stateCounter++;
            currentScale = 1.0f - (stateCounter / shrinkDuration);
            transform.localScale = new Vector3(currentScale, currentScale, currentScale);

            if (stateCounter >= shrinkDuration) {
                sliggaState = SliggaState.Dead;
                transform.position = startPos;
                gameObject.SetActive(false);
            }
        } else if (sliggaState == SliggaState.Dead) {

        } else {
            basicUpdate();
        }
    }

    public override void resetEnemy() {
        base.resetEnemy();

        sliggaState = SliggaState.Idle;
        animator.Play(type2InitialAnimation[sliggaType]);

        if (sliggaType == SliggaSluggerType.OneGunner) {
            gunTransformLeft.gameObject.SetActive(true);
        } else if (sliggaType == SliggaSluggerType.FlipOffer) {
            gunTransformLeft.gameObject.SetActive(false);
        }

        stalkCount = 2;
    }

    public void startPattern() {
        actionIndex = 0;
        nextAction();
        isActive = true;
    }

    protected override void nextAction() {
        if (sliggaPatterns.Length == 0
            || sliggaState == SliggaState.Dying
            || sliggaState == SliggaState.DyingShrinking
            || sliggaState == SliggaState.Dead) return;

        basicNextAction();
        
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

    public override void startDying() {
        base.startDying();

        for (int e = 0; e < eyeLookers.Length; e++) {
            eyeLookers[e].isActive = false;
            eyeLookers[e].transform.localRotation = Quaternion.identity;
        }

        animator.Play(type2DyingAnimation[sliggaType]);
        aimLooker.isActive = false;
    }

    public override void onStalkDestroyed() {
        stalkCount--;
        if (stalkCount <= 0) {
            base.startDying();

            for (int e = 0; e < eyeLookers.Length; e++) {
                eyeLookers[e].isActive = false;
                eyeLookers[e].transform.localRotation = Quaternion.identity;
            }

            animator.Play(type2StalkDyingAnimation[sliggaType]);
            aimLooker.isActive = false;
        }
    }

    public override void startShrinking() {
        base.startShrinking();
        for (int h = 0; h < hitObjects.Length; h++) {
            hitObjects[h].disableHitObject();
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
