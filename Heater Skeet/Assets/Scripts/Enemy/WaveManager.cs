using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public enum WavePatternType {
        Static,
        Random
    }

    public WavePattern[] wavePatterns;

    private bool isActive = true;
    private int actionIndex = 0;
    private int waveCounter;
    private float currentDuration;

    // Start is called before the first frame update
    void Start()
    {
        actionIndex = 0;
        nextWave();
    }

    private void nextWave() {
        waveCounter = 0;
        currentDuration = wavePatterns[actionIndex].duration;

        if (wavePatterns[actionIndex].patternType == WavePatternType.Static) {
            wavePatterns[actionIndex].enemyWaves[0].startAction();
        } else if (wavePatterns[actionIndex].patternType == WavePatternType.Random) {
            wavePatterns[actionIndex].enemyWaves[Random.Range(0, wavePatterns[actionIndex].enemyWaves.Length)].startAction();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;

        waveCounter++;
        if (waveCounter >= currentDuration) {
            actionIndex++;
            if (actionIndex >= wavePatterns.Length) {
                actionIndex = 0;
                isActive = false;
            } else {
                nextWave();
            }
        }
    }
}

[System.Serializable]
public class WavePattern {
    public WaveManager.WavePatternType patternType;
    public float duration;
    public EnemyWave[] enemyWaves;
}