using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtter : MonoBehaviour
{
    public bool isActive = true;

    public Transform targetTransform;
    public bool applyOffset = false;
    public Vector3 offset;

    public bool instaRotation = true;
    private Quaternion endQuaternion;

    private int rotationCounter = 0;
    private const float ROT_DUR = 60.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void enableRotation(bool e) {
        isActive = e;
        rotationCounter = 0;
    }

    // Update is called once per frame
    void Update() {
        if (!isActive) return;
        // transform.LookAt(targetTransform);
        // Debug.DrawLine(transform.position, targetTransform.position);

        if (instaRotation) {
            transform.rotation = Quaternion.LookRotation(transform.position - targetTransform.position);
            if (applyOffset) {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x + offset.x,
                    transform.rotation.eulerAngles.y + offset.y,
                    transform.rotation.eulerAngles.z + offset.z);
            }
        } else {
            rotationCounter++;
            endQuaternion = Quaternion.LookRotation(transform.position - targetTransform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, endQuaternion, rotationCounter / ROT_DUR);
        }
    }
}
