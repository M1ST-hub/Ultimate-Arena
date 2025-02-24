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
    public GameObject gameTimer;
    public GameObject preGameTimer;
    public GameObject canvas;
    public bool isGameStarted = false;

    private GameObject timmy;
    private GameObject playTime;
    public static GameManager Instance {  get; private set; }

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
            var arrow =  Instantiate(itArrow, randomPlayer.transform);
            
            arrow.GetComponent<NetworkObject>().Spawn();
            arrow.transform.SetParent(randomPlayer.transform);
            
        }
            
    }

    [Rpc(SendTo.Everyone)]
    public void DestroyTimmyRpc()
    {
        Destroy(playTime);
        Debug.Log("tim ded");
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

        DestroyTimmyRpc();

        if (IsServer)
            FirstTaggerRpc();

        SpawnGameTimerRpc();

        gameTimer.GetComponent<Timer>().UpdateGame();

        Debug.Log("GameStart");
    }


    [Rpc(SendTo.Everyone)]
    public void GameEndRpc()
    {
        GetPlayers();

        foreach (GameObject player in players)
        {
            player.transform.position = endPoints[Random.Range(0, spawnPoints.Length)].transform.position;
        }

        //Destroy(playTime);

        Destroy(itArrow);

        Debug.Log("GameEnd");
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


}
