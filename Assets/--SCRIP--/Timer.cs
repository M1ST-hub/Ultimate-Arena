using UnityEngine;
using TMPro;
using Unity.Netcode;


public class Timer : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainingTime;

    public GameManager gm;
    static public bool gameStart;
    static public bool gameEnd;
    static public bool isSpawned;
    public bool preGameTimer;
    public bool postGameTimer;
    public bool gameInProgress;

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

        //countdown timer
        if (Clock.Value > 0 && IsServer)
            Clock.Value -= Time.fixedDeltaTime;
        //stop timer at 0
        else if (Clock.Value < 1 && IsServer)
        {
            Clock.Value = 0;

            if (preGameTimer == true && gameStart == false)
            {
                gm.GameStartRpc();
                NetworkObject.Destroy(gameObject);
            }
            else if (preGameTimer == false && postGameTimer == false && gameInProgress == true)
            {
                gameInProgress = false;
                gm.GameEndRpc();
                gameEnd = true;
                NetworkObject.Destroy(gameObject);
            }
            else if (postGameTimer == true && gameEnd == true)
            {
                gameEnd = false;
                gm.GameRestartRpc();
                NetworkObject.Destroy(gameObject);
            }

        }

        
    }

    public override void OnNetworkSpawn()
    {
        Clock.OnValueChanged += OnStateChange;

        isSpawned = true;

        
        if (IsServer)
        {

            if (gameInProgress == true)
            {
                Clock.Value = 180;
#if UNITY_EDITOR 
                Clock.Value = 5;
#else
Clock.Value = 180;
#endif
            }
            else
            {
                Clock.Value = 30;
#if UNITY_EDITOR 
                Clock.Value = 15;
#else
Clock.Value = 30;
#endif
            }
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
