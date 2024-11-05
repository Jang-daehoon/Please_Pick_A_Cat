using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("��ȯ ������ ���� data")]
    public PlayerUnitSO[] playerUnits;

    [Header("���� �󿡼� ��ȣ�ۿ� �ϴ� ��ư, ��Ÿ�� ���ӿ�����Ʈ �� �̹���")]
    [SerializeField] private Button levelUpBtn;
    [SerializeField] private Button laserFireBtn;
    [Header("Common-----------------------------------")]
    [SerializeField] private Button spawnCommonButton;
    [SerializeField] private GameObject commonCoolObj;
    [SerializeField] private Image commonCoolTimeBar;
    [SerializeField] private bool commUnitReady;
    [Header("Tanker------------------------------------")]
    [SerializeField] private Button spawnTankerButton;
    [SerializeField] private GameObject tankerCoolObj;
    [SerializeField] private Image tankerCoolTimeBar;
    [SerializeField] private bool tankerUnitReady;
    [Header("Melee-------------------------------------")]
    [SerializeField] private Button spawnMeleeButton;
    [SerializeField] private GameObject meleeCoolObj;
    [SerializeField] private Image meleeCoolTimeBar;
    [SerializeField] private bool meleeUnitReady;
    [Header("Range------------------------------------")]
    [SerializeField] private Button spawnRangeButton;
    [SerializeField] private GameObject rangeCoolObj;
    [SerializeField] private Image rangeCoolTimeBar;
    [SerializeField] private bool rangeUnitReady;
    [Header("TowerLaser-------------------------------")]
    [SerializeField] private Image ReloadingImage;
    [SerializeField] private GameObject ReadyText;
    [SerializeField] private bool isReloadingDone;
    [SerializeField] private GameObject TowerLaserObj;
    [SerializeField] private GameObject TowerReloadParticle;
    [Header("���� ��ȯ ��ƼŬ")]
    [SerializeField,Tooltip("�Ϲ����� ��ȯ ��ƼŬ")] private GameObject commonSpawnVfx;
    [SerializeField, Tooltip("��Ŀ���� ��ȯ ��ƼŬ")] private GameObject tankerSpawnVfx;
    [SerializeField, Tooltip("�������� ��ȯ ��ƼŬ")] private GameObject meleeSpawnVfx;
    [SerializeField, Tooltip("���Ÿ����� ��ȯ ��ƼŬ")] private GameObject RangeSpawnVfx;
    public GameObject AcensionPrefab;

    [Header("GameData")]
    public int[] levelUpCost;
    public string StageName;
    public int CurLevel;
    public float playeTime;
    [SerializeField] private Transform spawnPos;
    [SerializeField]private float LaserCoolTime = 48;
    [SerializeField]private float LaserCurTime = 0;
    private bool isReloading = false; // �ڷ�ƾ ���� ������ Ȯ���ϴ� ����
    public GameObject GameSet;

    [Header("GameLevelSelect")]
    public bool isLevelEasy;
    public bool isLevelNomal;
    public bool isLevelHard;

    [Header("GameLevelData")]
    public bool isEasyClear = false;
    public bool isNomalClear = false;
    public bool isHardClear = false;

    [Header("PlayerCastleData")]
    public float playerTowerMaxHp;
    public float playerTowerCurHp;
    private float playerCastleHpAmount { get { return playerTowerCurHp / playerTowerMaxHp; } }  //���� ���Ǵ� �׸��� ������Ƽ�� �����
    [Header("PlayerCastleUI")]
    [SerializeField] private Image playerCastleHpBar;

    [Header("EnemyCastleData")]
    public float enemyTowerMaxHp;
    public float enemyTowerCurHp;
    [Header("EnemyCastleUI")]
    [SerializeField] private Image enemyCastleHpBar;
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
        LaserCoolTime = 48;
        isReloadingDone = false;
        GameSet.SetActive(false);

        //�⺻ Cost
        maxCost = 150;

        //��ȯ ��ư �ʱ�ȭ
        commUnitReady = true;
        tankerUnitReady = true;
        meleeUnitReady = true;
        rangeUnitReady = true;

        //��ȯ
        spawnCommonButton.onClick.AddListener(spawnCommonCat);
        spawnTankerButton.onClick.AddListener(spawnTankerCat);
        spawnMeleeButton.onClick.AddListener(spawnMeleeCat);
        spawnRangeButton.onClick.AddListener(spawnRangeCat);

        levelUpBtn.onClick.AddListener(WalletLevelUp);
        laserFireBtn.onClick.AddListener(LaserFire);
        
    }
    // Start is called before the first frame update
    void Start()
    {
        StageName = "�߿��̿ձ�";
    }

    void Update()
    {
        if (isReloading == false)
        {
            StartCoroutine(ReloadingLaser());
        }

        //ü�� �� ����
        playerCastleHpBar.fillAmount = playerCastleHpAmount;
        enemyCastleHpBar.fillAmount = enemyCastleHpAmount;

        //�÷���Ÿ��
        playeTime += Time.deltaTime;
        

        timer += Time.deltaTime;
        // Ÿ�̸Ӱ� ���� ������ �ʰ��ϸ� CurCost�� ������ŵ�ϴ�.
        if (timer >= incrementInterval && curCost < maxCost)
        {
            curCost++;
            timer = 0f; // Ÿ�̸� ����
            //Debug.Log($"CurCost: {curCost}"); // ���� ����� ���
        }
        if (isVictory == true && isDefeat == false)
        {
            CharacterCleaner.SetActive(true);
            UiManager.Instance.VictoryUiSet();
            Invoke("TimeStop", 0.2f);

        }
        else if (isDefeat == true && isVictory == false)
        {
            CharacterCleaner.SetActive(true);
            UiManager.Instance.DefeatUISet();
            Invoke("TimeStop", 0.2f);
        }
        else if(isVictory == false && isDefeat == false)
        {
            Time.timeScale = 1;
        }
        
        //Ű���� ��ȯ �� ��ȣ�ۿ�
        if (Input.GetKeyDown(KeyCode.Alpha1) && commUnitReady == true)
        {
            spawnCommonCat(); // ���� ȣ��
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && tankerUnitReady == true)
        {
            spawnTankerCat(); 
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && meleeUnitReady == true)
        {
            spawnMeleeCat(); 
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && rangeUnitReady == true)
        {
            spawnRangeCat(); 
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            WalletLevelUp();
        }
        else if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            LaserFire();
        }
        else if(Input.GetKeyDown(KeyCode.Escape))
        {
            UiManager.Instance.OptionUISet();
        }
    }
    private void TimeStop()
    {
        Time.timeScale = 0;
    }
    public void GameStartInit()
    {
        //���� ���� �� �ʱ�ȭ
        CharacterCleaner.SetActive(false);
        isVictory = false;
        isDefeat = false;
        isReloadingDone = false;
        isReloading = false;
        Time.timeScale = 1;

        playerTowerCurHp = playerTowerMaxHp;
        enemyTowerCurHp = enemyTowerMaxHp;
        curCost = 0;
        maxCost = 150;
        CurLevel = 0;
        playeTime = 0;
        LaserCurTime = 0;

        //��ȯ ��ư �ʱ�ȭ
        commUnitReady = true;
        tankerUnitReady = true;
        meleeUnitReady = true;
        rangeUnitReady = true;

        UiManager.Instance.ReloadingDoneParticle.Stop();
    }
    public IEnumerator ButtonCooltime(Button button, GameObject coolObj, Image coolTimeBar, float coolTime)
    {
        button.interactable = false; // ��ư ��Ȱ��ȭ
        coolObj.SetActive(true); // ��Ÿ�� ������Ʈ Ȱ��ȭ
        float elapsedTime = 0f;

        while (elapsedTime < coolTime)
        {
            elapsedTime += Time.deltaTime;
            coolTimeBar.fillAmount = elapsedTime / coolTime; // ��Ÿ�� �� ������Ʈ
            yield return null; // ���� �����ӱ��� ���
        }

        coolTimeBar.fillAmount = 0; // ��Ÿ�� �� �ʱ�ȭ
        button.interactable = true; // ��ư ��Ȱ��ȭ
        coolObj.SetActive(false); // ��Ÿ�� ������Ʈ ��Ȱ��ȭ
    }

    //��ȯ
    public void spawnCommonCat()
    {
        if (curCost > playerUnits[0].UsedCost)
        {
            curCost -= playerUnits[0].UsedCost;
            GameObject newCommonUnit = Instantiate(playerUnits[0].UnitProjectile, spawnPos.position, transform.rotation, null);
            Vector2 CommonVfxPos = new Vector2(newCommonUnit.transform.position.x-1f, newCommonUnit.transform.position.y + 2f);
            GameObject newSpawnParticle = Instantiate(commonSpawnVfx, CommonVfxPos, transform.rotation,null);
            Destroy(newSpawnParticle, 2f);
            StartCoroutine(ButtonCooltime(spawnCommonButton, commonCoolObj, commonCoolTimeBar, playerUnits[0].SpawnCoolTime));
            StartCoroutine(CommonUnitCooldown());
        }
        else
            Debug.Log("Cost�� �����մϴ�.");
    }
    private IEnumerator CommonUnitCooldown()
    {
        commUnitReady = false;
        yield return new WaitForSeconds(playerUnits[0].SpawnCoolTime);
        commUnitReady = true;
    }
    public void spawnTankerCat()
    {
        if (curCost > playerUnits[1].UsedCost)
        {
            curCost -= playerUnits[1].UsedCost;
            GameObject newTankerUnit = Instantiate(playerUnits[1].UnitProjectile, spawnPos.position, transform.rotation, null);
            Vector2 TankerVfxPos = new Vector2(newTankerUnit.transform.position.x - 1f, newTankerUnit.transform.position.y + 2f);
            GameObject newSpawnParticle = Instantiate(tankerSpawnVfx, TankerVfxPos, transform.rotation, null);
            Destroy(newSpawnParticle, 2f);
            StartCoroutine(ButtonCooltime(spawnTankerButton,tankerCoolObj, tankerCoolTimeBar, playerUnits[1].SpawnCoolTime));
            StartCoroutine(TankerUnitCooldown());
        }
        else
            Debug.Log("Cost�� �����մϴ�.");
    }
    private IEnumerator TankerUnitCooldown()
    {
        tankerUnitReady = false;
        yield return new WaitForSeconds(playerUnits[1].SpawnCoolTime);
        tankerUnitReady = true;
    }
    public void spawnMeleeCat()
    {
        if (curCost > playerUnits[2].UsedCost)
        {
            curCost -= playerUnits[2].UsedCost;
            GameObject newMeleeUnit = Instantiate(playerUnits[2].UnitProjectile, spawnPos.position, transform.rotation, null);
            Vector2 MeleeVfxPos = new Vector2(newMeleeUnit.transform.position.x - 1f, newMeleeUnit.transform.position.y + 2f);
            GameObject newSpawnParticle = Instantiate(meleeSpawnVfx, MeleeVfxPos, transform.rotation, null);
            Destroy(newSpawnParticle, 2f);
            StartCoroutine(ButtonCooltime(spawnMeleeButton,meleeCoolObj, meleeCoolTimeBar, playerUnits[2].SpawnCoolTime));
            StartCoroutine(MeleeUnitCooldown());
        }
        else
            Debug.Log("Cost�� �����մϴ�.");
    }
    private IEnumerator MeleeUnitCooldown()
    {
        meleeUnitReady = false;
        yield return new WaitForSeconds(playerUnits[2].SpawnCoolTime);
        meleeUnitReady = true;
    }
    public void spawnRangeCat()
    {
        if (curCost > playerUnits[3].UsedCost)
        {
            curCost -= playerUnits[3].UsedCost;
            GameObject newRangeUnit = Instantiate(playerUnits[3].UnitProjectile, spawnPos.position, transform.rotation, null);
            Vector2 RangeVfxPos = new Vector2(newRangeUnit.transform.position.x - 1f, newRangeUnit.transform.position.y + 2f);
            GameObject newSpawnParticle = Instantiate(RangeSpawnVfx, RangeVfxPos, transform.rotation, null);
            Destroy(newSpawnParticle, 2f);
            StartCoroutine(ButtonCooltime(spawnRangeButton, rangeCoolObj, rangeCoolTimeBar, playerUnits[3].SpawnCoolTime));
            StartCoroutine(RangeUnitCooldown());
        }
        else
            Debug.Log("Cost�� �����մϴ�.");
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
                curCost -= levelUpCost[CurLevel];
                CurLevel++;
                maxCost += 50;
                Debug.Log($"Level�� ����߽��ϴ�. ���� Lv.{CurLevel}");
            }
            else
            {
                Debug.Log("Cost�� ������ LevelUp�� �Ұ��� �մϴ�.");
            }
        }
        else
        {
            Debug.Log("�ִ� �����̹Ƿ� ���̻� ������ �Ұ����մϴ�.");
        }
    }

    private IEnumerator ReloadingLaser()
    {
        if (isReloading) yield break; // �̹� ���� ���̸� ����

        TowerReloadParticle.SetActive(true);
        UiManager.Instance.RealodingParticle.Play();
        isReloading = true; 
        isReloadingDone = false; 
        ReadyText.SetActive(false); 

        while (LaserCurTime < LaserCoolTime)
        {
            LaserCurTime += Time.deltaTime;
            ReloadingImage.fillAmount = LaserCurTime / LaserCoolTime; // ��Ÿ�� �� ������Ʈ
            yield return null; // ���� �����ӱ��� ���
        }

        // ��Ÿ�� �Ϸ� ��
        isReloadingDone = true;
        UiManager.Instance.RealodingParticle.Stop();
        UiManager.Instance.ReloadingDoneParticle.Play();
        TowerReloadParticle.SetActive(false);
        ReadyText.SetActive(true);
    }
    private void LaserFire()
    {
        if(isReloadingDone == true)
        {
            //�߻�
            UiManager.Instance.ReloadingDoneParticle.Stop();
            isReloading = false;
            isReloadingDone = false;
            LaserCurTime = 0;   //��Ÿ�� �ʱ�ȭ
            ReloadingImage.fillAmount = 0;
            ReadyText.SetActive(false);
            Debug.Log("������ �߻�!!");
            TowerLaserObj.GetComponent<FireLaser>().StartCoroutine(TowerLaserObj.GetComponent<FireLaser>().ShootLaser());
        }
        else
        {
            Debug.Log("�������� ���� �ʾ� �߻翡 �����߽��ϴ�.");
        }
    }
}
