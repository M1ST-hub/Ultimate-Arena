using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainingTime;

    public GameManager gm;
    public bool gameStart;
    public bool gameEnd;


    // Update is called once per frame
    void Update()
    {
        //countdown timer
        if (remainingTime > 0)
            remainingTime -= Time.deltaTime;
        //stop timer at 0
        else if (remainingTime < 1)
        {
            remainingTime = 0;

            if (gameStart == false)
            {
                gameStart = true;
                gm.GameStartRpc();
            }

        }
            

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
