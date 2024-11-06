using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyUnitSO[] enemyUnitSo;
    [SerializeField] private Transform spawnPos;

    public float[] incrementInterval; // Cost 증가 간격
    public float selectIncrementInterval;   // 선택한 난이도에 따른 Cost 증가
    private float timer;

    [SerializeField] private int curCost;
    [SerializeField] private int[] levelMaxCost; // 0 = Easy, 1 = Normal, 2 = Hard
    [SerializeField] private int selectMaxCost;
    float enemyCommonTimer;
    [SerializeField] private float enemyCommonInterval;

    float enemyEliteTimer;
    [SerializeField] private float enemyEliteInterval;

    float enemyBossTimer;
    [SerializeField] private float enemyBossInterval;

    [Header("유닛 소환 파티클")]
    [SerializeField, Tooltip("Common Enemy 소환 파티클")] private GameObject commonSpawnVfx;
    [SerializeField, Tooltip("Elite Enemy 소환 파티클")] private GameObject eliteSpawnVfx;
    [SerializeField, Tooltip("Boss Enemy 소환 파티클")] private GameObject bossSpawnVfx;


    private void OnEnable()
    {
        enemyCommonTimer = 0;
        enemyEliteTimer = 0;
        enemyBossTimer = 0;
        curCost = 0;

        if (GameManager.Instance.isLevelEasy)
        {
            selectMaxCost = levelMaxCost[0];
            selectIncrementInterval = incrementInterval[0];
        }
        else if (GameManager.Instance.isLevelNomal)
        {
            selectMaxCost = levelMaxCost[1];
            selectIncrementInterval = incrementInterval[1];
        }
        else if (GameManager.Instance.isLevelHard)
        {
            selectMaxCost = levelMaxCost[2];
            selectIncrementInterval = incrementInterval[2];
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        // 비용 증가
        if (timer >= selectIncrementInterval && curCost < selectMaxCost)
        {
            curCost++;
            timer = 0f;
        }

        enemyCommonTimer += Time.deltaTime;
        enemyEliteTimer += Time.deltaTime;
        enemyBossTimer += Time.deltaTime;

        // Easy 난이도에서는 주로 Common을 소환하고, 30% 확률로 Elite를 소환
        if (GameManager.Instance.isLevelEasy)
        {
            if (curCost >= enemyUnitSo[0].usedCost && enemyCommonTimer >= enemyCommonInterval)  // 소환하려는 유닛의 usedCost가 curCost보다 작거나 같으면 소환
            {
                enemyCommonTimer = 0;
                SpawnCommonUnit();
            }
            else if (enemyEliteTimer >= enemyEliteInterval)
            {
                if (Random.Range(0f, 1f) < 0.2f && curCost >= enemyUnitSo[1].usedCost) // 20% 확률로 Elite 소환
                {
                    enemyEliteTimer = 0;
                    SpawnEliteUnit();
                }
            }
        }

        // Normal 난이도에서는 Common과 Elite를 섞어서 소환하고, 낮은 확률로 Boss를 소환
        if (GameManager.Instance.isLevelNomal)
        {
            if (curCost >= enemyUnitSo[0].usedCost && enemyCommonTimer >= enemyCommonInterval) // Common 유닛 소환
            {
                enemyCommonTimer = 0;
                SpawnCommonUnit();
            }
            else if (enemyEliteTimer >= enemyEliteInterval)
            {
                if (curCost >= enemyUnitSo[1].usedCost) // Elite 유닛 소환
                {
                    enemyEliteTimer = 0;
                    SpawnEliteUnit();
                }
            }
            else if (enemyBossTimer >= enemyBossInterval)
            {
                if (Random.Range(0f, 1f) < 0.2f && curCost >= enemyUnitSo[2].usedCost) // 20% 확률로 Boss 소환
                {
                    enemyBossTimer = 0;
                    SpawnBossUnit();
                }
            }
        }

        // Hard 난이도에서는 Cost가 충분히 쌓이면 즉시 Boss를 소환
        if (GameManager.Instance.isLevelHard)
        {
            if (curCost >= enemyUnitSo[2].usedCost && enemyBossTimer >= enemyBossInterval)
            {
                SpawnBossUnit();
                enemyBossTimer = 0; // Boss 소환 후 Boss 타이머 리셋
            }
            else if (curCost >= enemyUnitSo[0].usedCost && enemyCommonTimer >= enemyCommonInterval)
            {
                enemyCommonTimer = 0;
                SpawnCommonUnit();
            }
            else if (curCost >= enemyUnitSo[1].usedCost && enemyEliteTimer >= enemyEliteInterval)
            {
                enemyEliteTimer = 0;
                SpawnEliteUnit();
            }
        }
    }


    private void SpawnCommonUnit()
    {
        //Instantiate(enemyUnitSo[0].unitProjectile, spawnPos.position, transform.rotation, null);
        EnemyUnit commonEnemyUnit = EnemyUnitPool.pool.Pop();
        commonEnemyUnit.transform.SetPositionAndRotation(spawnPos.position, transform.rotation);
        commonEnemyUnit.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        //초기화
        commonEnemyUnit.UnitInit(enemyUnitSo[0]);
        commonEnemyUnit.ResetUnit();
        //Sprite변경
        commonEnemyUnit.spriteRenderer.sprite = enemyUnitSo[0].unitProjectile.GetComponentInChildren<SpriteRenderer>().sprite;
        
        //Animator변경
        // 먼저 commonCat과 UnitProjectile의 Animator 컴포넌트를 각각 가져온다.
        Animator enemyAnimator = commonEnemyUnit.GetComponent<Animator>();
        Animator unitProjectileAnimator = enemyUnitSo[0].unitProjectile.GetComponent<Animator>();

        // 이제 commonCatAnimator의 상태를 unitProjectileAnimator의 상태로 변경.
        enemyAnimator.runtimeAnimatorController = unitProjectileAnimator.runtimeAnimatorController;

        Vector2 spawnVfxPos = new Vector2(spawnPos.position.x - 1f, spawnPos.position.y + 2f);
        GameObject newSpawnParticle = Instantiate(commonSpawnVfx, spawnVfxPos, transform.rotation, null);
        Destroy(newSpawnParticle, 2f);

        curCost -= Mathf.RoundToInt(enemyUnitSo[0].usedCost);  // 유닛 소환 후 비용 차감
    }

    private void SpawnEliteUnit()
    {
        //Instantiate(enemyUnitSo[1].unitProjectile, spawnPos.position, transform.rotation, null);
        EnemyUnit eliteEnemyUnit = EnemyUnitPool.pool.Pop();
        eliteEnemyUnit.transform.SetPositionAndRotation(spawnPos.position, transform.rotation);
        eliteEnemyUnit.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        //초기화
        eliteEnemyUnit.UnitInit(enemyUnitSo[1]);
        eliteEnemyUnit.ResetUnit();
        //Sprite변경
        eliteEnemyUnit.spriteRenderer.sprite = enemyUnitSo[1].unitProjectile.GetComponentInChildren<SpriteRenderer>().sprite;
        eliteEnemyUnit.spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        //Animator변경
        // 먼저 commonCat과 UnitProjectile의 Animator 컴포넌트를 각각 가져온다.
        Animator enemyAnimator = eliteEnemyUnit.GetComponent<Animator>();
        Animator unitProjectileAnimator = enemyUnitSo[1].unitProjectile.GetComponent<Animator>();

        // 이제 commonCatAnimator의 상태를 unitProjectileAnimator의 상태로 변경.
        enemyAnimator.runtimeAnimatorController = unitProjectileAnimator.runtimeAnimatorController;

        Vector2 spawnVfxPos = new Vector2(spawnPos.position.x - 1f, spawnPos.position.y + 2f);
        GameObject newSpawnParticle = Instantiate(eliteSpawnVfx, spawnVfxPos, transform.rotation, null);
        Destroy(newSpawnParticle, 2f);

        curCost -= Mathf.RoundToInt(enemyUnitSo[1].usedCost);  // 유닛 소환 후 비용 차감
    }

    private void SpawnBossUnit()
    {
        //Instantiate(enemyUnitSo[2].unitProjectile, spawnPos.position, transform.rotation, null);
        EnemyUnit bossEnemyUnit = EnemyUnitPool.pool.Pop();
        bossEnemyUnit.transform.SetPositionAndRotation(spawnPos.position, transform.rotation);
        bossEnemyUnit.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        //초기화
        bossEnemyUnit.UnitInit(enemyUnitSo[2]);
        bossEnemyUnit.ResetUnit();
        //Sprite변경
        bossEnemyUnit.spriteRenderer.sprite = enemyUnitSo[2].unitProjectile.GetComponentInChildren<SpriteRenderer>().sprite;
        //Animator변경
        // 먼저 commonCat과 UnitProjectile의 Animator 컴포넌트를 각각 가져온다.
        Animator enemyAnimator = bossEnemyUnit.GetComponent<Animator>();
        Animator unitProjectileAnimator = enemyUnitSo[2].unitProjectile.GetComponent<Animator>();

        // 이제 commonCatAnimator의 상태를 unitProjectileAnimator의 상태로 변경.
        enemyAnimator.runtimeAnimatorController = unitProjectileAnimator.runtimeAnimatorController;

        Vector2 spawnVfxPos = new Vector2(spawnPos.position.x - 1f, spawnPos.position.y + 2f);
        GameObject newSpawnParticle = Instantiate(bossSpawnVfx, spawnVfxPos, transform.rotation, null);
        Destroy(newSpawnParticle, 2f);

        curCost -= Mathf.RoundToInt(enemyUnitSo[2].usedCost);  // 유닛 소환 후 비용 차감
    }
}
