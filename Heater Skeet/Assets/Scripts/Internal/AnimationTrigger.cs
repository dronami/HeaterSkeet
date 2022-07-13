using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    public Animator targetAnimator;
    public string triggerName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void triggerAnimation() {
        targetAnimator.SetTrigger(triggerName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
