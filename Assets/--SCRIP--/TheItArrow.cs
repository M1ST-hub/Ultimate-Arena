using UnityEngine;
using Unity.Netcode;

public class TheItArrow : NetworkBehaviour


{
    public static TheItArrow Instance { get; private set; }

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
}
