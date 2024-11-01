using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI�� Ȱ��ȭ ��Ȱ��ȭ �� ���� ���� �Ŵ���
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
    [SerializeField] private bool isVictoryUiActive;  //�¸�UI Ȱ��ȭ / ��Ȱ��ȭ
    [SerializeField] private bool isDefeatUiActive;   //�й� UI Ȱ��ȭ / ��Ȱ��ȭ
    [SerializeField] private bool isOptionActive;  //�ɼ� Ȱ��ȭ / ��Ȱ��ȭ
    [SerializeField] private bool isResumeActive;    //�Ͻ����� Ȱ��ȭ / ��Ȱ��ȭ
    [SerializeField] private bool isExitUiActive;    //������Ż UIȰ��ȭ / ��Ȱ��ȭ
    [SerializeField] private bool isInGameUIActive;  //�ΰ��� UIȰ��ȭ / ��Ȱ��ȭ

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

    //�¸� UI ��ȣ�ۿ�
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

        //����ȭ������ ����
    }
    //�й� UI ��ȣ�ۿ�
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

        //����ȭ������ ����
    }
    public void OptionUISet()
    {
        //�Ͻ����� �϶�� ������ ������ �ӽ÷� �Ͻ����� �� ���ӵ� ������Ű���� �ۼ�
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

    //��ȹ�� �� Option ����, ���� ��Ż�� ���� ������ ����.
}
