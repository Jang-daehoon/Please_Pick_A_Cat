using Cinemachine;
using System;
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
    [SerializeField] private GameObject LaserNotice;
    [Header("유닛 소환 파티클")]
    [SerializeField,Tooltip("일반유닛 소환 파티클")] private GameObject commonSpawnVfx;
    [SerializeField, Tooltip("탱커유닛 소환 파티클")] private GameObject tankerSpawnVfx;
    [SerializeField, Tooltip("근접유닛 소환 파티클")] private GameObject meleeSpawnVfx;
    [SerializeField, Tooltip("원거리유닛 소환 파티클")] private GameObject RangeSpawnVfx;
    public GameObject AcensionPrefab;

    private bool hasTriggeredVictory = false;
    private bool hasTriggeredDefeat = false;

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

        LoadLevelData();
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
        //배경음 재생
        AudioManager.Instance.PlayBackgroundMusic(AudioManager.Instance.bgmClip);
        
        if (isLevelEasy)
        {
            StageName = "야옹이왕국";
        }
        else if (isLevelNomal)
        {
            StageName = "근육고양이왕국";
        }
        else if (isLevelHard)
        {
            StageName = "지옥고양이왕국";
        }
    }

    void Update()
    {
        Debug.Log($"현재 TimeScale = {Time.timeScale}");
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

        // 승리/패배 상태 체크 및 처리
        GameEndState();

        // 옵션 UI 상태에 따른 타임스케일 처리
        TimeScale();

        // 키보드 입력 처리
        KeyboardInput();
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
        // 게임이 승리 또는 패배 상태가 아니고, 배속이 활성화되지 않은 상태일 때
        if (!isVictory && !isDefeat)
        {
            // 옵션, 종료, 도움말 UI가 활성화된 상태에서는 게임 일시 정지
            if (UiManager.Instance.isOptionActive ||
                UiManager.Instance.isExitUiActive ||
                UiManager.Instance.isHelpUIActive)
            {
                Time.timeScale = 0;
            }
            // 배속이 활성화되지 않은 상태일 때, 게임 속도는 기본 (1)
            else if (!UiManager.Instance.isAccelerate)
            {
                Time.timeScale = 1;
            }
            // 배속이 활성화된 상태일 때, 게임 속도는 2
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
            LaserFire();
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
    // 게임 재시작이나 새 게임 시작 시 호출할 리셋 메서드
    public void GameStartInit()
    {
        //게임 시작 시 초기화
        hasTriggeredVictory = false;
        hasTriggeredDefeat = false;

        levelUpBtn.interactable = true;
        mainCamera.transform.position = StartCamPos;
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
            //유닛 구매 효과음 재생
            AudioManager.Instance.PlayEffectSound(AudioManager.Instance.spawnUnitClip);
            curCost -= playerUnits[0].UsedCost;

            //오브젝트 풀링
            PlayerUnit commonCat = PlayerUnitPool.pool.Pop();
            commonCat.transform.SetPositionAndRotation(spawnPos.position, transform.rotation);  //위치, 회전값 넣어줘야함.
            commonCat.Unitinit(playerUnits[0]);
            commonCat.ResetUnit();
            //Sprite변경
            commonCat.GetComponentInChildren<SpriteRenderer>().sprite = playerUnits[0].UnitProjectile.GetComponentInChildren<SpriteRenderer>().sprite;
            //Animator변경
            // 먼저 commonCat과 UnitProjectile의 Animator 컴포넌트를 각각 가져온다.
            Animator commonCatAnimator = commonCat.GetComponent<Animator>();
            Animator unitProjectileAnimator = playerUnits[0].UnitProjectile.GetComponent<Animator>();

            // 이제 commonCatAnimator의 상태를 unitProjectileAnimator의 상태로 변경.
            commonCatAnimator.runtimeAnimatorController = unitProjectileAnimator.runtimeAnimatorController;

            //GameObject newCommonUnit = Instantiate(playerUnits[0].UnitProjectile, spawnPos.position, transform.rotation, null); //일반소환

            Vector2 CommonVfxPos = new Vector2(commonCat.transform.position.x - 1f, commonCat.transform.position.y + 2f);
            GameObject newSpawnParticle = Instantiate(commonSpawnVfx, CommonVfxPos, transform.rotation, null);
            Destroy(newSpawnParticle, 2f);
            StartCoroutine(ButtonCooltime(spawnCommonButton, commonCoolObj, commonCoolTimeBar, playerUnits[0].SpawnCoolTime));
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
        if (curCost > playerUnits[1].UsedCost)
        {
            //유닛 구매 효과음 재생
            AudioManager.Instance.PlayEffectSound(AudioManager.Instance.spawnUnitClip);
            curCost -= playerUnits[1].UsedCost;
            //오브젝트 풀링
            PlayerUnit tankerCat = PlayerUnitPool.pool.Pop();
            tankerCat.transform.SetPositionAndRotation(spawnPos.position, transform.rotation);  //위치, 회전값 넣어줘야함.
            tankerCat.Unitinit(playerUnits[1]);
            tankerCat.ResetUnit();
            //Sprite변경
            tankerCat.GetComponentInChildren<SpriteRenderer>().sprite = playerUnits[1].UnitProjectile.GetComponentInChildren<SpriteRenderer>().sprite;
            //Animator변경
            // 먼저 commonCat과 UnitProjectile의 Animator 컴포넌트를 각각 가져온다.
            Animator tankerCatAnimator = tankerCat.GetComponent<Animator>();
            Animator unitProjectileAnimator = playerUnits[1].UnitProjectile.GetComponent<Animator>();

            // 이제 commonCatAnimator의 상태를 unitProjectileAnimator의 상태로 변경.
            tankerCatAnimator.runtimeAnimatorController = unitProjectileAnimator.runtimeAnimatorController;

            //GameObject newTankerUnit = Instantiate(playerUnits[1].UnitProjectile, spawnPos.position, transform.rotation, null);
            Vector2 TankerVfxPos = new Vector2(tankerCat.transform.position.x - 1f, tankerCat.transform.position.y + 2f);
            GameObject newSpawnParticle = Instantiate(tankerSpawnVfx, TankerVfxPos, transform.rotation, null);
            Destroy(newSpawnParticle, 2f);
            StartCoroutine(ButtonCooltime(spawnTankerButton,tankerCoolObj, tankerCoolTimeBar, playerUnits[1].SpawnCoolTime));
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
        if (curCost >= playerUnits[2].UsedCost)
        {
            //유닛 구매 효과음 재생
            AudioManager.Instance.PlayEffectSound(AudioManager.Instance.spawnUnitClip);
            curCost -= playerUnits[2].UsedCost;
            //오브젝트 풀링
            PlayerUnit meleeUnit = PlayerUnitPool.pool.Pop();
            meleeUnit.transform.SetPositionAndRotation(spawnPos.position, transform.rotation);  //위치, 회전값 넣어줘야함.
            meleeUnit.Unitinit(playerUnits[2]);
            meleeUnit.ResetUnit();
            //Sprite변경
            meleeUnit.GetComponentInChildren<SpriteRenderer>().sprite = playerUnits[2].UnitProjectile.GetComponentInChildren<SpriteRenderer>().sprite;
            //Animator변경
            // 먼저 commonCat과 UnitProjectile의 Animator 컴포넌트를 각각 가져온다.
            Animator meleeUnitAnimator = meleeUnit.GetComponent<Animator>();
            Animator unitProjectileAnimator = playerUnits[2].UnitProjectile.GetComponent<Animator>();

            // 이제 commonCatAnimator의 상태를 unitProjectileAnimator의 상태로 변경.
            meleeUnitAnimator.runtimeAnimatorController = unitProjectileAnimator.runtimeAnimatorController;


            //GameObject newMeleeUnit = Instantiate(playerUnits[2].UnitProjectile, spawnPos.position, transform.rotation, null);
            Vector2 MeleeVfxPos = new Vector2(meleeUnit.transform.position.x - 1f, meleeUnit.transform.position.y + 2f);
            GameObject newSpawnParticle = Instantiate(meleeSpawnVfx, MeleeVfxPos, transform.rotation, null);
            Destroy(newSpawnParticle, 2f);
            StartCoroutine(ButtonCooltime(spawnMeleeButton,meleeCoolObj, meleeCoolTimeBar, playerUnits[2].SpawnCoolTime));
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
        if (curCost > playerUnits[3].UsedCost)
        {
            //유닛 구매 효과음 재생
            AudioManager.Instance.PlayEffectSound(AudioManager.Instance.spawnUnitClip);
            curCost -= playerUnits[3].UsedCost;
            //오브젝트 풀링
            PlayerUnit rangeUnit = PlayerUnitPool.pool.Pop();
            rangeUnit.transform.SetPositionAndRotation(spawnPos.position, transform.rotation);  //위치, 회전값 넣어줘야함.
            rangeUnit.Unitinit(playerUnits[3]);
            rangeUnit.ResetUnit();
            //Sprite변경
            rangeUnit.GetComponentInChildren<SpriteRenderer>().sprite = playerUnits[3].UnitProjectile.GetComponentInChildren<SpriteRenderer>().sprite;
            //Animator변경
            // 먼저 commonCat과 UnitProjectile의 Animator 컴포넌트를 각각 가져온다.
            Animator rangeUnitAnimator = rangeUnit.GetComponent<Animator>();
            Animator unitProjectileAnimator = playerUnits[3].UnitProjectile.GetComponent<Animator>();

            // 이제 commonCatAnimator의 상태를 unitProjectileAnimator의 상태로 변경.
            rangeUnitAnimator.runtimeAnimatorController = unitProjectileAnimator.runtimeAnimatorController;


            //GameObject newRangeUnit = Instantiate(playerUnits[3].UnitProjectile, spawnPos.position, transform.rotation, null);
            Vector2 RangeVfxPos = new Vector2(rangeUnit.transform.position.x - 1f, rangeUnit.transform.position.y + 2f);
            GameObject newSpawnParticle = Instantiate(RangeSpawnVfx, RangeVfxPos, transform.rotation, null);
            Destroy(newSpawnParticle, 2f);
            StartCoroutine(ButtonCooltime(spawnRangeButton, rangeCoolObj, rangeCoolTimeBar, playerUnits[3].SpawnCoolTime));
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
        // 최대 레벨 체크: CurLevel이 levelUpCost의 최대 인덱스보다 작은지 확인
        if (CurLevel < levelUpCost.Length)
        {
            if (curCost >= levelUpCost[CurLevel])
            {
                //레벨 업 효과음 재생
                AudioManager.Instance.PlayEffectSound(AudioManager.Instance.levelUpClip);
                curCost -= levelUpCost[CurLevel];
                CurLevel++;
                maxCost += 80;
                levelUpNotice.GetComponent<Animator>().SetTrigger("LevelUp");
                Debug.Log($"Level이 상승했습니다. 현재 Lv.{CurLevel}");
            }
            else
            {
                lackCost.GetComponent<Animator>().SetTrigger("LackOfCost");
                Debug.Log("Cost가 부족해 LevelUp이 불가능 합니다.");
            }
        }
        else
        {
            levelUpBtn.interactable = false;
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
            //레이저 발사 효과음 재생
            AudioManager.Instance.PlayEffectSound(AudioManager.Instance.FireLaserClip);
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
            LaserNotice.GetComponent<Animator>().SetTrigger("LaserNoticeOn");
            Debug.Log("재장전이 되지 않아 발사에 실패했습니다.");
        }
    }
    public void SaveLevelData()
    {
        // 각 레벨의 클리어 여부를 저장
        PlayerPrefs.SetInt("EasyLevelClear", isEasyClear ? 1 : 0);
        PlayerPrefs.SetInt("NomalLevelClear", isNomalClear ? 1 : 0);
        PlayerPrefs.SetInt("HardLevelClear", isHardClear ? 1 : 0);

        // 변경 사항을 디스크에 저장
        PlayerPrefs.Save();

        // 저장된 데이터 확인
        Debug.Log("Level Easy: " + PlayerPrefs.GetInt("Level_Easy"));
        Debug.Log("Level Nomal: " + PlayerPrefs.GetInt("Level_Nomal"));
        Debug.Log("Level Hard: " + PlayerPrefs.GetInt("Level_Hard"));
    }
    public void LoadLevelData()
    {
        // 각 레벨의 클리어 여부를 불러옴
        isEasyClear = PlayerPrefs.GetInt("EasyLevelClear", 0) == 1;
        isNomalClear = PlayerPrefs.GetInt("NomalLevelClear", 0) == 1;
        isHardClear = PlayerPrefs.GetInt("HardLevelClear", 0) == 1;
    }
}
