using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public List<GameObject> players;
    public GameObject itArrow;
    public GameObject[] spawnPoints;
    public GameObject[] endPoints;
    public GameObject[] restartPoints;

    [Header("Timers")]
    public GameObject gameTimer;
    public GameObject preGameTimer;
    public GameObject postGameTimer;
    public GameObject canvas;
    public bool isGameStarted = false;

    [Header("Podium")]
    public TextMeshProUGUI pod1;
    public TextMeshProUGUI pod2;
    public TextMeshProUGUI pod3;
    public string mostTaggedPlayer = "";
    public int mostTags = 0;
    public string longestSurvivor = "";
    public float surviveTime = 0;
    public string taggedTime = "";
    public float mostTagTime = 0;

    private GameObject timmy;
    private GameObject playTime;
    private GameObject endTimer;

    [Header("Xp")]
    public int gainedExperience;
    public int surviveExperience;

    private PlayerController playerController;
    public static GameManager Instance { get; private set; }

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

        NetworkManager.Singleton.OnClientDisconnectCallback += HandleHostDisconnect;
    }

    //private void OnDestroy()
    //{
    //    // Unregister callback when GameManager is destroyed
    //    NetworkManager.Singleton.OnClientDisconnectCallback -= HandleHostDisconnect;
    //}

    private void HandleHostDisconnect(ulong clientId)
    {
        // If the host disconnects, we need to transfer ownership
        if (NetworkManager.Singleton.IsHost && clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Host disconnected. Transferring host ownership.");
            TransferHostOwnership();
        }
    }

    private void TransferHostOwnership()
    {
        // Get the list of connected clients
        List<NetworkClient> clients = new List<NetworkClient>(NetworkManager.Singleton.ConnectedClientsList);

        // Select a new host (e.g., first available client)
        NetworkClient newHost = null;
        foreach (NetworkClient client in clients)
        {
            if (client.ClientId != NetworkManager.Singleton.LocalClientId)  // Exclude current host
            {
                newHost = client;
                break;
            }
        }

        if (newHost == null)
        {
            Debug.LogError("No other clients available to take over the host.");
            return;
        }

        // Set new host as the server (give authority to the new host)
        NetworkManager.Singleton.StartHost();
        Debug.Log($"New host: Client {newHost.ClientId}");

        // Optionally notify all clients that the host has changed
        NotifyHostChangeRpc(newHost.ClientId);
    }

    [Rpc(SendTo.Everyone)]
    private void NotifyHostChangeRpc(ulong newHostClientId)
    {
        Debug.Log($"Host has changed to Client {newHostClientId}");

        // Additional logic to sync game state or UI on clients, if necessary
    }

    void Start()
    {
        Timer.gameStart = false;
        Timer.gameEnd = false;
        //NetworkManager.Singleton.OnClientDisconnectCallback += TransferHost;
    }

    void Update()
    {

    }



    //public void TransferHost(ulong clientId)
    //{
    //    if (clientId == IsHost)
    //    {

    //    }
    //}

    [Rpc(SendTo.Everyone)]
    private void FirstTaggerRpc()
    {
        //players[Random.Range(0, players.Count)].GetComponent<PlayerController>().itArrow.SetActive(true);
        isGameStarted = true;
        if (IsServer)
        {
            GameObject randomPlayer = players[Random.Range(0, players.Count)];
            arrow = Instantiate(itArrow, randomPlayer.transform);

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

        //Destroy(timmy);

        if (IsServer)
            FirstTaggerRpc();

        if (IsServer)
            SpawnGameTimerRpc();

        Timer.gameStart = true;

        Debug.Log("GameStart");
    }

    private IEnumerator DelayedPlayerStats()
    {
        yield return new WaitForSeconds(0.5f); // Give a moment for networked values to sync
        PlayerStats();
    }

    [Rpc(SendTo.Everyone)]
    public void GameEndRpc()
    {
        StartCoroutine(DelayedPlayerStats());

        if (players.Count == 0)
        {
            Debug.LogError("No players found in the players list.");
            return; // Prevent further processing if no players exist.
        }

        foreach (var player in players)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController == null)
            {
                Debug.LogWarning($"PlayerController not found for {player.name}");
                continue; // Skip this player if PlayerController is not found
            }

            SaveXpFromPlayer(playerController);
        }

        GetPlayers();

        foreach (GameObject player in players)
        {
            player.transform.position = endPoints[Random.Range(0, endPoints.Length)].transform.position;
        }

        if (IsServer)
            SpawnPostGameTimerRpc();

        if (arrow != null)
            Destroy(arrow);

        gainedExperience = gainedExperience + 50;

        Player.Instance.SavePlayer();
        PlayerStats();

        Debug.Log("GameEnd");
    }

    public void SaveXpFromPlayer(PlayerController pc)
    {
        // Ensure that only the local player (the one who owns the PlayerController) is saving their XP
        if (pc.IsOwner)
        {
            // Add the player's current XP to the global Player instance
            Player.Instance.xp += pc.currentXp.Value;

            // Save the player's XP using the SavePlayer method (make sure this is set up to persist the data)
            Player.Instance.SavePlayer();
        }
    }


    public void PlayerStats()
    {
        List<PlayerController> playerControllers = new List<PlayerController>();

        // Gather stats for all players
        foreach (GameObject player in players)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc == null) continue;
            
            playerControllers.Add(pc);
            
            float tagged = pc.netTaggedTime.Value;
            float untagged = pc.netUntaggedTime.Value;

            Debug.Log($"{pc.DisplayName} — Tagged: {tagged:F1}s, Untagged: {untagged:F1}s");
        }

        if (playerControllers.Count == 0)
        {
            Debug.LogError("No valid PlayerControllers found.");
            return;
        }

        // Now sort and display
        playerControllers.Sort((a, b) => b.mostTags.CompareTo(a.mostTags));
        DisplaySortedPlayers("Most Tags", playerControllers);

        playerControllers.Sort((a, b) => b.taggedTime.CompareTo(a.taggedTime));
        DisplaySortedPlayers("Tagged Time", playerControllers);

        playerControllers.Sort((a, b) => b.untaggedTime.CompareTo(a.untaggedTime));
        DisplaySortedPlayers("Untagged Time", playerControllers);
    }

    private void DisplaySortedPlayers(string criteria, List<PlayerController> sortedPlayers)
    {
        Debug.Log($"Sorted by {criteria}:");

        if (criteria == "Most Tags")
        {
            foreach (PlayerController playerController in sortedPlayers)
            {
                if (playerController.mostTags >= mostTags)
                {
                    mostTaggedPlayer = playerController.DisplayName; // use a proper display name
                    mostTags = playerController.mostTags;
                }
            }

            pod2.text = $"{mostTaggedPlayer} - Tags: {mostTags}";
        }

        if (criteria == "Most Untagged Time")
        {
            foreach (PlayerController playerController in sortedPlayers)
            {
                if (playerController.untaggedTime >= surviveTime)
                {
                    longestSurvivor = playerController.DisplayName;
                    surviveTime = playerController.untaggedTime;
                }
            }

            pod1.text = $"{longestSurvivor} - Time Untagged: {surviveTime:F2}s";
        }

        if (criteria == "Most Tagged Time")
        {
            foreach (PlayerController playerController in sortedPlayers)
            {
                if (playerController.taggedTime >= mostTagTime)
                {
                    taggedTime = playerController.DisplayName;
                    mostTagTime = playerController.taggedTime;
                }
            }

            pod3.text = $"{taggedTime} - Time Tagged: {mostTagTime:F2}s";
        }
    }

    [Rpc(SendTo.Everyone)]
    public void GameRestartRpc()
    {
        GetPlayers();

        foreach (GameObject player in players)
        {
            player.transform.position = restartPoints[Random.Range(0, restartPoints.Length)].transform.position;
        }

        //Destroy(endTimer);
        if (IsServer)
            SpawnTimerRpc();

        if (arrow != null)
            Destroy(arrow);

        Timer.gameStart = false;

        gainedExperience = 0;

        mostTaggedPlayer = "";
        mostTags = 0;
        longestSurvivor = "";
        surviveTime = 0;
        taggedTime = "";
        mostTagTime = 0;

        Debug.Log("Game Restarted");
    }

    private void GetPlayers()
    {
        players.Clear();

        if (NetworkManager.Singleton == null || NetworkManager.Singleton.ConnectedClientsList == null)
        {
            Debug.LogError("NetworkManager or ConnectedClientsList is null.");
            return;
        }

        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject != null)
            {
                players.Add(client.PlayerObject.gameObject);

                // Check if PlayerController is attached to the player object
                PlayerController playerController = client.PlayerObject.GetComponent<PlayerController>();
                if (playerController == null)
                {
                    Debug.LogWarning($"PlayerController not found for client {client.ClientId} (Player: {client.PlayerObject.name})");
                }
                else
                {
                    Debug.Log($"PlayerController found for client {client.ClientId} (Player: {client.PlayerObject.name})");
                }
            }
            else
            {
                Debug.LogWarning($"PlayerObject not found for client {client.ClientId}");
            }
        }

        if (players.Count == 0)
        {
            Debug.LogError("No players found in ConnectedClientsList.");
        }

        /*foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            players.Add(client.PlayerObject.gameObject);
        }*/
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

        if (IsServer)
        {
            timmy = Instantiate(preGameTimer, canvas.transform);
            timmy.GetComponent<NetworkObject>().Spawn();
            timmy.transform.SetParent(canvas.transform);
        }

    }

    [Rpc(SendTo.Everyone)]
    public void SpawnGameTimerRpc()
    {
        if (IsServer)
        {
            playTime = Instantiate(gameTimer, canvas.transform);
            playTime.GetComponent<NetworkObject>().Spawn();
            playTime.transform.SetParent(canvas.transform);
        }

    }

    [Rpc(SendTo.Everyone)]
    public void SpawnPostGameTimerRpc()
    {
        if (IsServer)
        {
            endTimer = Instantiate(postGameTimer, canvas.transform);
            endTimer.GetComponent<NetworkObject>().Spawn();
            endTimer.transform.SetParent(canvas.transform);
        }
    }



}
