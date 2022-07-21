using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BulletType = BulletManager.BulletType;

public class Bullet : MonoBehaviour
{
    public BulletType bulletType;
    private Vector3 startPosition;
    private Vector3 endPosition;

    private Vector3 bulletVelocity;
    public bool bulletActive = false;
    public bool hitEnemy = false;
    private bool ready2Delete = false;

    private const float MAX_DISTANCE = 50.0f;

    private HitObject otherHit;

    private float bulletSpeed;
    private readonly Dictionary<BulletType, float> type2Speed = new Dictionary<BulletType, float>() {
        { BulletType.PlayerBasic, 1.2f },
        { BulletType.EnemyBasic, 0.1f },
        { BulletType.RealBullet, 0.15f }
    };

    private bool rotates = false;
    private readonly Dictionary<BulletType, bool> type2Rotates = new Dictionary<BulletType, bool>() {
        { BulletType.PlayerBasic, true },
        { BulletType.EnemyBasic, false },
        { BulletType.RealBullet, false },
    };

    private const float BULLET_SPEED = 0.6f;

    public int reticleIndex;

    private int frameCounter = 0;
    private float duration = 120.0f;

    Vector2 diff;
    float angle;

    private readonly Vector3 ROTATE_VEL = new Vector3(30.0f, 0.0f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        bulletSpeed = type2Speed[bulletType];
        rotates = type2Rotates[bulletType];
    }

    public void initializeBullet(Vector3 startPos, Vector3 endPos) {
        startPosition = startPos;
        endPosition = endPos;

        Vector3 diffAss = endPos - startPos;
        transform.localPosition = startPosition;
        transform.localRotation = Quaternion.identity;
        transform.LookAt(endPos);
        bulletVelocity = (endPos - startPos).normalized * type2Speed[bulletType];

        bulletActive = true;
        ready2Delete = false;
        hitEnemy = false;

        frameCounter = 0;
    }

    public bool updateBullet() {
        if (bulletActive) {
            if (rotates) {
                transform.Rotate(ROTATE_VEL);
            }
            frameCounter++;
            //transform.localPosition = Vector3.Lerp(startPosition, endPosition, frameCounter / duration);
            transform.localPosition += bulletVelocity;
            if (transform.localPosition.z >= MAX_DISTANCE || transform.localPosition.z <= -10.0f || ready2Delete) {
                bulletActive = false;
                return true;
            }
        }

        return false;
    }

    private void OnTriggerEnter(Collider other) {
        ready2Delete = true;
        otherHit = other.GetComponent<HitObject>();
        if (otherHit) {
            otherHit.getHit(1);
            hitEnemy = otherHit.isEnemy;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
