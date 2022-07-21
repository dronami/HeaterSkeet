using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitObject : MonoBehaviour
{
    public enum DestructionAction {
        Explode,
        Animation
    }

    public bool isEnemy;
    public DestructionAction destructionAction;
    public FXManager.FXType explosionType;

    public Flasher flasher;
    public int maxHP;

    private int currentHP;

    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
    }

    public void getHit(int damage) {
        currentHP -= damage;
        if (currentHP <= 0) {
            onHitObjectDestroyed();
        } else {
            flasher.startFlashing();
        }
    }

    protected virtual void onHitObjectDestroyed() {
        gameObject.SetActive(false);
        if (destructionAction == DestructionAction.Explode) {
            InternalShit.fxManager.initializeFX(explosionType, transform.position);
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
