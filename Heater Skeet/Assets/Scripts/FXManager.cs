using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    public Transform fxDaddy;

    private int currentFX = 0;
    private ParticleSystem[] fx;
    private int doneCount;
    private int currentDone = 0;
    private int[] doneIndexes;
    private HashSet<int> activeFXIndexes = new HashSet<int>();

    // Start is called before the first frame update
    void Start()
    {
        fx = new ParticleSystem[fxDaddy.childCount];
        doneIndexes = new int[fxDaddy.childCount];
        for (int b = 0; b < fxDaddy.childCount; b++) {
            fx[b] = fxDaddy.GetChild(b).GetComponent<ParticleSystem>();
        }
    }

    public void initializeFX(Vector3 pos) {
        int fxIndexStart = currentFX;
        while (fx[currentFX].gameObject.activeSelf) {
            currentFX++;
            if (currentFX >= fx.Length) {
                currentFX = 0;
            } else if (currentFX == fxIndexStart) {
                Debug.LogError("OUTTA FX!!!");
                return;
            }
        }
        fx[currentFX].transform.localPosition = pos;
        fx[currentFX].gameObject.SetActive(true);
        activeFXIndexes.Add(currentFX);
        currentFX++;
        if (currentFX >= fx.Length) {
            currentFX = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        doneCount = 0;
        currentDone = 0;

        foreach(int fxIndex in activeFXIndexes) {
            if (!fx[fxIndex].gameObject.activeSelf) {
                fx[fxIndex].gameObject.SetActive(false);
                doneIndexes[currentDone] = fxIndex;
                doneCount++;
            }
        }

        for (int d = 0; d < doneCount; d++) {
            activeFXIndexes.Remove(doneIndexes[d]);
        }
    }
}
