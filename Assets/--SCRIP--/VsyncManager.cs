using Unity.Services.Core;
using UnityEngine;

public class VsyncManager : MonoBehaviour
{
    private static VsyncManager instance;
    public static VsyncManager Instance {  get { return instance; } }

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
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        if (QualitySettings.vSyncCount != 1)
        {
            QualitySettings.vSyncCount = 1;
        }
    }
}
