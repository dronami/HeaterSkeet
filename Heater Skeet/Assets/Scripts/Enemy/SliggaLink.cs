using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliggaLink : MonoBehaviour
{
    public Sligga sligga;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void startShrinking() {
        sligga.startShrinking();
    }

    public void finishHandExplode() {
        sligga.finishHandExplode();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
