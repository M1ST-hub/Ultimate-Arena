using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] players;

    void Start()
    {
        foreach (var player in players)
        {
            object obj = player.gameObject;
        }
    }

    void Update()
    {
        
    }

    private void FirstTagger()
    {
        
    }
}
