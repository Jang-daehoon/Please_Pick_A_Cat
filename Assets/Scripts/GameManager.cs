using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("소환 가능한 유닛 data")]
    public PlayerUnitSO[] playerUnits;

    [Header("게임 상에서 상호작용 하는 버튼, 쿨타임 게임오브젝트 및 이미지")]
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
    [Header("유닛 소환 파티클")]
    [SerializeField,Tooltip("일반유닛 소환 파티클")] private GameObject commonSpawnVfx;
    [SerializeField, Tooltip("탱커유닛 소환 파티클")] private GameObject tankerSpawnVfx;
    [SerializeField, Tooltip("근접유닛 소환 파티클")] private GameObject meleeSpawnVfx;
    [SerializeField, Tooltip("원거리유닛 소환 파티클")] private GameObject RangeSpawnVfx;
    public GameObject AcensionPrefab;

    [Header("GameData")]
    public int[] levelUpCost;
    public string StageName;
    public int CurLevel;
    public float playeTime;
    [SerializeField] private Transform spawnPos;
    [SerializeField]private float LaserCoolTime = 48;
    [SerializeField]private float LaserCurTime = 0;
    private bool isReloading = false; // 코루틴 실행 중인지 확인하는 변수
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
    private float playerCastleHpAmount { get { return playerTowerCurHp / playerTowerMaxHp; } }  //자주 계산되는 항목은 프로퍼티로 만들기
    [Header("PlayerCastleUI")]
    [SerializeField] private Image playerCastleHpBar;

    [Header("EnemyCastleData")]
    public float enemyTowerMaxHp;
    public float enemyTowerCurHp;
    [Header("EnemyCastleUI")]
    [SerializeField] private Image enemyCastleHpBar;
    private float enemyCastleHpAmount { get { return enemyTowerCurHp / enemyTowerMaxHp; } }  //자주 계산되는 항목은 프로퍼티로 만들기

    [Header("CostInfo")]
    public int maxCost;  //최대 Cost;
    public int curCost; //현재 보유중인 Cost
    public float incrementInterval = 0.01f; // Cost증가 간격
    private float timer = 0f;
    public bool isDefeat;  //패배
    public bool isVictory; //승리

    [Header("ObjectCleaner")]
    public GameObject CharacterCleaner;

    private void Awake()
    {
        Instance = this;
        LaserCoolTime = 48;
        isReloadingDone = false;
        GameSet.SetActive(false);

        //기본 Cost
        maxCost = 150;

        //소환 버튼 초기화
        commUnitReady = true;
        tankerUnitReady = true;
        meleeUnitReady = true;
        rangeUnitReady = true;

        //소환
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
        StageName = "야옹이왕국";
    }

    void Update()
    {
        if (isReloading == false)
        {
            StartCoroutine(ReloadingLaser());
        }

        //체력 바 갱신
        playerCastleHpBar.fillAmount = playerCastleHpAmount;
        enemyCastleHpBar.fillAmount = enemyCastleHpAmount;

        //플레이타임
        playeTime += Time.deltaTime;
        

        timer += Time.deltaTime;
        // 타이머가 증가 간격을 초과하면 CurCost를 증가시킵니다.
        if (timer >= incrementInterval && curCost < maxCost)
        {
            curCost++;
            timer = 0f; // 타이머 리셋
            //Debug.Log($"CurCost: {curCost}"); // 현재 비용을 출력
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
        
        //키보드 소환 및 상호작용
        if (Input.GetKeyDown(KeyCode.Alpha1) && commUnitReady == true)
        {
            spawnCommonCat(); // 직접 호출
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
        //게임 시작 시 초기화
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

        //소환 버튼 초기화
        commUnitReady = true;
        tankerUnitReady = true;
        meleeUnitReady = true;
        rangeUnitReady = true;

        UiManager.Instance.ReloadingDoneParticle.Stop();
    }
    public IEnumerator ButtonCooltime(Button button, GameObject coolObj, Image coolTimeBar, float coolTime)
    {
        button.interactable = false; // 버튼 비활성화
        coolObj.SetActive(true); // 쿨타임 오브젝트 활성화
        float elapsedTime = 0f;

        while (elapsedTime < coolTime)
        {
            elapsedTime += Time.deltaTime;
            coolTimeBar.fillAmount = elapsedTime / coolTime; // 쿨타임 바 업데이트
            yield return null; // 다음 프레임까지 대기
        }

        coolTimeBar.fillAmount = 0; // 쿨타임 바 초기화
        button.interactable = true; // 버튼 재활성화
        coolObj.SetActive(false); // 쿨타임 오브젝트 비활성화
    }

    //소환
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
            Debug.Log("Cost가 부족합니다.");
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
            Debug.Log("Cost가 부족합니다.");
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
            Debug.Log("Cost가 부족합니다.");
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
            Debug.Log("Cost가 부족합니다.");
    }
    private IEnumerator RangeUnitCooldown()
    {
        rangeUnitReady = false;
        yield return new WaitForSeconds(playerUnits[3].SpawnCoolTime);
        rangeUnitReady = true;
    }
    public void WalletLevelUp()
    {
        // 최대 레벨 체크: CurLevel이 levelUpCost의 최대 인덱스보다 작은지 확인
        if (CurLevel < levelUpCost.Length)
        {
            if (curCost >= levelUpCost[CurLevel])
            {
                curCost -= levelUpCost[CurLevel];
                CurLevel++;
                maxCost += 50;
                Debug.Log($"Level이 상승했습니다. 현재 Lv.{CurLevel}");
            }
            else
            {
                Debug.Log("Cost가 부족해 LevelUp이 불가능 합니다.");
            }
        }
        else
        {
            Debug.Log("최대 레벨이므로 더이상 성장이 불가능합니다.");
        }
    }

    private IEnumerator ReloadingLaser()
    {
        if (isReloading) yield break; // 이미 실행 중이면 종료

        TowerReloadParticle.SetActive(true);
        UiManager.Instance.RealodingParticle.Play();
        isReloading = true; 
        isReloadingDone = false; 
        ReadyText.SetActive(false); 

        while (LaserCurTime < LaserCoolTime)
        {
            LaserCurTime += Time.deltaTime;
            ReloadingImage.fillAmount = LaserCurTime / LaserCoolTime; // 쿨타임 바 업데이트
            yield return null; // 다음 프레임까지 대기
        }

        // 쿨타임 완료 후
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
            //발사
            UiManager.Instance.ReloadingDoneParticle.Stop();
            isReloading = false;
            isReloadingDone = false;
            LaserCurTime = 0;   //쿨타임 초기화
            ReloadingImage.fillAmount = 0;
            ReadyText.SetActive(false);
            Debug.Log("레이저 발사!!");
            TowerLaserObj.GetComponent<FireLaser>().StartCoroutine(TowerLaserObj.GetComponent<FireLaser>().ShootLaser());
        }
        else
        {
            Debug.Log("재장전이 되지 않아 발사에 실패했습니다.");
        }
    }
}
