using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OpenRetrieveUserMenu()
    {
        SceneManager.LoadScene("RetrieveUserScene");
    }

    public void OpenRetrieveAllUsersMenu()
    {
        SceneManager.LoadScene("RetrieveAllUsersScene");
    }

    public void OpenAddPlayerMenu()
    {
        SceneManager.LoadScene("AddPlayerScene");
    }

    public void OpenEditDeleteUserMenu()
    {
        SceneManager.LoadScene("EditDeleteUserScene");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}