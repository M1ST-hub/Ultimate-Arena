using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject resumeButton;

    public void ResumeGame()
    {
            Cursor.lockState = CursorLockMode.Locked;
    }

    public void LeftGame()
    {
        Cursor.lockState = CursorLockMode.None;
    }

}
