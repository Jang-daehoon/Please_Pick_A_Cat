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

    [Header("���� ��ȯ ��ƼŬ")]
    [SerializeField, Tooltip("Common Enemy ��ȯ ��ƼŬ")] private GameObject commonSpawnVfx;
    [SerializeField, Tooltip("Elite Ememy ��ȯ ��ƼŬ")] private GameObject tankerSpawnVfx;
    [SerializeField, Tooltip("Boss Enemy ��ȯ ��ƼŬ")] private GameObject meleeSpawnVfx;
    private void Start()
    {
        //�Ϲ� ���� �ٷ� ��ȯ
        Instantiate(enemyUnitSo[0].unitProjectile, spawnPos.position, transform.rotation, null);
    }
    // Update is called once per frame
    void Update()
    {
        enemyCommonTimer +=Time.deltaTime;
        enemyEliteTimer+=Time.deltaTime;
        //���������� tower curHp�� 50%���ϸ� 20�ʿ� 1������ ��ȯ
        enemyBossTimer+=Time.deltaTime;

        if(enemyCommonTimer >= enemyCommonInterval)
        {
            //�� �Ϲ����� ��ȯ
            Debug.Log("�� �Ϲ����� ��ȯ");
            enemyCommonTimer = 0;
            Instantiate(enemyUnitSo[0].unitProjectile, spawnPos.position, transform.rotation, null);
            Vector2 CommonVfxPos = new Vector2(spawnPos.transform.position.x - 1f, spawnPos.transform.position.y + 2f);
            GameObject newSpawnParticle = Instantiate(commonSpawnVfx, CommonVfxPos, transform.rotation, null);
            Destroy(newSpawnParticle, 2f);
        }
        if(GameManager.Instance.playeTime >= 60 && enemyEliteTimer >= enemyEliteInterval)
        {
            Debug.Log("�߰� ���� ��ȯ");
            enemyEliteTimer = 0;
            //�߰� ���� ��ȯ
            Instantiate(enemyUnitSo[1].unitProjectile, spawnPos.position, transform.rotation, null);
            Vector2 EliteVfxPos = new Vector2(spawnPos.transform.position.x - 1f, spawnPos.transform.position.y + 2f);
            GameObject newSpawnParticle = Instantiate(commonSpawnVfx, EliteVfxPos, transform.rotation, null);
            Destroy(newSpawnParticle, 2f);
        }
        if(GameManager.Instance.enemyTowerCurHp<=500 && enemyBossTimer >= enemyBossInterval)
        {
            Debug.Log("Boss���� ��ȯ");
            enemyBossTimer = 0;
            //Boss���� ��ȯ
            Instantiate(enemyUnitSo[2].unitProjectile, spawnPos.position, transform.rotation, null);
            Vector2 BossVfxPos = new Vector2(spawnPos.transform.position.x - 1f, spawnPos.transform.position.y + 2f);
            GameObject newSpawnParticle = Instantiate(commonSpawnVfx, BossVfxPos, transform.rotation, null);
            Destroy(newSpawnParticle, 2f);
        }
        

    }
}
