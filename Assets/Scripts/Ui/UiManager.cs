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
    [SerializeField] private GameObject HelpUI;

    [Header("UIActive Set")]
    [SerializeField] private bool isTitleUIActive;
    [SerializeField] private bool isTitleSetActive;
    [SerializeField] private bool isStageSelectactive;

    [SerializeField] private bool isVictoryUiActive;  //승리UI 활성화 / 비활성화
    [SerializeField] private bool isDefeatUiActive;   //패배 UI 활성화 / 비활성화
    public bool isOptionActive;  //옵션 활성화 / 비활성화
    public bool isExitUiActive;    //전투이탈 UI활성화 / 비활성화
    public bool isHelpUIActive; //도움말 UI활성화 / 비활성화
    [SerializeField] private bool isInGameUIActive;  //인게임 UI활셩화 / 비활성화
    public bool isAccelerate;   //배속 활성화 여부

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
    [SerializeField] private Button HelpBtn;
    [SerializeField] private Button helpExitBtn;
    [SerializeField] private Button bgmMuteBtn;
    [SerializeField] private Button effectClipMuteBtn;
    [SerializeField] private Button SpeedUpBtn;

    [Header("Mute Button States")]
    public bool isBgmMute = false;
    public bool isEffectClipMute = false;

    [Header("Mute Button Colors")]
    [SerializeField] private Color mutedColor = Color.red;
    [SerializeField] private Color unmutedColor = Color.white;

    [Header("UIStageSelectButtons")]
    [SerializeField] private Button levelEasyBtn;
    [SerializeField] private Button levelNomalBtn;
    [SerializeField] private Button levelHardBtn;

    [Header("UI Particle")]
    public GameObject UiParticleObj;
    public ParticleSystem RealodingParticle;
    public ParticleSystem ReloadingDoneParticle;

    [Header("UI Notice")]
    public GameObject CantSelectStage;

    private void Awake()
    {
        Instance = this;
        // 초기 버튼 색상 설정
        InitializeMuteButtonColors();
        
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
        HelpBtn.onClick.AddListener(HelpUISet);
        helpExitBtn.onClick.AddListener(HelpExit);

        battleExitBtn.onClick.AddListener(ExitButtonClick);
        battleExitYesBtn.onClick.AddListener(BattleExitOk);
        battleExitNoBtn.onClick.AddListener(BattleExitCancel);

        bgmMuteBtn.onClick.AddListener(MusicMute);
        effectClipMuteBtn.onClick.AddListener(EffectMute);

        SpeedUpBtn.onClick.AddListener(SpeedUpSet);


    }
    private void InitializeMuteButtonColors()
    {
        // BGM 버튼 초기 색상
        SetButtonColor(bgmMuteBtn, isBgmMute ? mutedColor : unmutedColor);

        // Effect 버튼 초기 색상
        SetButtonColor(effectClipMuteBtn, isEffectClipMute ? mutedColor : unmutedColor);
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

        //배속 상태 확인
        if (isAccelerate == true)
            SpeedUpSet();

        AudioManager.Instance.PlayGameEndSound(true);

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

        AudioManager.Instance.backgroundAudioSource.pitch = 1f;
        AudioManager.Instance.PlayBackgroundMusic(AudioManager.Instance.bgmClip);

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

        //배속 상태 확인
        if(isAccelerate == true)
            SpeedUpSet();

        //패배 효과음 재생
        AudioManager.Instance.PlayGameEndSound(false);

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

        AudioManager.Instance.backgroundAudioSource.pitch = 1f;
        AudioManager.Instance.PlayBackgroundMusic(AudioManager.Instance.bgmClip);

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
    private void HelpUISet()
    {
        resume_OptionUi.SetActive(false);
        isOptionActive = false;

        HelpUI.SetActive(true);
        isHelpUIActive = true;
    }
    private void HelpExit()
    {
        HelpUI.SetActive(false);
        isHelpUIActive = false;

        resume_OptionUi.SetActive(true);
        isOptionActive = true;
    }
    private void MusicMute()
    {
        isBgmMute = !isBgmMute;

        // BGM 음소거 상태 변경
        AudioManager.Instance.backgroundAudioSource.volume = isBgmMute ? 0 : 1;

        // BGM 버튼 색상만 변경
        SetButtonColor(bgmMuteBtn, isBgmMute ? mutedColor : unmutedColor);
    }

    private void EffectMute()
    {
        isEffectClipMute = !isEffectClipMute;

        // 이펙트 음소거 상태 변경
        AudioManager.Instance.effectAudioSource.volume = isEffectClipMute ? 0 : 1;

        // 이펙트 버튼 색상만 변경
        SetButtonColor(effectClipMuteBtn, isEffectClipMute ? mutedColor : unmutedColor);
    }
    private void SetButtonColor(Button btn, Color baseColor)
    {
        ColorBlock colors = btn.colors;
        colors.normalColor = baseColor;
        colors.highlightedColor = new Color(
            Mathf.Min(baseColor.r * 1.1f, 1f),
            Mathf.Min(baseColor.g * 1.1f, 1f),
            Mathf.Min(baseColor.b * 1.1f, 1f),
            baseColor.a
        );
        colors.pressedColor = new Color(
            baseColor.r * 0.8f,
            baseColor.g * 0.8f,
            baseColor.b * 0.8f,
            baseColor.a
        );
        colors.selectedColor = baseColor;
        colors.disabledColor = new Color(0.8f, 0.8f, 0.8f, 0.5f);
        btn.colors = colors;
    }
    private void SpeedUpSet()
    {
        // 배속 상태를 반전시킴
        isAccelerate = !isAccelerate;

        //배속 스크립트는 GameManager에서 Bool값으로 제어중 

        // 배속 상태에 따라 버튼 색상 변경
        SetButtonColor(SpeedUpBtn, isAccelerate ? Color.green : Color.white);
    }

}
