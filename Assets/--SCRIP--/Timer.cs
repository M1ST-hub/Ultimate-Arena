using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using static UnityEngine.CullingGroup;
using JetBrains.Annotations;
using System.Threading;

public class Timer : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainingTime;

    public GameManager gm;
    public bool gameStart;
    public bool gameEnd;
    public bool isSpawned;
    public bool preGameTimer;
    public bool postGameTimer;
    public bool gameInProgress;
    public bool gameTimer;

    public NetworkVariable<float> Clock = new NetworkVariable<float>();

    private void Start()
    {
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isSpawned == true) 
            CountdownRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void CountdownRpc()
    {   
        preGameTimer = true;
        postGameTimer = false;
        gameTimer = false;
        gameStart = false;
        gameEnd = false;
        gameInProgress = false;

        //countdown timer
        if (Clock.Value > 0 && IsServer)
            Clock.Value -= Time.deltaTime;
        //stop timer at 0
        else if (Clock.Value < 1 && IsServer)
        {
            Clock.Value = 0;

            if (preGameTimer == true && gameStart == false && gameInProgress == false)
            {
                gm.GameStartRpc();
            }
            else if (preGameTimer == false && gameStart == true && gameTimer == true)
            {
                
                gameTimer = false;
            }
            else if (preGameTimer == false && postGameTimer == false && gameStart == true && gameInProgress == true && gameTimer == false)
            {
                gm.DestroyTimmyRpc();
                postGameTimer = true;
                gameStart = false;
                gameEnd = true;
                gm.GameEndRpc();
            }


        }

        
    }

    public void UpdateGame()
    {
        preGameTimer = false;
        gameStart = true;
        gameInProgress = true;
    }



    public override void OnNetworkSpawn()
    {
        Clock.OnValueChanged += OnStateChange;

        isSpawned = true;

        
        if (IsServer)
        {
           Clock.Value = 30;
        }
    }

    public override void OnNetworkDespawn()
    {
        Clock.OnValueChanged -= OnStateChange;
    }

    public void OnStateChange(float previous,  float current)
    {
            int minutes = Mathf.FloorToInt(Clock.Value / 60);
            int seconds = Mathf.FloorToInt(Clock.Value % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

}
