using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Per-level coin counter. Coins register themselves, so nothing is searched for by tag.
/// When every coin is collected the key is revealed.
/// </summary>
public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    /// <summary>
    /// Fired with (collected, total) whenever the counts change. Static so UI that exists before
    /// the level is spawned (like the game header) can subscribe without needing the instance.
    /// </summary>
    public static event Action<int, int> OnCoinsChanged;

    [SerializeField] private GameObject key;
    [SerializeField] private TextMeshProUGUI cointext;

    /// <summary>Coins collected in this level run.</summary>
    public int coinPoints = 0;

    private readonly List<Coin> coins = new List<Coin>();
    private int collectedCoins;
    private bool allCoinsHidden;
    private bool coinsCommitted;

    public int TotalCoins => coins.Count;
    public int CollectedCoins => collectedCoins;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Start()
    {
        if (key != null) key.SetActive(false);
    }

    /// <summary>Called by each Coin in its Start.</summary>
    public void RegisterCoin(Coin coin)
    {
        coins.Add(coin);
        NotifyChanged();
    }

    public void CollectCoin()
    {
        collectedCoins++;
        coinPoints++;
        NotifyChanged();

        if (collectedCoins >= coins.Count) RevealKey();
    }

    public void ResetScore()
    {
        coinPoints = 0;
        collectedCoins = 0;
        coinsCommitted = false;
        NotifyChanged();
    }

    /// <summary>Hides every coin and counts them all as collected (used by power-ups).</summary>
    public void HideCoins()
    {
        if (allCoinsHidden) return;
        allCoinsHidden = true;

        for (int i = 0; i < coins.Count; i++)
        {
            if (coins[i] != null) coins[i].gameObject.SetActive(false);
        }

        collectedCoins = coins.Count;
        coinPoints = coins.Count;
        NotifyChanged();
        RevealKey();
    }

    /// <summary>Adds this level's coins to the player's total. Safe to call more than once.</summary>
    public void CommitCoins()
    {
        if (coinsCommitted) return;
        coinsCommitted = true;

        if (PlayerDataManager.Instance != null && coinPoints > 0)
        {
            PlayerDataManager.Instance.AddCoins(coinPoints);
        }
    }

    private void NotifyChanged()
    {
        OnCoinsChanged?.Invoke(collectedCoins, coins.Count);
    }

    private void RevealKey()
    {
        if (key != null) key.SetActive(true);
    }
}
