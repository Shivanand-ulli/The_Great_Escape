using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BootstrapLoadingUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider loadingSlider;

    private Coroutine loadingCoroutine;

    /// <summary>
    /// Starts the loading animation
    /// </summary>
    /// <param name="duration">Duration in seconds (should match Bootstrap minimumLoadTime)</param>
    public void StartLoading(float duration)
    {
        if (loadingSlider == null)
        {
            Debug.LogWarning("[BootstrapLoadingUI] Loading slider is not assigned!");
            return;
        }

        // Stop any existing loading animation
        if (loadingCoroutine != null)
        {
            StopCoroutine(loadingCoroutine);
        }

        // Start new loading animation
        loadingCoroutine = StartCoroutine(AnimateSlider(duration));
    }

    /// <summary>
    /// Animates the slider from 0 to 1 over the specified duration
    /// </summary>
    private IEnumerator AnimateSlider(float duration)
    {
        float elapsed = 0f;
        loadingSlider.value = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            loadingSlider.value = progress;
            yield return null;
        }

        loadingSlider.value = 1f;
        loadingCoroutine = null;
    }

    /// <summary>
    /// Manually set the slider progress (0-1)
    /// </summary>
    /// <param name="progress">Progress value between 0 and 1</param>
    public void SetProgress(float progress)
    {
        if (loadingSlider != null)
        {
            if (loadingCoroutine != null)
            {
                StopCoroutine(loadingCoroutine);
                loadingCoroutine = null;
            }
            loadingSlider.value = Mathf.Clamp01(progress);
        }
    }

    /// <summary>
    /// Hides the loading screen
    /// </summary>
    public void HideLoadingScreen()
    {
        if (loadingCoroutine != null)
        {
            StopCoroutine(loadingCoroutine);
            loadingCoroutine = null;
        }

        gameObject.SetActive(false);
    }
}
