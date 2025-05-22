using UnityEngine;
using Unity.Netcode;

public class ShowOnMobile : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(!IsOwner)
        {
            gameObject.SetActive(false);
            return;
        }


        gameObject.SetActive(Application.isMobilePlatform);
#if UNITY_IOS || UNITY_ANDROID
        Cursor.lockState = CursorLockMode.None;
#endif
    }
}
