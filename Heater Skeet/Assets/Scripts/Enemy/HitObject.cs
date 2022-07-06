using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitObject : MonoBehaviour
{
    public Flasher flasher;
    public int maxHP;
    public FXManager fxManager;

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
        fxManager.initializeFX(FXManager.FXType.CrateExplode, transform.localPosition);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
