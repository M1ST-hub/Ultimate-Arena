using UnityEngine;

[System.Serializable]
public class PlayerData {

    public int level;
    public int tokens;
    public int xp;
    public int[] ownedBanners;
    public int[] ownedIcons;

    public PlayerData (Player player)
    {
        level = player.level;
        tokens = player.tokens;
        ownedIcons = player.ownedIcons;
        ownedBanners = player.ownedBanners;
    }
    

    

}
