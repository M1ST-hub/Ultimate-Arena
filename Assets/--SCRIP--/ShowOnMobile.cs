using UnityEngine;

public class ShowOnMobile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        gameObject.SetActive(Application.isMobilePlatform);
#if UNITY_IOS || UNITY_ANDROID
        Cursor.lockState = CursorLockMode.None;
#endif
    }
}
