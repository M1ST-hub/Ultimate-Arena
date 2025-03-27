[System.Serializable]
public class PlayerData
{

    public int level;
    public int tokens;
    public int xp;
    public int[] ownedBanners = new int[18];
    public int[] ownedIcons = new int[2];
    public string playerName;

    public PlayerData(Player player)
    {
        ownedIcons = player.ownedIcons;
        ownedBanners = player.ownedBanners;
        level = player.level;
        tokens = player.tokens;
        playerName = player.playerName;
 
    }

    public PlayerData()
    {
        level = 1;
        tokens = 0;
        playerName = "AnonymousPlayer";
        ownedIcons = new int[] { 1, 0 };
        ownedBanners = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 };
    }


}
