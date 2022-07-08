using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtter : MonoBehaviour
{
    public Transform targetTransform;
    public bool applyOffset = false;
    public Vector3 offset;

    private float rotationSpeed = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        // transform.LookAt(targetTransform);
        Debug.DrawLine(transform.position, targetTransform.position);

        // transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetTransform.position), rotationSpeed);

        if (applyOffset) {
            // transform.LookAt(targetTransform,Vector3.up);
            // transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x + offset.x,
            //     transform.localRotation.eulerAngles.y + offset.y, transform.localRotation.eulerAngles.z + offset.z);

            Vector3 relativePos = transform.InverseTransformDirection(targetTransform.position);
            Vector3 targetPos = transform.TransformPoint(relativePos);

            transform.LookAt(targetPos, transform.up);
        }

    }
}
