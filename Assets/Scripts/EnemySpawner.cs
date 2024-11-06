using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyUnitSO[] enemyUnitSo;
    [SerializeField] private Transform spawnPos;

    public float[] incrementInterval; // Cost ���� ����
    public float selectIncrementInterval;   // ������ ���̵��� ���� Cost ����
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

    [Header("���� ��ȯ ��ƼŬ")]
    [SerializeField, Tooltip("Common Enemy ��ȯ ��ƼŬ")] private GameObject commonSpawnVfx;
    [SerializeField, Tooltip("Elite Enemy ��ȯ ��ƼŬ")] private GameObject eliteSpawnVfx;
    [SerializeField, Tooltip("Boss Enemy ��ȯ ��ƼŬ")] private GameObject bossSpawnVfx;


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

        // ��� ����
        if (timer >= selectIncrementInterval && curCost < selectMaxCost)
        {
            curCost++;
            timer = 0f;
        }

        enemyCommonTimer += Time.deltaTime;
        enemyEliteTimer += Time.deltaTime;
        enemyBossTimer += Time.deltaTime;

        // Easy ���̵������� �ַ� Common�� ��ȯ�ϰ�, 30% Ȯ���� Elite�� ��ȯ
        if (GameManager.Instance.isLevelEasy)
        {
            if (curCost >= enemyUnitSo[0].usedCost && enemyCommonTimer >= enemyCommonInterval)  // ��ȯ�Ϸ��� ������ usedCost�� curCost���� �۰ų� ������ ��ȯ
            {
                enemyCommonTimer = 0;
                SpawnCommonUnit();
            }
            else if (enemyEliteTimer >= enemyEliteInterval)
            {
                if (Random.Range(0f, 1f) < 0.2f && curCost >= enemyUnitSo[1].usedCost) // 20% Ȯ���� Elite ��ȯ
                {
                    enemyEliteTimer = 0;
                    SpawnEliteUnit();
                }
            }
        }

        // Normal ���̵������� Common�� Elite�� ��� ��ȯ�ϰ�, ���� Ȯ���� Boss�� ��ȯ
        if (GameManager.Instance.isLevelNomal)
        {
            if (curCost >= enemyUnitSo[0].usedCost && enemyCommonTimer >= enemyCommonInterval) // Common ���� ��ȯ
            {
                enemyCommonTimer = 0;
                SpawnCommonUnit();
            }
            else if (enemyEliteTimer >= enemyEliteInterval)
            {
                if (curCost >= enemyUnitSo[1].usedCost) // Elite ���� ��ȯ
                {
                    enemyEliteTimer = 0;
                    SpawnEliteUnit();
                }
            }
            else if (enemyBossTimer >= enemyBossInterval)
            {
                if (Random.Range(0f, 1f) < 0.2f && curCost >= enemyUnitSo[2].usedCost) // 20% Ȯ���� Boss ��ȯ
                {
                    enemyBossTimer = 0;
                    SpawnBossUnit();
                }
            }
        }

        // Hard ���̵������� Cost�� ����� ���̸� ��� Boss�� ��ȯ
        if (GameManager.Instance.isLevelHard)
        {
            if (curCost >= enemyUnitSo[2].usedCost && enemyBossTimer >= enemyBossInterval)
            {
                SpawnBossUnit();
                enemyBossTimer = 0; // Boss ��ȯ �� Boss Ÿ�̸� ����
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
        //�ʱ�ȭ
        commonEnemyUnit.UnitInit(enemyUnitSo[0]);
        commonEnemyUnit.ResetUnit();
        //Sprite����
        commonEnemyUnit.spriteRenderer.sprite = enemyUnitSo[0].unitProjectile.GetComponentInChildren<SpriteRenderer>().sprite;
        
        //Animator����
        // ���� commonCat�� UnitProjectile�� Animator ������Ʈ�� ���� �����´�.
        Animator enemyAnimator = commonEnemyUnit.GetComponent<Animator>();
        Animator unitProjectileAnimator = enemyUnitSo[0].unitProjectile.GetComponent<Animator>();

        // ���� commonCatAnimator�� ���¸� unitProjectileAnimator�� ���·� ����.
        enemyAnimator.runtimeAnimatorController = unitProjectileAnimator.runtimeAnimatorController;

        Vector2 spawnVfxPos = new Vector2(spawnPos.position.x - 1f, spawnPos.position.y + 2f);
        GameObject newSpawnParticle = Instantiate(commonSpawnVfx, spawnVfxPos, transform.rotation, null);
        Destroy(newSpawnParticle, 2f);

        curCost -= Mathf.RoundToInt(enemyUnitSo[0].usedCost);  // ���� ��ȯ �� ��� ����
    }

    private void SpawnEliteUnit()
    {
        //Instantiate(enemyUnitSo[1].unitProjectile, spawnPos.position, transform.rotation, null);
        EnemyUnit eliteEnemyUnit = EnemyUnitPool.pool.Pop();
        eliteEnemyUnit.transform.SetPositionAndRotation(spawnPos.position, transform.rotation);
        eliteEnemyUnit.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        //�ʱ�ȭ
        eliteEnemyUnit.UnitInit(enemyUnitSo[1]);
        eliteEnemyUnit.ResetUnit();
        //Sprite����
        eliteEnemyUnit.spriteRenderer.sprite = enemyUnitSo[1].unitProjectile.GetComponentInChildren<SpriteRenderer>().sprite;
        eliteEnemyUnit.spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        //Animator����
        // ���� commonCat�� UnitProjectile�� Animator ������Ʈ�� ���� �����´�.
        Animator enemyAnimator = eliteEnemyUnit.GetComponent<Animator>();
        Animator unitProjectileAnimator = enemyUnitSo[1].unitProjectile.GetComponent<Animator>();

        // ���� commonCatAnimator�� ���¸� unitProjectileAnimator�� ���·� ����.
        enemyAnimator.runtimeAnimatorController = unitProjectileAnimator.runtimeAnimatorController;

        Vector2 spawnVfxPos = new Vector2(spawnPos.position.x - 1f, spawnPos.position.y + 2f);
        GameObject newSpawnParticle = Instantiate(eliteSpawnVfx, spawnVfxPos, transform.rotation, null);
        Destroy(newSpawnParticle, 2f);

        curCost -= Mathf.RoundToInt(enemyUnitSo[1].usedCost);  // ���� ��ȯ �� ��� ����
    }

    private void SpawnBossUnit()
    {
        //Instantiate(enemyUnitSo[2].unitProjectile, spawnPos.position, transform.rotation, null);
        EnemyUnit bossEnemyUnit = EnemyUnitPool.pool.Pop();
        bossEnemyUnit.transform.SetPositionAndRotation(spawnPos.position, transform.rotation);
        bossEnemyUnit.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        //�ʱ�ȭ
        bossEnemyUnit.UnitInit(enemyUnitSo[2]);
        bossEnemyUnit.ResetUnit();
        //Sprite����
        bossEnemyUnit.spriteRenderer.sprite = enemyUnitSo[2].unitProjectile.GetComponentInChildren<SpriteRenderer>().sprite;
        //Animator����
        // ���� commonCat�� UnitProjectile�� Animator ������Ʈ�� ���� �����´�.
        Animator enemyAnimator = bossEnemyUnit.GetComponent<Animator>();
        Animator unitProjectileAnimator = enemyUnitSo[2].unitProjectile.GetComponent<Animator>();

        // ���� commonCatAnimator�� ���¸� unitProjectileAnimator�� ���·� ����.
        enemyAnimator.runtimeAnimatorController = unitProjectileAnimator.runtimeAnimatorController;

        Vector2 spawnVfxPos = new Vector2(spawnPos.position.x - 1f, spawnPos.position.y + 2f);
        GameObject newSpawnParticle = Instantiate(bossSpawnVfx, spawnVfxPos, transform.rotation, null);
        Destroy(newSpawnParticle, 2f);

        curCost -= Mathf.RoundToInt(enemyUnitSo[2].usedCost);  // ���� ��ȯ �� ��� ����
    }
}
