using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public enum BulletType {
        PlayerBasic,
        EnemyBasic,
        RealBullet,

        Count
    }

    public FXManager fxManager;
    public Transform bulletDaddy;
    public Transform reticleDaddy;

    private int[] currentBullets;
    private Bullet[][] bullets;
    private int doneCount;
    private int currentDone = 0;
    private int[] doneIndexes;
    private HashSet<int>[] activeBulletIndexes;

    private int currentReticle = 0;
    private GameObject[] reticles;

    // Start is called before the first frame update
    void Start()
    {
        currentBullets = new int[(int)BulletType.Count];
        bullets = new Bullet[(int)BulletType.Count][];
        activeBulletIndexes = new HashSet<int>[(int)BulletType.Count];

        currentReticle = 0;
        reticles = new GameObject[reticleDaddy.childCount];

        for (int r = 0; r < reticleDaddy.childCount; r++) {
            reticles[r] = reticleDaddy.GetChild(r).gameObject;
        }

        int maxCount = 0;
        for (int t = 0; t < (int)BulletType.Count; t++) {
            maxCount = Mathf.Max(maxCount, bulletDaddy.GetChild(t).childCount);
            bullets[t] = new Bullet[bulletDaddy.GetChild(t).childCount];
            activeBulletIndexes[t] = new HashSet<int>();
            currentBullets[t] = 0;

            for (int b = 0; b < bulletDaddy.GetChild(t).childCount; b++) {
                bullets[t][b] = bulletDaddy.GetChild(t).GetChild(b).GetComponent<Bullet>();
            }
        }
        doneIndexes = new int[maxCount];
    }

    public void initializeBullet(BulletType bType, Vector3 startPos, Vector3 endPos) {
        int bulletIndexStart = currentBullets[(int)bType];
        int typeIndex = (int)bType;

        while (bullets[typeIndex][currentBullets[typeIndex]].bulletActive) {
            currentBullets[typeIndex]++;
            if (currentBullets[typeIndex] >= bullets[typeIndex].Length) {
                currentBullets[typeIndex] = 0;
            } else if (currentBullets[typeIndex] == bulletIndexStart) {
                Debug.LogError("OUTTA BULLETS!!!");
                return;
            }
        }
        bullets[typeIndex][currentBullets[typeIndex]].gameObject.SetActive(true);
        bullets[typeIndex][currentBullets[typeIndex]].initializeBullet(startPos, endPos);
        activeBulletIndexes[typeIndex].Add(currentBullets[typeIndex]);

        if (bType == BulletType.EnemyBasic) {
            int reticleIndexStart = currentReticle;

            while (reticles[currentReticle].activeSelf) {
                currentReticle++;
                if (currentReticle >= reticles.Length) {
                    currentReticle = 0;
                } else if (currentReticle == reticleIndexStart) {
                    Debug.LogError("OUTTA RETICLES!!!");
                    return;
                }
            }

            reticles[currentReticle].SetActive(true);
            reticles[currentReticle].transform.position = endPos;

            bullets[typeIndex][currentBullets[typeIndex]].reticleIndex = currentReticle;

            currentReticle++;
            if (currentReticle >= reticles.Length) {
                currentReticle = 0;
            }
        }

        currentBullets[typeIndex]++;
        if (currentBullets[typeIndex] >= bullets[typeIndex].Length) {
            currentBullets[typeIndex] = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int t = 0; t < (int)BulletType.Count; t++) {
            doneCount = 0;
            currentDone = 0;

            foreach (int bulletIndex in activeBulletIndexes[t]) {
                if (bullets[t][bulletIndex].updateBullet()) {
                    bullets[t][bulletIndex].gameObject.SetActive(false);
                    doneIndexes[currentDone] = bulletIndex;
                    doneCount++;

                    if ((BulletType)t == BulletType.EnemyBasic) {
                        reticleDaddy.GetChild(bullets[t][bulletIndex].reticleIndex).gameObject.SetActive(false);
                    }

                    fxManager.initializeFX(FXManager.FXType.BulletHitFX, bullets[t][bulletIndex].transform.localPosition);
                }
            }

            for (int d = 0; d < doneCount; d++) {
                activeBulletIndexes[t].Remove(doneIndexes[d]);
            }
        }
    }
}
