using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public  List <GameObject> players;
    public GameObject[] spawnPoints;
    public GameObject gameTimer;
    public GameObject preGameTimer;
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
        players[Random.Range(0, players.Count)].GetComponent<PlayerController>().itArrow.SetActive(true);
    }

    [Rpc(SendTo.Everyone)]
    public void GameStartRpc()
    {
        foreach (GameObject player in players)
        {
            player.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
        }

        gameTimer.SetActive(true);
        preGameTimer.SetActive(false);

        FirstTaggerRpc();
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

    
}
