using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelFailedPopup : PopupBase
{
    [Header("Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button homeButton;

    [Header("Texts (optional)")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI coinCount;

    [Header("Scene")]
    [Tooltip("Scene opened by the Home button. Must be in Build Settings.")]
    [SerializeField] private string homeSceneName = "Meta";

    protected override void OnInit()
    {
        AddListener(restartButton, OnRestartClicked);
        AddListener(homeButton, OnHomeClicked);

        if (levelText != null && LevelManager.Instance != null)
        {
            levelText.text = $"Level {LevelManager.Instance.CurrentLevelNumber}";
        }

        if(CoinManager.Instance != null)
        {
            coinCount.text = CoinManager.Instance.CollectedCoins.ToString();
        }
    }

    protected override void OnDestroy()
    {
        RemoveListener(restartButton, OnRestartClicked);
        RemoveListener(homeButton, OnHomeClicked);

        base.OnDestroy();
    }

    private void OnRestartClicked()
    {
        PlayClick();

        // Natural break before gameplay starts again: Poki may show an ad here.
        PokiManager.CommercialBreak(() =>
        {
            // RestartLevel respawns the level and sets the game back to Playing at normal speed.
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.RestartLevel();
            }
            else if (CoreGameManager.Instance != null)
            {
                CoreGameManager.Instance.StartGame();
            }

            Close();
        });
    }

    private void OnHomeClicked()
    {
        PlayClick();

        if (CoreGameManager.Instance != null) CoreGameManager.Instance.ResetState();
        SceneManager.LoadScene(homeSceneName);
    }

    private static void PlayClick()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Sfx.ButtonClick);
    }

    private static void AddListener(Button button, UnityAction action)
    {
        if (button != null) button.onClick.AddListener(action);
    }

    private static void RemoveListener(Button button, UnityAction action)
    {
        if (button != null) button.onClick.RemoveListener(action);
    }
}
