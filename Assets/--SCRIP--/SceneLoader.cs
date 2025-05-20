using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadTag()
    {
        Player.Instance.SavePlayer();
        PlayerPrefs.Save();
        SceneManager.LoadScene("Tag");
    }

    public void LoadMainMenu()
    {
        Player.Instance.SavePlayer();
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadTitle()
    {
        Player.Instance.SavePlayer();
        PlayerPrefs.Save();
        SceneManager.LoadScene("TitleScene");       
    }

    public void QuitGame()
    {
#if UNITY_EDITOR 
        EditorApplication.ExitPlaymode();
#else
Application.Quit();
Player.Instance.SavePlayer();
PlayerPrefs.Save();
#endif
    }
}
