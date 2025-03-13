using UnityEngine;

[System.Serializable]
public class PlayerData {

    public int level;
    public int score;

    public PlayerData (Player player)
    {
        level = player.level;
        score = player.score;
    }
    

    

}
