using UnityEngine;
using UnityEngine.UI;

/// <summary>Footer navigation buttons. Each switches the Meta UX to its screen.</summary>
public class MetaFooterPanelUI : MonoBehaviour
{
    [SerializeField] private Button homeButton;
    [SerializeField] private Button levelSelectButton;
    [SerializeField] private Button settingsButton;

    private void OnEnable()
    {
        Bind(homeButton, MetaScreenType.Home);
        Bind(levelSelectButton, MetaScreenType.LevelSelect);
        Bind(settingsButton, MetaScreenType.Settings);
    }

    private void OnDisable()
    {
        Unbind(homeButton);
        Unbind(levelSelectButton);
        Unbind(settingsButton);
    }

    private static void Bind(Button button, MetaScreenType screen)
    {
        if (button == null) return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Sfx.ButtonClick);
            if (MetaUXController.Instance != null) MetaUXController.Instance.SwitchScreen(screen);
        });
    }

    private static void Unbind(Button button)
    {
        if (button != null) button.onClick.RemoveAllListeners();
    }
}
