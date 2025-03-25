using UnityEngine;

public class Player : MonoBehaviour
{
    public int level = 0;
    public int tokens;
    public int[] ownedBanners;
    public int[] ownedIcons;

    public void SavePlayer ()
    {
        PlayerSaveManager.SavePlayer(this);
    }

    public void LoadPlayer()
    {
        PlayerData data = PlayerSaveManager.LoadPlayer();

        level = data.level;
        tokens = data.tokens;
        ownedBanners = data.ownedBanners;
        ownedIcons = data.ownedIcons;
    }

}
