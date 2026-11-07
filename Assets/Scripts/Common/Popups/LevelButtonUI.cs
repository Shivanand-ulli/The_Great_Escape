using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum LevelStatus
{
    Locked,
    Active,     // unlocked, not finished yet (the level to play)
    Completed
}

/// <summary>A single level button: shows the level number and its status, and reports clicks.</summary>
[RequireComponent(typeof(Button))]
public class LevelButtonUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text levelNumberText;

    [Header("Status Sprites")]
    [Tooltip("Image that shows the status sprite. Leave empty to use the Image on this button.")]
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite completedSprite;

    private int levelNumber;
    private Action<int> onClicked;

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        button.onClick.AddListener(HandleClick);
    }

    private void OnDestroy()
    {
        if (button != null) button.onClick.RemoveListener(HandleClick);
    }

    /// <param name="number">1-based level number.</param>
    /// <param name="status">Locked buttons can't be pressed.</param>
    /// <param name="clickCallback">Called with the level number when the button is pressed.</param>
    public void Setup(int number, LevelStatus status, Action<int> clickCallback)
    {
        levelNumber = number;
        onClicked = clickCallback;

        if (levelNumberText != null) levelNumberText.text = number.ToString();

        if (button == null) button = GetComponent<Button>();
        button.interactable = status != LevelStatus.Locked;

        if (buttonImage == null) buttonImage = GetComponent<Image>();
        if (buttonImage != null)
        {
            Sprite sprite = GetSprite(status);
            if (sprite != null)
            {
                buttonImage.sprite = sprite;
                buttonImage.SetNativeSize();
            }
        }
    }

    private Sprite GetSprite(LevelStatus status)
    {
        switch (status)
        {
            case LevelStatus.Active: return activeSprite;
            case LevelStatus.Completed: return completedSprite;
            default: return lockedSprite;
        }
    }

    private void HandleClick()
    {
        onClicked?.Invoke(levelNumber);
    }
}
