using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHeaderPanelUI : MonoBehaviour
{
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button hintButton;

    [SerializeField] private TextMeshProUGUI coinCount;
    [SerializeField] private TextMeshProUGUI levelId;

    private void OnEnable()
    {
        if (pauseButton != null) pauseButton.onClick.AddListener(OnPauseButtonClicked);
        if (hintButton != null) hintButton.onClick.AddListener(OnHintButtonClicked);

        // The level (and its CoinManager) is spawned after this header, so listen for changes
        // instead of reading once. Also show the current values if the level already exists.
        CoinManager.OnCoinsChanged += UpdateCoinText;
        LevelManager.OnLevelStarted += UpdateLevelId;

        if (CoinManager.Instance != null)
        {
            UpdateCoinText(CoinManager.Instance.CollectedCoins, CoinManager.Instance.TotalCoins);
        }
        else
        {
            UpdateCoinText(0, 0);
        }

        if (LevelManager.Instance != null && LevelManager.Instance.CurrentLevelNumber > 0)
        {
            UpdateLevelId(LevelManager.Instance.CurrentLevelNumber);
        }
    }

    private void OnDisable()
    {
        if (pauseButton != null) pauseButton.onClick.RemoveListener(OnPauseButtonClicked);
        if (hintButton != null) hintButton.onClick.RemoveListener(OnHintButtonClicked);
        CoinManager.OnCoinsChanged -= UpdateCoinText;
        LevelManager.OnLevelStarted -= UpdateLevelId;
    }

    private void UpdateLevelId(int levelNumber)
    {
        if (levelId != null) levelId.text = "LEVEL " + levelNumber;
    }

    private void UpdateCoinText(int collected, int total)
    {
        if (coinCount != null) coinCount.text = $"{collected} / {total}";
    }

    private void OnHintButtonClicked()
    {
        if (PopupManager.Instance == null) return;

        // Same rules as pause: only while playing, and never stack a second hint popup.
        CoreGameManager game = CoreGameManager.Instance;
        if (game != null && !game.IsPlaying) return;
        if (PopupManager.Instance.IsPopupOpen<LevelHintPanel>()) return;

        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Sfx.ButtonClick);

        PopupManager.Instance.ShowPopup<LevelHintPanel>();
    }

    private void OnPauseButtonClicked()
    {
        if (PopupManager.Instance == null) return;

        // Only pause while playing, and never stack a second pause popup.
        CoreGameManager game = CoreGameManager.Instance;
        if (game != null && !game.IsPlaying) return;
        if (PopupManager.Instance.IsPopupOpen<PausePopup>()) return;

        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Sfx.ButtonClick);

        // PausePopup pauses the game itself when it opens.
        PopupManager.Instance.ShowPopup<PausePopup>();
    }
}
