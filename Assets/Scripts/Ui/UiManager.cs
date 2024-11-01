using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI의 활성화 비활성화 및 상태 참조 매니저
/// </summary>
public class UiManager : MonoBehaviour
{
    public static UiManager Instance;
    [Header("UI Objects")]
    [SerializeField] private GameObject inGameUi;
    [SerializeField] private GameObject victoryUi;
    [SerializeField] private GameObject defeatUi;
    [SerializeField] private GameObject resume_OptionUi;
    [SerializeField] private GameObject exitUi;

    [Header("UIActive Set")]
    [SerializeField] private bool isVictoryUiActive;  //승리UI 활성화 / 비활성화
    [SerializeField] private bool isDefeatUiActive;   //패배 UI 활성화 / 비활성화
    [SerializeField] private bool isOptionActive;  //옵션 활성화 / 비활성화
    [SerializeField] private bool isResumeActive;    //일시정지 활성화 / 비활성화
    [SerializeField] private bool isExitUiActive;    //전투이탈 UI활성화 / 비활성화
    [SerializeField] private bool isInGameUIActive;  //인게임 UI활셩화 / 비활성화

    [Header("UIObjectButtons")]
    [SerializeField] private Button victoryButton;
    [SerializeField] private Button defeatButton;
    [SerializeField] private Button battleExitBtn;
    [SerializeField] private Button battleExitYesBtn;
    [SerializeField] private Button battleExitNoBtn;
    [SerializeField] private Button optionButton;
    [Header("UI Particle")]
    public ParticleSystem RealodingParticle;
    public ParticleSystem ReloadingDoneParticle;

    private void Awake()
    {
        Instance = this;

        RealodingParticle.Stop();
        ReloadingDoneParticle.Stop();

        victoryButton.onClick.AddListener(VictoryOkClick);
        defeatButton.onClick.AddListener(DefeatOkClick);
        battleExitBtn.onClick.AddListener(ExitButtonClick);
    }

    private void Update()
    {
        optionButton.onClick.AddListener(OptionUISet);

        if (GameManager.Instance.isDefeat == true)
        {
            inGameUi.SetActive(false);
            isInGameUIActive = false;

            defeatUi.SetActive(true);
            isDefeatUiActive = true;
        }
        else if(GameManager.Instance.isVictory == true)
        {
            inGameUi.SetActive(false);
            isInGameUIActive = false;

            victoryUi.SetActive(true);
            isVictoryUiActive = false;
        }
    }

    //승리 UI 상호작용
    public void VictoryUiSet()
    {
        victoryUi.SetActive(true);
        isVictoryUiActive = true;
    }
    private void VictoryOkClick()
    {
        GameManager.Instance.isVictory = false;
        victoryUi.SetActive(false);
        isVictoryUiActive = false;

        inGameUi.SetActive(true);
        isInGameUIActive = true;

        //메인화면으로 복귀
    }
    //패배 UI 상호작용
    public void DefeatUISet()
    {
        defeatUi.SetActive(true);
        isDefeatUiActive = true;
    }   
    private void DefeatOkClick()
    {
        GameManager.Instance.isDefeat = false;
        defeatUi.SetActive(false);
        isDefeatUiActive=false;

        inGameUi.SetActive(true);
        isInGameUIActive = true;

        //메인화면으로 복귀
    }
    public void OptionUISet()
    {
        //일시정지 하라는 내용은 없지만 임시로 일시정지 시 게임도 정지시키도록 작성
        Time.timeScale = 0;
        resume_OptionUi.SetActive(true);
        isOptionActive = true;
    }
    private void ExitButtonClick()
    {
        exitUi.gameObject.SetActive(true);
        isExitUiActive = true;
        resume_OptionUi.SetActive(false);
        isOptionActive = false;
    }

    //기획서 상에 Option 종료, 전투 이탈에 대한 내용이 없다.
}
