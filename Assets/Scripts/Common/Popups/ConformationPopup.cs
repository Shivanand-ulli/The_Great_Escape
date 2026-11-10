using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConformationPopup : PopupBase
{
    [Header("Buttons")]
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    [Header("Scene")]
    [Tooltip("Scene opened by the Home button. Must be in Build Settings.")]
    [SerializeField] private string homeSceneName = "Meta";

    private void OnEnable()
    {
        if (yesButton != null) yesButton.onClick.AddListener(OnYesButtonClicked);
        if (noButton != null) noButton.onClick.AddListener(OnNoButtonClicked);
    }

    private void OnDisable()
    {
        if (yesButton != null) yesButton.onClick.RemoveListener(OnYesButtonClicked);
        if (noButton != null) noButton.onClick.RemoveListener(OnNoButtonClicked);
    }

    private void OnYesButtonClicked()
    {
        // Close this popup and every other open popup (for example the pause popup behind it).
        if (PopupManager.Instance != null) PopupManager.Instance.CloseAll();

        // Back to normal time before leaving, otherwise the next scene would start frozen.
        if (CoreGameManager.Instance != null) CoreGameManager.Instance.ResetState();
        SceneManager.LoadScene(homeSceneName);
    }

    private void OnNoButtonClicked()
    {
        Close();
    }
}
