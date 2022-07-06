using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flasher : MonoBehaviour
{
    private enum FlasherState {
        Idle,
        FlashingUp,
        Holding,
        FlashingDown
    }

    private FlasherState flasherState = FlasherState.Idle;

    public Material baseMaterial;

    public Vector3Int flashDurations;
    public Color startColos = Color.white;
    public Color endColos = Color.red;

    private Color diffColos;

    public Renderer[] flashRenderers;

    private int frameCounter = 0;
    private float currentDuration;

    private MaterialPropertyBlock mpb;
    private Color currentColos;

    private readonly string COLOR_STRING = "_EmissionColor";

    // Start is called before the first frame update
    void Start()
    {
        baseMaterial.EnableKeyword("_EMISSION");
        mpb = new MaterialPropertyBlock();
        diffColos = endColos - startColos;
    }

    public void startFlashing() {
        if (flasherState != FlasherState.Idle) return;

        frameCounter = 0;
        flasherState = FlasherState.FlashingUp;

        currentColos = startColos;
        currentDuration = (float)flashDurations.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (flasherState == FlasherState.FlashingUp) {
            frameCounter++;
            currentColos = startColos + diffColos * (frameCounter / currentDuration);

            for (int f = 0; f < flashRenderers.Length; f++) {
                flashRenderers[f].GetPropertyBlock(mpb);
                mpb.SetColor(COLOR_STRING, currentColos);
                flashRenderers[f].SetPropertyBlock(mpb);
            }

            if (frameCounter >= currentDuration) {
                flasherState = FlasherState.Holding;
                frameCounter = 0;
                currentDuration = (float)flashDurations.y;
            }
        } else if (flasherState == FlasherState.Holding) {
            frameCounter++;
            if (frameCounter >= currentDuration) {
                flasherState = FlasherState.FlashingDown;
                frameCounter = 0;
                currentDuration = (float)flashDurations.z;
            }
        } else if (flasherState == FlasherState.FlashingDown) {
            frameCounter++;
            currentColos = endColos - diffColos * (frameCounter / currentDuration);

            for (int f = 0; f < flashRenderers.Length; f++) {
                flashRenderers[f].GetPropertyBlock(mpb);
                mpb.SetColor(COLOR_STRING, currentColos);
                flashRenderers[f].SetPropertyBlock(mpb);
            }

            if (frameCounter >= currentDuration) {
                flasherState = FlasherState.Idle;
            }
        }
    }
}
