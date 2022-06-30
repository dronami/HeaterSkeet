using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public FXManager fxManager;
    public Transform bulletDaddy;

    private int currentBullet = 0;
    private Bullet[] bullets;
    private int doneCount;
    private int currentDone = 0;
    private int[] doneIndexes;
    private HashSet<int> activeBulletIndexes = new HashSet<int>();

    // Start is called before the first frame update
    void Start()
    {
        bullets = new Bullet[bulletDaddy.childCount];
        doneIndexes = new int[bulletDaddy.childCount];
        for (int b = 0; b < bulletDaddy.childCount; b++) {
            bullets[b] = bulletDaddy.GetChild(b).GetComponent<Bullet>();
        }
    }

    public void initializeBullet(Vector3 startPos, Vector3 endPos) {
        int bulletIndexStart = currentBullet;
        while (bullets[currentBullet].bulletActive) {
            currentBullet++;
            if (currentBullet >= bullets.Length) {
                currentBullet = 0;
            } else if (currentBullet == bulletIndexStart) {
                Debug.LogError("OUTTA BULLETS!!!");
                return;
            }
        }
        bullets[currentBullet].gameObject.SetActive(true);
        bullets[currentBullet].initializeBullet(startPos, endPos);
        activeBulletIndexes.Add(currentBullet);
        currentBullet++;
        if (currentBullet >= bullets.Length) {
            currentBullet = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        doneCount = 0;
        currentDone = 0;

        foreach(int bulletIndex in activeBulletIndexes) {
            if (bullets[bulletIndex].updateBullet()) {
                bullets[bulletIndex].gameObject.SetActive(false);
                doneIndexes[currentDone] = bulletIndex;
                doneCount++;

                fxManager.initializeFX(bullets[bulletIndex].transform.localPosition);
            }
        }

        for (int d = 0; d < doneCount; d++) {
            activeBulletIndexes.Remove(doneIndexes[d]);
        }
    }
}
