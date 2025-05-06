using UnityEngine;
using UnityEngine.UI;

public class CallButton : MonoBehaviour
{
    public GameManager gm;

    private void Start()
    {
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        gm.CyclePodiumCategory();
    }
}
