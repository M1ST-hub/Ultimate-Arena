using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Entities;
using UnityEditor.PackageManager;

public class GameManager : NetworkBehaviour
{
    public  List <GameObject> players;
    public GameObject itArrow;
    public GameObject[] spawnPoints;
    public GameObject[] endPoints;
    public GameObject[] restartPoints;
    public GameObject gameTimer;
    public GameObject preGameTimer;
    public GameObject postGameTimer;
    public GameObject canvas;
    public bool isGameStarted = false;

    private GameObject timmy;
    private GameObject playTime;
    private GameObject endTimer;

    public int gainedExperience;
    public int surviveExperience;

    public static GameManager Instance {  get; private set; }

    private GameObject arrow;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        Timer.gameStart = false;
        Timer.gameEnd = false;
        //NetworkManager.Singleton.OnClientConnectedCallback += PlayerJoined;
    }

    void Update() 
    {
        
    }

    [Rpc(SendTo.Everyone)]
    private void FirstTaggerRpc()
    {
        //players[Random.Range(0, players.Count)].GetComponent<PlayerController>().itArrow.SetActive(true);
        isGameStarted = true;
        if (IsServer)
        {
            GameObject randomPlayer = players[Random.Range(0, players.Count)];
            arrow =  Instantiate(itArrow, randomPlayer.transform);
            
            arrow.GetComponent<NetworkObject>().Spawn();
            arrow.transform.SetParent(randomPlayer.transform);
            
        }
            
    }


    [Rpc(SendTo.Everyone)]
    public void GameStartRpc()
    {
        GetPlayers();

        foreach (GameObject player in players)
        {
            player.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
        }

        Destroy(timmy);

        if (IsServer)
            FirstTaggerRpc();

        SpawnGameTimerRpc();

        Timer.gameStart = true;

        Debug.Log("GameStart");
    }


    [Rpc(SendTo.Everyone)]
    public void GameEndRpc()
    {
        GetPlayers();

        foreach (GameObject player in players)
        {
            player.transform.position = endPoints[Random.Range(0, endPoints.Length)].transform.position;
        }

        Destroy(playTime);

        SpawnPostGameTimerRpc();

        if (arrow != null)
            Destroy(arrow);
        

        Debug.Log("GameEnd");
    }

    public void CalculateExp()
    {
        ExperienceManager.Instance.AddExperience(gainedExperience);
    }

    [Rpc(SendTo.Everyone)]
    public void GameRestartRpc()
    {
        GetPlayers();

        foreach (GameObject player in players)
        {
            player.transform.position = restartPoints[Random.Range(0, restartPoints.Length)].transform.position;
        }

        Destroy(endTimer);

        SpawnTimerRpc();

        if (arrow != null)
            Destroy(arrow);

        Timer.gameStart = false;

        gainedExperience = 0;

        Debug.Log("Game Restarted");
    }

    private void GetPlayers()
    {
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            players.Add(client.PlayerObject.gameObject);
        }
    }

    private void PlayerJoined(ulong clientId)
    {
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.ClientId == clientId)
            {
                players.Add(client.PlayerObject.gameObject);
            }
        }

       // NetworkManager.ConnectedClientsIds;
    }

    [Rpc(SendTo.Everyone)]
    public void SpawnTimerRpc()
    {
        timmy = Instantiate(preGameTimer, canvas.transform);
        timmy.GetComponent<NetworkObject>().Spawn();
        timmy.transform.SetParent(canvas.transform);
    }

    [Rpc(SendTo.Everyone)]
    public void SpawnGameTimerRpc()
    {
        playTime = Instantiate(gameTimer, canvas.transform);
        playTime.GetComponent<NetworkObject>().Spawn();
        playTime.transform.SetParent(canvas.transform);
    }

    [Rpc(SendTo.Everyone)]
    public void SpawnPostGameTimerRpc()
    {
        endTimer = Instantiate(postGameTimer, canvas.transform);
        endTimer.GetComponent<NetworkObject>().Spawn();
        endTimer.transform.SetParent(canvas.transform);
    }



}
