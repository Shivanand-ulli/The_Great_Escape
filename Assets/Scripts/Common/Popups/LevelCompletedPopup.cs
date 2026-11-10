using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelCompletedPopup : PopupBase
{
    [Header("Buttons")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button homeButton;

    [Header("Texts (optional)")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI coinText;

    [Header("Scene")]
    [Tooltip("Scene opened by the Home button. Must be in Build Settings.")]
    [SerializeField] private string homeSceneName = "Meta";

    protected override void OnInit()
    {
        AddListener(nextButton, OnNextClicked);
        AddListener(restartButton, OnRestartClicked);
        AddListener(homeButton, OnHomeClicked);

        LevelManager levelManager = LevelManager.Instance;

        if (levelManager != null)
        {
            if (levelText != null) levelText.text = $"Level {levelManager.CurrentLevelNumber}";
            if (coinText != null && CoinManager.Instance != null) coinText.text = CoinManager.Instance.CollectedCoins.ToString();
        }
    }

    protected override void OnDestroy()
    {
        RemoveListener(nextButton, OnNextClicked);
        RemoveListener(restartButton, OnRestartClicked);
        RemoveListener(homeButton, OnHomeClicked);

        base.OnDestroy();
    }

    private void OnNextClicked()
    {
        PlayClick();

        // Natural break before gameplay starts again: Poki may show an ad here.
        PokiManager.CommercialBreak(() =>
        {
            if (LevelManager.Instance != null && LevelManager.Instance.LoadNextLevel())
            {
                Close();
            }
        });
    }

    private void OnRestartClicked()
    {
        PlayClick();

        PokiManager.CommercialBreak(() =>
        {
            if (LevelManager.Instance != null) LevelManager.Instance.RestartLevel();
            Close();
        });
    }

    private void OnHomeClicked()
    {
        PlayClick();
        Close();
        GoHome();
    }

    private void GoHome()
    {
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
