using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyUnitSO[] enemyUnitSo;
    [SerializeField] private Transform spawnPos;

    float enemyCommonTimer;
    [SerializeField] private float enemyCommonInterval;

    float enemyEliteTimer;
    [SerializeField] private float enemyEliteInterval;

    float enemyBossTimer;
    [SerializeField] private float enemyBossInterval;

    [Header("유닛 소환 파티클")]
    [SerializeField, Tooltip("Common Enemy 소환 파티클")] private GameObject commonSpawnVfx;
    [SerializeField, Tooltip("Elite Ememy 소환 파티클")] private GameObject tankerSpawnVfx;
    [SerializeField, Tooltip("Boss Enemy 소환 파티클")] private GameObject meleeSpawnVfx;
    private void Start()
    {
        //일반 유닛 바로 소환
        Instantiate(enemyUnitSo[0].unitProjectile, spawnPos.position, transform.rotation, null);
    }
    // Update is called once per frame
    void Update()
    {
        enemyCommonTimer +=Time.deltaTime;
        enemyEliteTimer+=Time.deltaTime;
        //보스조건은 tower curHp가 50%이하면 20초에 1마리씩 소환
        enemyBossTimer+=Time.deltaTime;

        if(enemyCommonTimer >= enemyCommonInterval)
        {
            //적 일반유닛 소환
            Debug.Log("적 일반유닛 소환");
            enemyCommonTimer = 0;
            Instantiate(enemyUnitSo[0].unitProjectile, spawnPos.position, transform.rotation, null);
            Vector2 CommonVfxPos = new Vector2(spawnPos.transform.position.x - 1f, spawnPos.transform.position.y + 2f);
            GameObject newSpawnParticle = Instantiate(commonSpawnVfx, CommonVfxPos, transform.rotation, null);
            Destroy(newSpawnParticle, 2f);
        }
        if(GameManager.Instance.playeTime >= 60 && enemyEliteTimer >= enemyEliteInterval)
        {
            Debug.Log("중간 유닛 소환");
            enemyEliteTimer = 0;
            //중간 유닛 소환
            Instantiate(enemyUnitSo[1].unitProjectile, spawnPos.position, transform.rotation, null);
            Vector2 EliteVfxPos = new Vector2(spawnPos.transform.position.x - 1f, spawnPos.transform.position.y + 2f);
            GameObject newSpawnParticle = Instantiate(commonSpawnVfx, EliteVfxPos, transform.rotation, null);
            Destroy(newSpawnParticle, 2f);
        }
        if(GameManager.Instance.enemyTowerCurHp<=500 && enemyBossTimer >= enemyBossInterval)
        {
            Debug.Log("Boss유닛 소환");
            enemyBossTimer = 0;
            //Boss유닛 소환
            Instantiate(enemyUnitSo[2].unitProjectile, spawnPos.position, transform.rotation, null);
            Vector2 BossVfxPos = new Vector2(spawnPos.transform.position.x - 1f, spawnPos.transform.position.y + 2f);
            GameObject newSpawnParticle = Instantiate(commonSpawnVfx, BossVfxPos, transform.rotation, null);
            Destroy(newSpawnParticle, 2f);
        }
        

    }
}
