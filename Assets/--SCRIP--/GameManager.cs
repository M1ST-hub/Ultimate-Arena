using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Entities;
using UnityEditor.PackageManager;
using TMPro;

public class GameManager : NetworkBehaviour
{
    public  List <GameObject> players;
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

    private GameObject timmy;
    private GameObject playTime;
    private GameObject endTimer;

    [Header("Xp")]
    public int gainedExperience;
    public int surviveExperience;

    private PlayerController playerController;
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

        gainedExperience = gainedExperience + 50;

        CalculateExp();
        PlayerStats();

        Debug.Log("GameEnd");
    }

    public void CalculateExp()
    {
        ExperienceManager.Instance.AddExperience(gainedExperience * (1 / 3));
    }

    public void PlayerStats()
    {
        List<PlayerController> playerControllers = new List<PlayerController>();

        // Gather stats for all players
        foreach (GameObject player in players)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerControllers.Add(playerController);
            }
        }

        // Sort players by the most tags
        playerControllers.Sort((a, b) => b.mostTags.CompareTo(a.mostTags)); // Sort descending by mostTags
        DisplaySortedPlayers("Most Tags", playerControllers);

        // Sort players by tagged time
        playerControllers.Sort((a, b) => b.taggedTime.CompareTo(a.taggedTime)); // Sort descending by taggedTime
        DisplaySortedPlayers("Most Tagged Time", playerControllers);

        // Sort players by untagged time
        playerControllers.Sort((a, b) => b.untaggedTime.CompareTo(a.untaggedTime)); // Sort descending by untaggedTime
        DisplaySortedPlayers("Most Untagged Time", playerControllers);
    }

    private void DisplaySortedPlayers(string criteria, List<PlayerController> sortedPlayers)
    {
        Debug.Log($"Sorted by {criteria}:");

        foreach (PlayerController playerController in sortedPlayers)
        {
            pod1.text = $"{playerController.name} - Tags: {playerController.mostTags}";
            pod2.text = $"Tagged Time: {playerController.taggedTime}";
            pod3.text = $"Untagged Time: {playerController.untaggedTime}";
            Debug.Log($"{playerController.name} - Tags: {playerController.mostTags}, Tagged Time: {playerController.taggedTime}, Untagged Time: {playerController.untaggedTime}");
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
        players.Clear();
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
