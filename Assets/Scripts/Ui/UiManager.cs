using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI�� Ȱ��ȭ ��Ȱ��ȭ �� ���� ���� �Ŵ���
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

    [SerializeField] private bool isVictoryUiActive;  //�¸�UI Ȱ��ȭ / ��Ȱ��ȭ
    [SerializeField] private bool isDefeatUiActive;   //�й� UI Ȱ��ȭ / ��Ȱ��ȭ
    public bool isOptionActive;  //�ɼ� Ȱ��ȭ / ��Ȱ��ȭ
    public bool isExitUiActive;    //������Ż UIȰ��ȭ / ��Ȱ��ȭ
    public bool isHelpUIActive; //���� UIȰ��ȭ / ��Ȱ��ȭ
    [SerializeField] private bool isInGameUIActive;  //�ΰ��� UIȰ��ȭ / ��Ȱ��ȭ
    public bool isAccelerate;   //��� Ȱ��ȭ ����

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
        // �ʱ� ��ư ���� ����
        InitializeMuteButtonColors();
        
        //�ʱ� ����
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
        // BGM ��ư �ʱ� ����
        SetButtonColor(bgmMuteBtn, isBgmMute ? mutedColor : unmutedColor);

        // Effect ��ư �ʱ� ����
        SetButtonColor(effectClipMuteBtn, isEffectClipMute ? mutedColor : unmutedColor);
    }

    public void GameStart()
    {
        // UI�� ��Ȱ��ȭ�ϱ� ���� ���� ���� �� ������Ʈ
        if (isTitleUIActive)
        {
            TitleUiSet.SetActive(false);
            isTitleSetActive = false;
            isTitleUIActive = false;
        }

        StageSelectUISet.SetActive(true);
        isStageSelectactive = true;

        // ���� ������Ʈ�� �ϰ��ǰ� �̷�������� Ȯ��
        Debug.Log("Title UI Active: " + isTitleUIActive);  // ���� ����
    }

    public void GameExit()
    {
        // ������ ����Ǿ� ���� ���� ��
        Application.Quit();

        // �����Ϳ��� ���� ���� ��
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

    //�¸� UI ��ȣ�ۿ�
    public void VictoryUiSet()
    {
        GameManager.Instance.isVictory = true;

        //��� ���� Ȯ��
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

        GameManager.Instance.SaveLevelData();   //Ŭ���� ������ ����

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

        //����ȭ������ ����
        TitleUI.SetActive(true);
        isTitleUIActive = true;
        TitleUiSet.SetActive(true);
        isTitleSetActive = true;

        StageSelectUISet.SetActive(false);
        isStageSelectactive = false;
        GameManager.Instance.GameSet.SetActive(false);
    }
    //�й� UI ��ȣ�ۿ�
    public void DefeatUISet()
    {
        GameManager.Instance.isDefeat = true;

        //��� ���� Ȯ��
        if(isAccelerate == true)
            SpeedUpSet();

        //�й� ȿ���� ���
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

        //����ȭ������ ����
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
        //�Ͻ����� �϶�� ������ ������ �ӽ÷� �Ͻ����� �� ���ӵ� ������Ű���� �ۼ�
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
    //������Ż Yes or No �޼��� �߰�
    private void BattleExitOk()
    {
        // ������ ����Ǿ� ���� ���� ��
        Application.Quit();

        // �����Ϳ��� ���� ���� ��
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

        // BGM ���Ұ� ���� ����
        AudioManager.Instance.backgroundAudioSource.volume = isBgmMute ? 0 : 1;

        // BGM ��ư ���� ����
        SetButtonColor(bgmMuteBtn, isBgmMute ? mutedColor : unmutedColor);
    }

    private void EffectMute()
    {
        isEffectClipMute = !isEffectClipMute;

        // ����Ʈ ���Ұ� ���� ����
        AudioManager.Instance.effectAudioSource.volume = isEffectClipMute ? 0 : 1;

        // ����Ʈ ��ư ���� ����
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
        // ��� ���¸� ������Ŵ
        isAccelerate = !isAccelerate;

        //��� ��ũ��Ʈ�� GameManager���� Bool������ ������ 

        // ��� ���¿� ���� ��ư ���� ����
        SetButtonColor(SpeedUpBtn, isAccelerate ? Color.green : Color.white);
    }

}
