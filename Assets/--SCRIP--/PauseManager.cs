using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject resumeButton;
    public bool isPaused;

    public void ResumeGame()
    {
            Cursor.lockState = CursorLockMode.Locked;
            isPaused = false;
    }

    public void LeftGame()
    {
        Cursor.lockState = CursorLockMode.None;
    }

}
