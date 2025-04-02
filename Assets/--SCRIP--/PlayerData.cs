using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{

    public int level;
    public int tokens;
    public int xp;
    public int[] ownedBanners = new int[19];
    public int[] ownedIcons = new int[2];
    public string playerName;

    public List<ItemData> cosmeticItems = new List<ItemData>(); // Add this line

    public PlayerData(Player player)
    {
        ownedIcons = player.ownedIcons;
        ownedBanners = player.ownedBanners;
        level = player.level;
        tokens = player.tokens;
        playerName = player.playerName;
        xp = player.xp;
        cosmeticItems = player.cosmeticItems;
    }

    public PlayerData()
    {
        level = 1;
        xp = 0;
        tokens = 0;
        playerName = "AnonymousPlayer";
        ownedIcons = new int[] { 1, 0 };
        ownedBanners = new int[] { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    }


}
