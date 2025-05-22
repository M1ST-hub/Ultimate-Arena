using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject resumeButton;
    public bool isPaused;

    public void ResumeGame()
    {
#if UNITY_IOS || UNITY_ANDROID
        Cursor.lockState = CursorLockMode.None;
#else
        Cursor.lockState = CursorLockMode.Locked;
#endif

            isPaused = false;
    }

    public void LeftGame()
    {
        Cursor.lockState = CursorLockMode.None;
    }

}
