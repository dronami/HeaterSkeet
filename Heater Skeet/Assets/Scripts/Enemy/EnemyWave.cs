using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWave : MonoBehaviour
{
    public enum WaveActionType {
        Enemy,
        Animation
    }

    public WaveAction[] waveActions;

    private bool isActive = false;
    private int actionIndex = 0;
    private int waveCounter;
    private float currentDuration;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void startAction() {
        isActive = true;
        actionIndex = 0;
        nextAction();
    }

    private void nextAction() {
        waveCounter = 0;
        currentDuration = waveActions[actionIndex].duration;

        if (waveActions[actionIndex].actionType == WaveActionType.Enemy) {
            waveActions[actionIndex].enemy.resetEnemy();
            waveActions[actionIndex].enemy.startPattern();
        } else if (waveActions[actionIndex].actionType == WaveActionType.Animation) {
            if (waveActions[actionIndex].boolAnimation) {
                waveActions[actionIndex].targetAnimator.SetBool(waveActions[actionIndex].animationName,
                    waveActions[actionIndex].boolYesNo);
            } else {
                waveActions[actionIndex].targetAnimator.SetTrigger(waveActions[actionIndex].animationName);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;

        waveCounter++;
        if (waveCounter >= currentDuration) {
            actionIndex++;
            if (actionIndex >= waveActions.Length) {
                actionIndex = 0;
                isActive = false;
            } else {
                nextAction();
            }
        }
    }
}

[System.Serializable]
public class WaveAction {
    public EnemyWave.WaveActionType actionType;
    public float duration;
    public Animator targetAnimator;
    public string animationName;
    public bool boolAnimation;
    public bool boolYesNo;
    public SliggaSlugger enemy;
}