using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public  List <GameObject> players;
    public GameObject[] spawnPoints;
    public GameObject gameTimer;

    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += PlayerJoined;

    }

    void Update() 
    {
        
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
    private void FirstTaggerRpc()
    {
        players[Random.Range(0, players.Count)].GetComponent<PlayerController>().itArrow.SetActive(true);
    }

    public void GameStart()
    {
        foreach (GameObject player in players)
        {
            player.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
        }

        gameTimer.SetActive(true);

        FirstTaggerRpc();
    }
}
