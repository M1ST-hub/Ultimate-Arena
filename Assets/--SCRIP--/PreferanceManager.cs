using UnityEngine;

public class PreferanceManager : MonoBehaviour
{

    public float fov;


    private static PreferanceManager instance;
    public static PreferanceManager Instance { get { return instance; } }

    private void Awake()
    {
        if (instance != null && this != instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(instance.gameObject);
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFOVValue(float value)
    {
        PlayerPrefs.SetFloat("FOV", value);
    }
}
