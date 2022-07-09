using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 startPosition;
    private Vector3 endPosition;

    private Vector3 bulletVelocity;
    public bool bulletActive = false;
    private bool ready2Delete = false;

    private const float BULLET_SPEED = 0.6f;

    private int frameCounter = 0;
    private float duration = 120.0f;

    Vector2 diff;
    float angle;

    private readonly Vector3 ROTATE_VEL = new Vector3(30.0f, 0.0f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void initializeBullet(Vector3 startPos, Vector3 endPos) {
        startPosition = startPos;
        endPosition = endPos;

        Vector3 diffAss = endPos - startPos;
        transform.localPosition = startPosition;
        transform.localRotation = Quaternion.identity;
        transform.LookAt(endPos);
        bulletVelocity = (endPos - startPos).normalized * BULLET_SPEED;

        bulletActive = true;
        ready2Delete = false;

        frameCounter = 0;
    }

    public bool updateBullet() {
        if (bulletActive) {
            transform.Rotate(ROTATE_VEL);
            frameCounter++;
            //transform.localPosition = Vector3.Lerp(startPosition, endPosition, frameCounter / duration);
            transform.localPosition += bulletVelocity;
            if (transform.localPosition.z >= 10.0f || ready2Delete) {
                bulletActive = false;
                return true;
            }
        }

        return false;
    }

    private void OnTriggerEnter(Collider other) {
        ready2Delete = true;
        // other.GetComponent<Flasher>().startFlashing();
        other.GetComponent<HitObject>().getHit(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}