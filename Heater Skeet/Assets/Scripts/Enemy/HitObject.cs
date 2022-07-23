using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitObject : MonoBehaviour
{
    public enum DestructionAction {
        Explode,
        Animation,
        SplurtNShrink
    }

    public bool isActive = true;
    public bool isEnemy;
    public bool hideOnDestroyed = false;
    public bool explodeOnDestroyed = false;
    public DestructionAction destructionAction;
    public FXManager.FXType explosionType;

    public Sligga enemyDaddy;
    public Flasher flasher;
    public int maxHP;

    private int currentHP;

    public GameObject[] hurtFX;
    public Transform[] zeroSetters;

    // Start is called before the first frame update
    void Start()
    {
        resetHitObject();
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
        isActive = false;
        if (hideOnDestroyed) {
            gameObject.SetActive(false);
        }

        if (explodeOnDestroyed) {
            InternalShit.fxManager.initializeFX(explosionType, transform.position);
        }

        for (int h = 0; h < hurtFX.Length; h++) {
            hurtFX[h].gameObject.SetActive(true);
        }

        for (int z = 0; z < zeroSetters.Length; z++) {
            zeroSetters[z].localScale = Vector3.zero;
        }

        if (enemyDaddy != null) {
            enemyDaddy.startDying();
        }

        /*
        if (destructionAction == DestructionAction.Explode) {
            InternalShit.fxManager.initializeFX(explosionType, transform.position);
            gameObject.SetActive(false);
        }
        
        if (destructionAction == DestructionAction.SplurtNShrink) {
            
        }
        */
    }

    public void resetHitObject() {
        isActive = true;
        currentHP = maxHP;

        for (int h = 0; h < hurtFX.Length; h++) {
            hurtFX[h].gameObject.SetActive(false);
        }

        for (int z = 0; z < zeroSetters.Length; z++) {
            zeroSetters[z].gameObject.SetActive(true);
            zeroSetters[z].localScale = Vector3.one;
        }
    }

    public void disableHitObject() {
        for (int h = 0; h < hurtFX.Length; h++) {
            hurtFX[h].gameObject.SetActive(false);
        }

        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
