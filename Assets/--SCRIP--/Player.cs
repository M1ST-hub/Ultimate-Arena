using UnityEngine;

public class Player : MonoBehaviour
{
    public int level = 0;
    public int score;

    public void SavePlayer ()
    {
        PlayerSaveManager.SavePlayer(this);
    }

    public void LoadPlayer()
    {
        PlayerData data = PlayerSaveManager.LoadPlayer();

        level = data.level;
        score = data.score;
    }

}
