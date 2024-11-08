using System;
using Unity.Entities;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    public float threshold = 3.5f; // The X position that divides the map into two sides
    public Material materialTeam1;
    public Material materialTeam2;
    void Start()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        Debug.Log(allObjects.Length);
        foreach (GameObject obj in allObjects)
        {
            // Check if the object is on the right or left side of the map (using Z position)
            if (obj.transform.position.z > threshold)
            {
                Material team1Mat = obj.GetComponent<Material>();

                // Check if the Renderer exists to prevent errors
                if (team1Mat != null)
                {
                    // Change the material of the GameObject
                    team1Mat = materialTeam1;
                    Debug.Log("should change color");
                }
                else
                {
                    Debug.Log("Is above threshold but didnt get renderer");
                }
            }
            else
            {
                Material team2Mat = obj.GetComponent<Material>();

                
                if (team2Mat != null)
                {
                    team2Mat = materialTeam2;
                }
                else
                {
                    Debug.Log("Is above threshold but didnt get renderer");
                }
            }
        }
    }
    private void Update()
    {
       
    }
}
