using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    public enum FXType {
        BulletFX,
        BulletHitFX,
        CrateExplode,

        Count
    }
 
    public Transform fxDaddy;

    private int[] currentFX;
    private ParticleSystem[,] fx;
    private int[,] doneIndexes;
    private HashSet<int>[] activeFXIndexes;

    private int doneCount;
    private int currentDone = 0;

    private const int NUM_FX = 100;

    // Start is called before the first frame update
    void Start()
    {
        currentFX = new int[(int)FXType.Count];
        fx = new ParticleSystem[(int)FXType.Count, NUM_FX];
        doneIndexes = new int[(int)FXType.Count, NUM_FX];
        activeFXIndexes = new HashSet<int>[(int)FXType.Count];
        
        for (int f = 0; f < (int)FXType.Count; f++) {
            currentFX[f] = 0;
            activeFXIndexes[f] = new HashSet<int>();

            for (int b = 0; b < NUM_FX; b++) {
                fx[f,b] = transform.GetChild(f).GetChild(b).GetComponent<ParticleSystem>();
            }
        }
    }

    public void initializeFX(FXType fxType, Vector3 pos) {
        int fxIndexStart = currentFX[(int)fxType];
        while (fx[(int)fxType,currentFX[(int)fxType]].gameObject.activeSelf) {
            currentFX[(int)fxType]++;
            if (currentFX[(int)fxType] >= NUM_FX) {
                currentFX[(int)fxType] = 0;
            } else if (currentFX[(int)fxType] == fxIndexStart) {
                Debug.LogError("OUTTA FX!!!");
                return;
            }
        }
        fx[(int)fxType, currentFX[(int)fxType]].transform.localPosition = pos;
        fx[(int)fxType, currentFX[(int)fxType]].gameObject.SetActive(true);
        activeFXIndexes[(int)fxType].Add(currentFX[(int)fxType]);
        currentFX[(int)fxType]++;
        if (currentFX[(int)fxType] >= NUM_FX) {
            currentFX[(int)fxType] = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int f = 0; f < (int)FXType.Count; f++) {
            doneCount = 0;
            currentDone = 0;

            foreach (int fxIndex in activeFXIndexes[f]) {
                if (!fx[f,fxIndex].gameObject.activeSelf) {
                    fx[f,fxIndex].gameObject.SetActive(false);
                    doneIndexes[f,currentDone] = fxIndex;
                    doneCount++;
                }
            }

            for (int d = 0; d < doneCount; d++) {
                activeFXIndexes[f].Remove(doneIndexes[f, d]);
            }
        }
    }
}
