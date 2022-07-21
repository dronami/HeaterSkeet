using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialFlasher : MonoBehaviour
{
    private enum FlasherState {
        FlashingUp,
        FlashingDown
    }

    private FlasherState flasherState = FlasherState.FlashingUp;

    public Material targetMaterial;

    public Vector3Int flashDurations;
    public Color startColos = Color.white;
    public Color endColos = Color.red;

    private Color diffColos;

    private int frameCounter = 0;
    private float currentDuration;

    private readonly string COLOR_STRING = "_EmissionColor";

    // Start is called before the first frame update
    void Start()
    {
        diffColos = endColos - startColos;

        frameCounter = 0;
        flasherState = FlasherState.FlashingUp;

        currentDuration = (float)flashDurations.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (flasherState == FlasherState.FlashingUp) {
            frameCounter++;
            targetMaterial.SetColor(COLOR_STRING, startColos + diffColos * (frameCounter / currentDuration));

            if (frameCounter >= currentDuration) {
                flasherState = FlasherState.FlashingDown;
                frameCounter = 0;
                currentDuration = (float)flashDurations.y;
            }
        } else if (flasherState == FlasherState.FlashingDown) {
            frameCounter++;
            targetMaterial.SetColor(COLOR_STRING, endColos - diffColos * (frameCounter / currentDuration));

            if (frameCounter >= currentDuration) {
                flasherState = FlasherState.FlashingUp;
                frameCounter = 0;
                currentDuration = (float)flashDurations.x;
            }
        }
    }
}
