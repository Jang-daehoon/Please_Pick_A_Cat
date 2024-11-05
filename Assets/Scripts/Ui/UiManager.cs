using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI의 활성화 비활성화 및 상태 참조 매니저
/// </summary>
public class UiManager : MonoBehaviour
{
    public static UiManager Instance;
    [Header("UI Objects")]
    [SerializeField] private GameObject TitleUI;
    [SerializeField] private GameObject TitleUiSet;
    [SerializeField] private GameObject StageSelectUISet;

    [SerializeField] private GameObject inGameUi;
    [SerializeField] private GameObject victoryUi;
    [SerializeField] private GameObject defeatUi;
    [SerializeField] private GameObject resume_OptionUi;
    [SerializeField] private GameObject exitUi;

    [Header("UIActive Set")]
    [SerializeField] private bool isTitleUIActive;
    [SerializeField] private bool isTitleSetActive;
    [SerializeField] private bool isStageSelectactive;

    [SerializeField] private bool isVictoryUiActive;  //승리UI 활성화 / 비활성화
    [SerializeField] private bool isDefeatUiActive;   //패배 UI 활성화 / 비활성화
    [SerializeField] private bool isOptionActive;  //옵션 활성화 / 비활성화
    [SerializeField] private bool isResumeActive;    //일시정지 활성화 / 비활성화
    [SerializeField] private bool isExitUiActive;    //전투이탈 UI활성화 / 비활성화
    [SerializeField] private bool isInGameUIActive;  //인게임 UI활셩화 / 비활성화

    [Header("UIObjectButtons")]
    [SerializeField] private Button gameStartBtn;
    [SerializeField] private Button gameExitBtn;
    [SerializeField] private Button victoryButton;
    [SerializeField] private Button defeatButton;
    [SerializeField] private Button battleExitBtn;
    [SerializeField] private Button battleExitYesBtn;
    [SerializeField] private Button battleExitNoBtn;
    [SerializeField] private Button optionBtn;
    [SerializeField] private Button optionContinueBtn;

    [Header("UIStageSelectButtons")]
    [SerializeField] private Button levelEasyBtn;
    [SerializeField] private Button levelNomalBtn;
    [SerializeField] private Button levelHardBtn;

    [Header("UI Particle")]
    public ParticleSystem RealodingParticle;
    public ParticleSystem ReloadingDoneParticle;

    [Header("UI Notice")]
    public GameObject CantSelectStage;

    private void Awake()
    {
        Instance = this;

        //초기 세팅
        TitleUI.SetActive(true);
        isTitleUIActive = true;

        levelEasyBtn.onClick.AddListener(SelectEasyLevel);
        levelNomalBtn.onClick.AddListener(SelectNomalLevel);
        levelHardBtn.onClick.AddListener(SelectHardLevel);
        
        RealodingParticle.Stop();
        ReloadingDoneParticle.Stop();

        gameStartBtn.onClick.AddListener(GameStart);
        gameExitBtn.onClick.AddListener(GameExit);

        victoryButton.onClick.AddListener(VictoryOkClick);
        defeatButton.onClick.AddListener(DefeatOkClick);

        optionBtn.onClick.AddListener(OptionUISet);
        optionContinueBtn.onClick.AddListener(OptionUIExit);

        battleExitBtn.onClick.AddListener(ExitButtonClick);
        battleExitYesBtn.onClick.AddListener(BattleExitOk);
        battleExitNoBtn.onClick.AddListener(BattleExitCancel);
    }

    private void Update()
    { 

    }
    public void GameStart()
    {
        // UI를 비활성화하기 전에 상태 점검 및 업데이트
        if (isTitleUIActive)
        {
            TitleUiSet.SetActive(false);
            isTitleSetActive = false;
            isTitleUIActive = false;
        }

        StageSelectUISet.SetActive(true);
        isStageSelectactive = true;

        // 상태 업데이트가 일관되게 이루어지는지 확인
        Debug.Log("Title UI Active: " + isTitleUIActive);  // 상태 점검
    }

    public void GameExit()
    {
        // 게임이 빌드되어 실행 중일 때
        Application.Quit();

        // 에디터에서 실행 중일 때
#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
        }
#endif
    }
    public void SelectEasyLevel()
    {
        GameManager.Instance.isLevelEasy = true;

        TitleUI.SetActive(false);
        isTitleUIActive = false;

        StageSelectUISet.SetActive(false);
        isStageSelectactive = false;

        inGameUi.SetActive(true);
        isInGameUIActive = true;
        GameManager.Instance.GameStartInit();
        GameManager.Instance.GameSet.SetActive(true);
    }
    public void SelectNomalLevel()
    {
        if (GameManager.Instance.isEasyClear == false)
        {
            StartCoroutine(CantSelectStageNotice());
        }
        else if (GameManager.Instance.isEasyClear == true)
        {
            GameManager.Instance.isLevelNomal = true;

            TitleUI.SetActive(false);
            isTitleUIActive = false;

            StageSelectUISet.SetActive(false);
            isStageSelectactive = false;

            inGameUi.SetActive(true);
            isInGameUIActive = true;
            GameManager.Instance.GameStartInit();
            GameManager.Instance.GameSet.SetActive(true);
        }
    }
    public void SelectHardLevel()
    {
        if (GameManager.Instance.isNomalClear == false)
        {
            StartCoroutine(CantSelectStageNotice());
        }
        else if(GameManager.Instance.isNomalClear == true)
        {
            GameManager.Instance.isLevelHard = true;

            TitleUI.SetActive(false);
            isTitleUIActive = false;

            StageSelectUISet.SetActive(false);
            isStageSelectactive = false;

            inGameUi.SetActive(true);
            isInGameUIActive = true;
            GameManager.Instance.GameStartInit();
            GameManager.Instance.GameSet.SetActive(true);
        }
    }
    private IEnumerator CantSelectStageNotice()
    {
        CantSelectStage.SetActive(true);
        yield return new WaitForSeconds(2f);
        CantSelectStage.SetActive(false);
    }

    //승리 UI 상호작용
    public void VictoryUiSet()
    {
        GameManager.Instance.isVictory = true;

        if (GameManager.Instance.isLevelEasy == true)
        {
            GameManager.Instance.isEasyClear = true;
            GameManager.Instance.isLevelEasy = false;
        }
        else if (GameManager.Instance.isLevelNomal == true)
        {
            GameManager.Instance.isNomalClear = true;
            GameManager.Instance.isLevelNomal = false;
        }
        else if (GameManager.Instance.isLevelHard == true)
        {
            GameManager.Instance.isHardClear = true;
            GameManager.Instance.isLevelHard = false;
        }

        GameManager.Instance.SaveLevelData();   //클리어 데이터 저장

        victoryUi.SetActive(true);
        isVictoryUiActive = true;

        inGameUi.SetActive(false);
        isInGameUIActive = false;
    }
    private void VictoryOkClick()
    {
        GameManager.Instance.isVictory = false;
        victoryUi.SetActive(false);
        isVictoryUiActive = false;

        inGameUi.SetActive(false);
        isInGameUIActive = false;

        //메인화면으로 복귀
        TitleUI.SetActive(true);
        isTitleUIActive = true;
        TitleUiSet.SetActive(true);
        isTitleSetActive = true;

        StageSelectUISet.SetActive(false);
        isStageSelectactive = false;
        GameManager.Instance.GameSet.SetActive(false);
    }
    //패배 UI 상호작용
    public void DefeatUISet()
    {
        GameManager.Instance.isDefeat = true;
        GameManager.Instance.isLevelEasy = false;
        GameManager.Instance.isLevelNomal = false;
        GameManager.Instance.isLevelHard = false;

        defeatUi.SetActive(true);
        isDefeatUiActive = true;

        inGameUi.SetActive(false);
        isInGameUIActive = false;
    }   
    private void DefeatOkClick()
    {
        GameManager.Instance.isDefeat = false;

        defeatUi.SetActive(false);
        isDefeatUiActive=false;

        inGameUi.SetActive(false);
        isInGameUIActive = false;

        //메인화면으로 복귀
        TitleUI.SetActive(true);
        isTitleUIActive = true;
        TitleUiSet.SetActive(true);
        isTitleSetActive = true;

        StageSelectUISet.SetActive(false);
        isStageSelectactive = false;
        GameManager.Instance.GameSet.SetActive(false);
    }
    public void OptionUISet()
    {
        //일시정지 하라는 내용은 없지만 임시로 일시정지 시 게임도 정지시키도록 작성
        Time.timeScale = 0;
        resume_OptionUi.SetActive(true);
        isOptionActive = true;
    }
    public void OptionUIExit()
    {
        Time.timeScale = 1;
        resume_OptionUi.SetActive(false);
        isOptionActive = false;
    }
    private void ExitButtonClick()
    {
        resume_OptionUi.SetActive(false);
        isOptionActive = false;

        exitUi.gameObject.SetActive(true);
        isExitUiActive = true;
    }
    //전투이탈 Yes or No 메서드 추가
    private void BattleExitOk()
    {
        // 게임이 빌드되어 실행 중일 때
        Application.Quit();

        // 에디터에서 실행 중일 때
#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
        }
#endif
    }
    private void BattleExitCancel()
    {
        exitUi.gameObject.SetActive(false);
        isExitUiActive = false;

        resume_OptionUi.SetActive(true);
        isOptionActive = true;
    }
}
