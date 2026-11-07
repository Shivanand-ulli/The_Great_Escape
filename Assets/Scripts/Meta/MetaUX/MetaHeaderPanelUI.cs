using TMPro;
using UnityEngine;

/// <summary>Header widgets: player name and coin/gem/key counts, kept in sync with PlayerData.</summary>
public class MetaHeaderPanelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text gemsText;
    [SerializeField] private TMP_Text keysText;

    private PlayerDataManager dataManager;

    private void OnEnable()
    {
        dataManager = PlayerDataManager.Instance;
        if (dataManager == null) return;

        dataManager.OnDataChanged += Refresh;
        Refresh(dataManager.Data);
    }

    private void OnDisable()
    {
        if (dataManager != null) dataManager.OnDataChanged -= Refresh;
        dataManager = null;
    }

    private void Refresh(PlayerData data)
    {
        if (data == null) return;

        if (playerNameText != null) playerNameText.text = data.playerName;
        if (coinsText != null) coinsText.text = data.coins.ToString();
        if (gemsText != null) gemsText.text = data.gems.ToString();
        if (keysText != null) keysText.text = data.keys.ToString();
    }
}
