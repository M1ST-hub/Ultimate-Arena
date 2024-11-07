using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadTag()
    {
        SceneManager.LoadScene("Tag");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
