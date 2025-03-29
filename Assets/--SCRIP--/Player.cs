using UnityEngine;

public class Player : MonoBehaviour
{
    public int level;
    public int xp;
    public int tokens;
    public int[] ownedBanners = new int[18];
    public int[] ownedIcons = new int[2];
    public string playerName;

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

    public void SavePlayer()
    {
        PlayerSaveManager.SavePlayer(this);
    }

    public void LoadPlayer()
    {
        PlayerData data = PlayerSaveManager.LoadPlayer();

        if (data.level == 0)
            level = 1;
        else
            level = data.level;
        tokens = data.tokens;
        xp = data.xp;

        // Icons handling
        if (data.ownedIcons == null || data.ownedIcons.Length != 2)
        {
            Debug.Log("NULL or invalid ownedIcons data");
            ownedIcons = new int[] { 1, 0 };
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
            ownedBanners = new int[18];  // Default to all unowned
            ownedBanners[0] = ownedBanners[1] = ownedBanners[2] = ownedBanners[3] = ownedBanners[4] = ownedBanners[5] = ownedBanners[6] = ownedBanners[7] = 1; // Example: Set some as owned by default
        }

        playerName = data.playerName;
    }

    public void SetPlayerName(string newName)
    {
        playerName = newName;
    }

    public void SetBannerOwnership(int bannerIndex, bool isOwned)
    {
        ownedBanners[bannerIndex] = isOwned ? 1 : 0;
    }

}
