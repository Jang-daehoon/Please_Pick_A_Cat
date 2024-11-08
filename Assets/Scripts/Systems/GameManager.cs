using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("��ȯ ������ ���� data")]
    public PlayerUnitSO[] playerUnits;

    [Header("���� ��ȯ ��ȣ�ۿ� ����")]
    [Header("Common-----------------------------------")]
    [SerializeField] private bool commUnitReady;
    [Header("Tanker------------------------------------")]
    [SerializeField] private bool tankerUnitReady;
    [Header("Melee-------------------------------------")]
    [SerializeField] private bool meleeUnitReady;
    [Header("Range------------------------------------")]
    [SerializeField] private bool rangeUnitReady;
    [Header("TowerLaser-------------------------------")]
    public bool isReloadingDone;
    public FireLaser TowerLaserObj;

    [Header("���� ��ȯ ��ƼŬ")]
    [SerializeField,Tooltip("�Ϲ����� ��ȯ ��ƼŬ")] private GameObject commonSpawnVfx;
    [SerializeField, Tooltip("��Ŀ���� ��ȯ ��ƼŬ")] private GameObject tankerSpawnVfx;
    [SerializeField, Tooltip("�������� ��ȯ ��ƼŬ")] private GameObject meleeSpawnVfx;
    [SerializeField, Tooltip("���Ÿ����� ��ȯ ��ƼŬ")] private GameObject RangeSpawnVfx;
    
    [Header("���� ����� �����Ǵ� ������Ʈ")]
    public GameObject AcensionPrefab;

    //�ݺ�ȣ�� ����
    private bool hasTriggeredVictory = false;
    private bool hasTriggeredDefeat = false;

    [Header("GameData")]
    public int[] levelUpCost;
    public string StageName;
    public int CurLevel;
    public float playeTime;
    [SerializeField] private Transform spawnPos;
    
    public GameObject GameSet;
    public CinemachineVirtualCamera mainCamera;
    public Vector3 StartCamPos = new Vector3(20, 1, -74.82741f);

    [Header("GameLevelSelect")]
    public bool isLevelEasy;
    public bool isLevelNomal;
    public bool isLevelHard;
    public GameObject levelUpNotice;
    public GameObject lackCost;

    [Header("GameLevelData")]
    public bool isEasyClear;
    public bool isNomalClear;
    public bool isHardClear;

    [Header("PlayerCastleData")]
    public float playerTowerMaxHp;
    public float playerTowerCurHp;
    private float playerCastleHpAmount { get { return playerTowerCurHp / playerTowerMaxHp; } }  //���� ���Ǵ� �׸��� ������Ƽ�� �����

    [Header("EnemyCastleData")]
    public float enemyTowerMaxHp;
    public float enemyTowerCurHp;

    private float enemyCastleHpAmount { get { return enemyTowerCurHp / enemyTowerMaxHp; } }  //���� ���Ǵ� �׸��� ������Ƽ�� �����

    [Header("CostInfo")]
    public int maxCost;  //�ִ� Cost;
    public int curCost; //���� �������� Cost
    public float incrementInterval = 0.01f; // Cost���� ����
    private float timer = 0f;
    public bool isDefeat;  //�й�
    public bool isVictory; //�¸�

    [Header("ObjectCleaner")]
    public GameObject CharacterCleaner;

    private void Awake()
    {
        Instance = this;

        LoadLevelData();
        isReloadingDone = false;
        GameSet.SetActive(false);

        //�⺻ Cost
        maxCost = 150;

        //��ȯ ��ư �ʱ�ȭ
        commUnitReady = true;
        tankerUnitReady = true;
        meleeUnitReady = true;
        rangeUnitReady = true;

        
    }
    // Start is called before the first frame update
    void Start()
    {
        //����� ���
        AudioManager.Instance.PlayBackgroundMusic(AudioManager.Instance.bgmClip);
        
        if (isLevelEasy)
        {
            StageName = "�߿��̿ձ�";
        }
        else if (isLevelNomal)
        {
            StageName = "��������̿ձ�";
        }
        else if (isLevelHard)
        {
            StageName = "��������̿ձ�";
        }
    }

    void Update()
    {
        //ü�� �� ����
        CastleHpAmount();

        //Cost ȹ��
        GetCost();

        // �¸�/�й� ���� üũ �� ó��
        GameEndState();

        // �ɼ� UI ���¿� ���� Ÿ�ӽ����� ó��
        TimeScale();

        // Ű���� �Է� ó��
        KeyboardInput();
    }
    private void GetCost()
    {
        //�÷���Ÿ��
        playeTime += Time.deltaTime;
        timer += Time.deltaTime;
        // Ÿ�̸Ӱ� ���� ������ �ʰ��ϸ� CurCost�� ������ŵ�ϴ�.
        if (timer >= incrementInterval && curCost < maxCost)
        {
            curCost++;
            timer = 0f; // Ÿ�̸� ����
        }
    }
    private void CastleHpAmount()
    {
        //ü�� �� ����
        UiManager.Instance.playerCastleHpBar.fillAmount = playerCastleHpAmount;
        UiManager.Instance.enemyCastleHpBar.fillAmount = enemyCastleHpAmount;
    }
    private void GameEndState()
    {
        if (isVictory && !hasTriggeredVictory)
        {
            hasTriggeredVictory = true;
            CharacterCleaner.SetActive(true);
            UiManager.Instance.VictoryUiSet();
            Invoke("TimeStop", 0.2f);
        }
        else if (isDefeat && !hasTriggeredDefeat)
        {
            hasTriggeredDefeat = true;
            CharacterCleaner.SetActive(true);
            UiManager.Instance.DefeatUISet();
            Invoke("TimeStop", 0.2f);
        }
    }

    private void TimeScale()
    {
        // ������ �¸� �Ǵ� �й� ���°� �ƴϰ�, ����� Ȱ��ȭ���� ���� ������ ��
        if (!isVictory && !isDefeat)
        {
            // �ɼ�, ����, ���� UI�� Ȱ��ȭ�� ���¿����� ���� �Ͻ� ����
            if (UiManager.Instance.isOptionActive ||
                UiManager.Instance.isExitUiActive ||
                UiManager.Instance.isHelpUIActive)
            {
                Time.timeScale = 0;
            }
            // ����� Ȱ��ȭ���� ���� ������ ��, ���� �ӵ��� �⺻ (1)
            else if (!UiManager.Instance.isAccelerate)
            {
                Time.timeScale = 1;
            }
            // ����� Ȱ��ȭ�� ������ ��, ���� �ӵ��� 2
            else if (UiManager.Instance.isAccelerate)
            {
                Time.timeScale = 2;
            }
        }
    }


    private void KeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && commUnitReady)
        {
            spawnCommonCat();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && tankerUnitReady)
        {
            spawnTankerCat();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && meleeUnitReady)
        {
            spawnMeleeCat();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && rangeUnitReady)
        {
            spawnRangeCat();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            WalletLevelUp();
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            TowerLaserObj.LaserFire();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            UiManager.Instance.OptionUISet();
        }
    }
    private void TimeStop()
    {
        Time.timeScale = 0;
    }
    // ���� ������̳� �� ���� ���� �� ȣ���� ���� �޼���
    public void GameStartInit()
    {
        //���� ���� �� �ʱ�ȭ
        hasTriggeredVictory = false;
        hasTriggeredDefeat = false;

        UiManager.Instance.levelUpBtn.interactable = true;
        mainCamera.transform.position = StartCamPos;
        CharacterCleaner.SetActive(false);
        isVictory = false;
        isDefeat = false;
        isReloadingDone = false;
        Time.timeScale = 1;

        playerTowerCurHp = playerTowerMaxHp;
        enemyTowerCurHp = enemyTowerMaxHp;
        curCost = 0;
        maxCost = 150;
        CurLevel = 0;
        playeTime = 0;

        //��ȯ ��ư �ʱ�ȭ
        commUnitReady = true;
        tankerUnitReady = true;
        meleeUnitReady = true;
        rangeUnitReady = true;
        UiManager.Instance.ReloadingDoneParticle.Stop();
    }

    //��ȯ
    public void spawnCommonCat()
    {
        if (curCost > playerUnits[0].UsedCost && (!isVictory || !isDefeat))
        {
            //���� ���� ȿ���� ���
            AudioManager.Instance.PlayEffectSound(AudioManager.Instance.spawnUnitClip);
            curCost -= playerUnits[0].UsedCost;

            //������Ʈ Ǯ��
            PlayerUnit commonCat = PlayerUnitPool.pool.Pop();
            commonCat.transform.SetPositionAndRotation(spawnPos.position, transform.rotation);  //��ġ, ȸ���� �־������.
            commonCat.Unitinit(playerUnits[0]);
            commonCat.ResetUnit();

            //Sprite����
            commonCat.GetComponentInChildren<SpriteRenderer>().sprite = playerUnits[0].UnitProjectile.GetComponentInChildren<SpriteRenderer>().sprite;
            //Animator����
            // ���� commonCat�� UnitProjectile�� Animator ������Ʈ�� ���� �����´�.
            Animator commonCatAnimator = commonCat.GetComponent<Animator>();
            Animator unitProjectileAnimator = playerUnits[0].UnitProjectile.GetComponent<Animator>();

            // ���� commonCatAnimator�� ���¸� unitProjectileAnimator�� ���·� ����.
            commonCatAnimator.runtimeAnimatorController = unitProjectileAnimator.runtimeAnimatorController;

            //GameObject newCommonUnit = Instantiate(playerUnits[0].UnitProjectile, spawnPos.position, transform.rotation, null); //�Ϲݼ�ȯ

            Vector2 CommonVfxPos = new Vector2(commonCat.transform.position.x - 1f, commonCat.transform.position.y + 2f);
            GameObject newSpawnParticle = Instantiate(commonSpawnVfx, CommonVfxPos, transform.rotation, null);
            Destroy(newSpawnParticle, 2f);
            StartCoroutine(UiManager.Instance.ButtonCooltime(UiManager.Instance.spawnCommonButton, UiManager.Instance.commonCoolObj, UiManager.Instance.commonCoolTimeBar, playerUnits[0].SpawnCoolTime));
            StartCoroutine(CommonUnitCooldown());
        }
        else
            lackCost.GetComponent<Animator>().SetTrigger("LackOfCost");
    }
    private IEnumerator CommonUnitCooldown()
    {
        commUnitReady = false;
        yield return new WaitForSeconds(playerUnits[0].SpawnCoolTime);
        commUnitReady = true;
    }
    public void spawnTankerCat()
    {
        if (curCost > playerUnits[1].UsedCost && (!isVictory || !isDefeat))
        {
            //���� ���� ȿ���� ���
            AudioManager.Instance.PlayEffectSound(AudioManager.Instance.spawnUnitClip);
            curCost -= playerUnits[1].UsedCost;
            //������Ʈ Ǯ��
            PlayerUnit tankerCat = PlayerUnitPool.pool.Pop();
            tankerCat.transform.SetPositionAndRotation(spawnPos.position, transform.rotation);  //��ġ, ȸ���� �־������.
            tankerCat.Unitinit(playerUnits[1]);
            tankerCat.ResetUnit();
            //Sprite����
            tankerCat.GetComponentInChildren<SpriteRenderer>().sprite = playerUnits[1].UnitProjectile.GetComponentInChildren<SpriteRenderer>().sprite;
            //Animator����
            // ���� commonCat�� UnitProjectile�� Animator ������Ʈ�� ���� �����´�.
            Animator tankerCatAnimator = tankerCat.GetComponent<Animator>();
            Animator unitProjectileAnimator = playerUnits[1].UnitProjectile.GetComponent<Animator>();

            // ���� commonCatAnimator�� ���¸� unitProjectileAnimator�� ���·� ����.
            tankerCatAnimator.runtimeAnimatorController = unitProjectileAnimator.runtimeAnimatorController;

            //GameObject newTankerUnit = Instantiate(playerUnits[1].UnitProjectile, spawnPos.position, transform.rotation, null);
            Vector2 TankerVfxPos = new Vector2(tankerCat.transform.position.x - 1f, tankerCat.transform.position.y + 2f);
            GameObject newSpawnParticle = Instantiate(tankerSpawnVfx, TankerVfxPos, transform.rotation, null);
            Destroy(newSpawnParticle, 2f);
            StartCoroutine(UiManager.Instance.ButtonCooltime(UiManager.Instance.spawnTankerButton, UiManager.Instance.tankerCoolObj, UiManager.Instance.tankerCoolTimeBar, playerUnits[1].SpawnCoolTime));
            StartCoroutine(TankerUnitCooldown());
        }
        else
            lackCost.GetComponent<Animator>().SetTrigger("LackOfCost");
    }
    private IEnumerator TankerUnitCooldown()
    {
        tankerUnitReady = false;
        yield return new WaitForSeconds(playerUnits[1].SpawnCoolTime);
        tankerUnitReady = true;
    }
    public void spawnMeleeCat()
    {
        if (curCost >= playerUnits[2].UsedCost && (!isVictory || !isDefeat))
        {
            //���� ���� ȿ���� ���
            AudioManager.Instance.PlayEffectSound(AudioManager.Instance.spawnUnitClip);
            curCost -= playerUnits[2].UsedCost;
            //������Ʈ Ǯ��
            PlayerUnit meleeUnit = PlayerUnitPool.pool.Pop();
            meleeUnit.transform.SetPositionAndRotation(spawnPos.position, transform.rotation);  //��ġ, ȸ���� �־������.
            meleeUnit.Unitinit(playerUnits[2]);
            meleeUnit.ResetUnit();
            //Sprite����
            meleeUnit.GetComponentInChildren<SpriteRenderer>().sprite = playerUnits[2].UnitProjectile.GetComponentInChildren<SpriteRenderer>().sprite;
            //Animator����
            // ���� commonCat�� UnitProjectile�� Animator ������Ʈ�� ���� �����´�.
            Animator meleeUnitAnimator = meleeUnit.GetComponent<Animator>();
            Animator unitProjectileAnimator = playerUnits[2].UnitProjectile.GetComponent<Animator>();

            // ���� commonCatAnimator�� ���¸� unitProjectileAnimator�� ���·� ����.
            meleeUnitAnimator.runtimeAnimatorController = unitProjectileAnimator.runtimeAnimatorController;


            //GameObject newMeleeUnit = Instantiate(playerUnits[2].UnitProjectile, spawnPos.position, transform.rotation, null);
            Vector2 MeleeVfxPos = new Vector2(meleeUnit.transform.position.x - 1f, meleeUnit.transform.position.y + 2f);
            GameObject newSpawnParticle = Instantiate(meleeSpawnVfx, MeleeVfxPos, transform.rotation, null);
            Destroy(newSpawnParticle, 2f);
            StartCoroutine(UiManager.Instance.ButtonCooltime(UiManager.Instance.spawnMeleeButton,UiManager.Instance.meleeCoolObj, UiManager.Instance.meleeCoolTimeBar, playerUnits[2].SpawnCoolTime));
            StartCoroutine(MeleeUnitCooldown());
        }
        else
            lackCost.GetComponent<Animator>().SetTrigger("LackOfCost");
    }
    private IEnumerator MeleeUnitCooldown()
    {
        meleeUnitReady = false;
        yield return new WaitForSeconds(playerUnits[2].SpawnCoolTime);
        meleeUnitReady = true;
    }
    public void spawnRangeCat()
    {
        if (curCost > playerUnits[3].UsedCost && (!isVictory || !isDefeat))
        {
            //���� ���� ȿ���� ���
            AudioManager.Instance.PlayEffectSound(AudioManager.Instance.spawnUnitClip);
            curCost -= playerUnits[3].UsedCost;
            //������Ʈ Ǯ��
            PlayerUnit rangeUnit = PlayerUnitPool.pool.Pop();
            rangeUnit.transform.SetPositionAndRotation(spawnPos.position, transform.rotation);  //��ġ, ȸ���� �־������.
            rangeUnit.Unitinit(playerUnits[3]);
            rangeUnit.ResetUnit();
            //Sprite����
            rangeUnit.GetComponentInChildren<SpriteRenderer>().sprite = playerUnits[3].UnitProjectile.GetComponentInChildren<SpriteRenderer>().sprite;
            //Animator����
            // ���� commonCat�� UnitProjectile�� Animator ������Ʈ�� ���� �����´�.
            Animator rangeUnitAnimator = rangeUnit.GetComponent<Animator>();
            Animator unitProjectileAnimator = playerUnits[3].UnitProjectile.GetComponent<Animator>();

            // ���� commonCatAnimator�� ���¸� unitProjectileAnimator�� ���·� ����.
            rangeUnitAnimator.runtimeAnimatorController = unitProjectileAnimator.runtimeAnimatorController;


            //GameObject newRangeUnit = Instantiate(playerUnits[3].UnitProjectile, spawnPos.position, transform.rotation, null);
            Vector2 RangeVfxPos = new Vector2(rangeUnit.transform.position.x - 1f, rangeUnit.transform.position.y + 2f);
            GameObject newSpawnParticle = Instantiate(RangeSpawnVfx, RangeVfxPos, transform.rotation, null);
            Destroy(newSpawnParticle, 2f);
            StartCoroutine(UiManager.Instance.ButtonCooltime(UiManager.Instance.spawnRangeButton, UiManager.Instance.rangeCoolObj, UiManager.Instance.rangeCoolTimeBar, playerUnits[3].SpawnCoolTime));
            StartCoroutine(RangeUnitCooldown());
        }
        else
            lackCost.GetComponent<Animator>().SetTrigger("LackOfCost");
    }
    private IEnumerator RangeUnitCooldown()
    {
        rangeUnitReady = false;
        yield return new WaitForSeconds(playerUnits[3].SpawnCoolTime);
        rangeUnitReady = true;
    }
    public void WalletLevelUp()
    {
        // �ִ� ���� üũ: CurLevel�� levelUpCost�� �ִ� �ε������� ������ Ȯ��
        if (CurLevel < levelUpCost.Length)
        {
            if (curCost >= levelUpCost[CurLevel])
            {
                //���� �� ȿ���� ���
                AudioManager.Instance.PlayEffectSound(AudioManager.Instance.levelUpClip);
                curCost -= levelUpCost[CurLevel];
                CurLevel++;
                maxCost += 80;
                levelUpNotice.GetComponent<Animator>().SetTrigger("LevelUp");
                Debug.Log($"Level�� ����߽��ϴ�. ���� Lv.{CurLevel}");
            }
            else
            {
                lackCost.GetComponent<Animator>().SetTrigger("LackOfCost");
                Debug.Log("Cost�� ������ LevelUp�� �Ұ��� �մϴ�.");
            }
        }
        else
        {
            UiManager.Instance.levelUpBtn.interactable = false;
            Debug.Log("�ִ� �����̹Ƿ� ���̻� ������ �Ұ����մϴ�.");
        }
    }


    public void SaveLevelData()
    {
        // �� ������ Ŭ���� ���θ� ����
        PlayerPrefs.SetInt("EasyLevelClear", isEasyClear ? 1 : 0);
        PlayerPrefs.SetInt("NomalLevelClear", isNomalClear ? 1 : 0);
        PlayerPrefs.SetInt("HardLevelClear", isHardClear ? 1 : 0);

        // ���� ������ ��ũ�� ����
        PlayerPrefs.Save();

        // ����� ������ Ȯ��
        Debug.Log("Level Easy: " + PlayerPrefs.GetInt("Level_Easy"));
        Debug.Log("Level Nomal: " + PlayerPrefs.GetInt("Level_Nomal"));
        Debug.Log("Level Hard: " + PlayerPrefs.GetInt("Level_Hard"));
    }
    public void LoadLevelData()
    {
        // �� ������ Ŭ���� ���θ� �ҷ���
        isEasyClear = PlayerPrefs.GetInt("EasyLevelClear", 0) == 1;
        isNomalClear = PlayerPrefs.GetInt("NomalLevelClear", 0) == 1;
        isHardClear = PlayerPrefs.GetInt("HardLevelClear", 0) == 1;
    }
}
