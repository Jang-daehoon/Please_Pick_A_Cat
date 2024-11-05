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

    [Header("UIActive Set")]
    [SerializeField] private bool isTitleUIActive;
    [SerializeField] private bool isTitleSetActive;
    [SerializeField] private bool isStageSelectactive;

    [SerializeField] private bool isVictoryUiActive;  //�¸�UI Ȱ��ȭ / ��Ȱ��ȭ
    [SerializeField] private bool isDefeatUiActive;   //�й� UI Ȱ��ȭ / ��Ȱ��ȭ
    [SerializeField] private bool isOptionActive;  //�ɼ� Ȱ��ȭ / ��Ȱ��ȭ
    [SerializeField] private bool isResumeActive;    //�Ͻ����� Ȱ��ȭ / ��Ȱ��ȭ
    [SerializeField] private bool isExitUiActive;    //������Ż UIȰ��ȭ / ��Ȱ��ȭ
    [SerializeField] private bool isInGameUIActive;  //�ΰ��� UIȰ��ȭ / ��Ȱ��ȭ

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

        battleExitBtn.onClick.AddListener(ExitButtonClick);
        battleExitYesBtn.onClick.AddListener(BattleExitOk);
        battleExitNoBtn.onClick.AddListener(BattleExitCancel);
    }

    private void Update()
    { 

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
}
