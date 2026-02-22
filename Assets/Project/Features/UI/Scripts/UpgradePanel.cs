using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePanel : MonoBehaviour
{
    [SerializeField] private UpgradeDataSO upgradeData;

    [Header("UI text elements")]
    [SerializeField] private TMPro.TextMeshProUGUI upgradeNameText;
    [SerializeField] private TMPro.TextMeshProUGUI upgradeLevelText;
    [SerializeField] private TMPro.TextMeshProUGUI upgradeCostText;
    [SerializeField] private TMPro.TextMeshProUGUI upgradeValueText;

    [Header ("Upgrade Button")]
    [SerializeField] private UnityEngine.UI.Button upgradeButton;
    
    [Header("Upgrade Icon")]
    [SerializeField] private UnityEngine.UI.Image upgradeIconImage;



    void Start()
    {
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        
        // DÜZELTME: UI'ı burada güncelle. 
        // Böylece GameDataManager veriyi yüklemeyi bitirmiş olur.
        UpdateUI(); 
    }

    private void OnUpgradeButtonClicked()
    {
        bool success = GameDataManager.Instance.TryBuyUpgrade(upgradeData);
        if (success)
        {
            // Event publish
            UpdateUI();
        }
    }

void UpdateUI()
    {
        upgradeNameText.text = upgradeData.upgradeName;
        upgradeLevelText.text = $"Lvl {upgradeData.currentLevel}";
        upgradeValueText.text = "+" + upgradeData.GetCurrentValue().ToString("F1");
        upgradeIconImage.sprite = upgradeData.icon;
        

        if (upgradeData.CanLevelUp())
        {
            upgradeCostText.text = "$" + upgradeData.GetCurrentCost().ToString("F0");
            upgradeButton.interactable = true;
        }
        else
        {
            upgradeCostText.text = "MAX";
            upgradeButton.interactable = false;
        }
    }

}
