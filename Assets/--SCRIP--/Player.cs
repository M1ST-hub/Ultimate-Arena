using UnityEngine;

public class Player : MonoBehaviour
{
    public int level;
    public int xp;
    public int tokens;
    public int[] ownedBanners = new int[19];
    public int[] ownedIcons = new int[2];
    public string playerName;
    public int currentBanner;

    public static Player Instance;

    [InspectorButton("DeletePlayer")]
        public bool deletePlayer;

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
        else if (data.ownedIcons.Length != ownedIcons.Length)
        {
            int[] temp = new int[19];
            ownedIcons.CopyTo(temp, 0);
            ownedIcons = temp;
            ownedIcons = data.ownedIcons;
        }
        else
        {
            ownedIcons = data.ownedIcons;
        }

        // Banners handling - no longer necessary, as SetBannerOwnership will handle the updates
        if (data.ownedBanners == null || data.ownedBanners.Length != 19)
        {
            ownedBanners = new int[19];
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
            // Default owned banners if the data is null
            ownedBanners = data.ownedBanners; // Default to all unowned
                                              // Example: Set some as owned by default
        }

        ownedBanners[6] = ownedBanners[7] = ownedBanners[9] = ownedBanners[13] = ownedBanners[17] = 1;
        currentBanner = data.currentBanner;
        playerName = data.playerName;
    }

    public void SetPlayerName(string newName)
    {
        playerName = newName;
    }

}
