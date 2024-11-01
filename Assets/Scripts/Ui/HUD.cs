using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    private TextMeshProUGUI myText;

    public enum uiType { StageName, Cost, walletLevel, walletUpCost, LaserGauge, CharacterCost, spawnCoolTime }
    public uiType UiType;
    [SerializeField] private int buttonId;
    private int WalletLevel;
    private void Awake()
    {
        myText = GetComponent<TextMeshProUGUI>();
    }
    private void LateUpdate()
    {
        switch (UiType)
        {
            case uiType.StageName:
                myText.text = GameManager.Instance.StageName;
                break;
            case uiType.Cost:
                int curCost = GameManager.Instance.curCost;
                int maxCost = GameManager.Instance.maxCost;
                myText.text = ($"{curCost} / {maxCost}");
                break;
            case uiType.walletLevel:
                WalletLevel = GameManager.Instance.CurLevel;
                myText.text = ($"Lv.{WalletLevel}");
                break;
            case uiType.walletUpCost:
                // 최대 레벨 체크
                if (GameManager.Instance.CurLevel >= GameManager.Instance.levelUpCost.Length)
                {
                    myText.text = "Max Level"; // 최대 레벨에 도달
                }
                else
                {
                    int walletUpgradeCost = GameManager.Instance.levelUpCost[GameManager.Instance.CurLevel];
                    myText.text = $"{walletUpgradeCost}";
                }
                break;
            case uiType.CharacterCost:
                if (buttonId == 0)
                {
                    myText.text = GameManager.Instance.playerUnits[buttonId].UsedCost.ToString();
                }
                else if (buttonId == 1)
                {
                    myText.text = GameManager.Instance.playerUnits[buttonId].UsedCost.ToString();
                }
                else if (buttonId == 2)
                {
                    myText.text = GameManager.Instance.playerUnits[buttonId].UsedCost.ToString();
                }
                else if (buttonId == 3)
                {
                    myText.text = GameManager.Instance.playerUnits[buttonId].UsedCost.ToString();
                }
                break;
        }
    }
}
