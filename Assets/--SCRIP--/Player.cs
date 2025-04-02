using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int level;
    public int xp;
    public int tokens;
    public int[] ownedBanners = new int[19];
    public int[] ownedIcons = new int[2];
    public string playerName;

    public List<ItemData> cosmeticItems = new List<ItemData>(); // Add this line

    public static Player Instance;

    private void Awake()
    {
        if (Instance != null && this != Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

    }

    private void Start()
    {
        LoadPlayer();
    }

    [ContextMenu("DeletePlayer")]
    public void DeletePlayer()
    {
        PlayerSaveManager.DeletePlayer();
    }

    public void SavePlayer()
    {
        Debug.Log("Saving tokens: " + tokens);
        PlayerSaveManager.SavePlayer(this);
    }

    public void LoadPlayer()
    {
        PlayerData data = PlayerSaveManager.LoadPlayer();

        if (data != null)
        {

            if (data.level == 0)
                level = 1;
            else
                level = data.level;
            Debug.Log("Loaded tokens: " + data.tokens);
            tokens = data.tokens;

            Debug.Log("Player tokens after loading: " + tokens);

            xp = data.xp;
        }
        else
        {
            Debug.LogError("Player data is null during load.");
        }

            // Icons handling
            if (data.ownedIcons == null || data.ownedIcons.Length != 2)
        {
            Debug.Log("NULL or invalid ownedIcons data");
            ownedIcons = new int[] { 1, 0 };
        }
        else if (data.ownedBanners.Length != ownedBanners.Length)
        {
            int[] temp = new int[19];
            ownedBanners.CopyTo(temp, 0);
            ownedBanners = temp;
            ownedBanners = data.ownedBanners;
        }
        else
        {
            ownedIcons = data.ownedIcons;
        }

        // Banners handling - no longer necessary, as SetBannerOwnership will handle the updates
        if (data.ownedBanners != null)
        {
            ownedBanners = data.ownedBanners;
        }
        else
        {
            // Default owned banners if the data is null
            ownedBanners = new int[19];  // Default to all unowned
             // Example: Set some as owned by default
        }

        ownedBanners[6] = ownedBanners[7] = ownedBanners[9] = ownedBanners[13] = ownedBanners[17] = 1;

        playerName = data.playerName;
        cosmeticItems = data.cosmeticItems;
    }

    public void SetPlayerName(string newName)
    {
        playerName = newName;
    }

    public void SetBannerOwnership(int bannerIndex, bool isOwned)
    {
        ownedBanners[bannerIndex] = isOwned ? 1 : 0;
        // Also update the cosmeticItems list
        if (cosmeticItems.Count > bannerIndex)
        {
            cosmeticItems[bannerIndex].isPurchased = isOwned;
        }
    }

}
