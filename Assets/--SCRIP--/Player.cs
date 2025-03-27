using System.Globalization;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int level;
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

    public void SavePlayer ()
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

        foreach (int i in ownedIcons)
        {
            Debug.Log(i);
        }
        

        ////Icons--------------------------------------------------------------------------------
        if (data.ownedIcons == null || data.ownedIcons.Length != 2)
        {
            Debug.Log("NULL");
            ownedIcons = new int[] { 1, 0};
        }
        else if (data.ownedIcons.Length != ownedIcons.Length)
        {
            Debug.Log("DIDNT MATCH LENGTH");
            int[] temp = new int[2];
            ownedIcons.CopyTo(temp, 0);
            ownedIcons = temp;
            ownedIcons = data.ownedIcons;
        }
        else
        {
            Debug.Log("LOADED PLAYER DATA");
            ownedIcons = data.ownedIcons;
        }

        ////BANNERS--------------------------------------------------------------------------------
        if (data.ownedBanners == null || data.ownedBanners.Length != 18)
        {
            ownedBanners = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 };
        }
        else if (data.ownedBanners.Length != ownedBanners.Length)
        {
            int[] temp = new int[18];
            ownedBanners.CopyTo(temp, 0);
            ownedBanners = temp;
            ownedBanners = data.ownedBanners;
        }
        else
        {
            ownedBanners = data.ownedBanners;
        }

        playerName = data.playerName;
    }

    public void SetPlayerName(string newName)
    {
        playerName = newName;
    }

}
