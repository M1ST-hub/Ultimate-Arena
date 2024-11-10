using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainingTime;

    public GameManager gm;
   


    // Update is called once per frame
    void Update()
    {
        //countdown timer
        if (remainingTime > 0)
            remainingTime -= Time.deltaTime;
        //stop timer at 0
        else if (remainingTime <= 0)
        {
            remainingTime = 0;
        }
            

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
