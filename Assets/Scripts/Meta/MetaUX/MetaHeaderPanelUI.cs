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
        if (SettingsButton != null) SettingsButton.onClick.AddListener(OnSettingsButtonClicked);

        dataManager = PlayerDataManager.Instance;
        if (dataManager == null) return;

        dataManager.OnDataChanged += Refresh;
        Refresh(dataManager.Data);
    }

    private void OnDisable()
    {
        if (dataManager != null) dataManager.OnDataChanged -= Refresh;
        dataManager = null;

        if (SettingsButton != null) SettingsButton.onClick.RemoveListener(OnSettingsButtonClicked);
    }

    private void Refresh(PlayerData data)
    {
        if (data == null) return;
        if (coinsText != null)
        {
            coinsText.text = data.coins.ToString();   
        }
    }

    private void OnSettingsButtonClicked()
    {
        if (PopupManager.Instance == null) return;
        if (PopupManager.Instance.IsPopupOpen<SettingsPopup>()) return; // don't stack a second one

        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Sfx.ButtonClick);
        PopupManager.Instance.ShowPopup<SettingsPopup>();
    }
}
