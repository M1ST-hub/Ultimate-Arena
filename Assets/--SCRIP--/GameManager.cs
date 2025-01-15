using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Entities;

public class GameManager : NetworkBehaviour
{
    public  List <GameObject> players;
    public GameObject itArrow;
    public GameObject[] spawnPoints;
    public GameObject gameTimer;
    public GameObject preGameTimer;
    public GameObject canvas;
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
        
        NetworkManager.Singleton.OnClientConnectedCallback += PlayerJoined;
    }

    void Update() 
    {
        
    }

    [Rpc(SendTo.Everyone)]
    private void FirstTaggerRpc()
    {
        //players[Random.Range(0, players.Count)].GetComponent<PlayerController>().itArrow.SetActive(true);

        if (IsServer)
        {
            GameObject randomPlayer = players[Random.Range(0, players.Count)];
            var arrow =  Instantiate(itArrow, randomPlayer.transform);
            
            arrow.GetComponent<NetworkObject>().Spawn();
            arrow.transform.SetParent(randomPlayer.transform);
        }
            
    }

    [Rpc(SendTo.Everyone)]
    public void GameStartRpc()
    {

        foreach (GameObject player in players)
        {
            player.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
        }

        //gameTimer.SetActive(true);
        //preGameTimer.SetActive(false);

        if (IsServer)
            FirstTaggerRpc();

        Debug.Log("GameStart");
    }

    [Rpc(SendTo.Everyone)]
    public void GameEndRpc()
    {
        Destroy(itArrow);
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
       var timmy = Instantiate(preGameTimer, canvas.transform);
        timmy.GetComponent<NetworkObject>().Spawn();
        timmy.transform.SetParent(canvas.transform);
        
        
    }

    
}
