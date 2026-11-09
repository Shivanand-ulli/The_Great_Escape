using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Header widgets: player name and coin/gem/key counts, kept in sync with PlayerData.</summary>
public class MetaHeaderPanelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private Button SettingsButton;

    private PlayerDataManager dataManager;

    private void OnEnable()
    {
        dataManager = PlayerDataManager.Instance;
        if (dataManager == null) return;

        dataManager.OnDataChanged += Refresh;
        Refresh(dataManager.Data);

        SettingsButton.onClick.AddListener(OnSettingsButtonClicked);
    }

    private void OnDisable()
    {
        if (dataManager != null) dataManager.OnDataChanged -= Refresh;
        dataManager = null;
        SettingsButton.onClick.RemoveListener(OnSettingsButtonClicked);
    }

    private void Refresh(PlayerData data)
    {
        if (data == null) return;
        if (coinsText != null) coinsText.text = data.coins.ToString();
    }

    private void OnSettingsButtonClicked()
    {
        if(PopupManager.Instance != null)
        {
            PopupManager.Instance.ShowPopup<SettingsPopup>();
        }
    }
}
