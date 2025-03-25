using UnityEngine;

public class CosmeticStuff : MonoBehaviour
{
    
    private static CosmeticStuff instance;
    public static CosmeticStuff Instance { get { return instance; } }

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

    public enum CosmeticBanner
    {

    }

    public Sprite[] banners;
    public Sprite[] icons;



}
