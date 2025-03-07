using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class RetrieveUserMenu : MonoBehaviour
{
    public TMP_InputField screenNameInput;
    public TMP_Text resultText;
    private string apiUrl = "http://localhost:3000/player/";

    public void GetUser()
    {
        string screenName = screenNameInput.text.Trim();
        if (string.IsNullOrEmpty(screenName))
        {
            resultText.text = "Please enter a screen name.";
            return;
        }

        StartCoroutine(FetchUser(screenName));
    }

    private IEnumerator FetchUser(string screenName)
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl + screenName);
        yield return request.SendWebRequest();
        Debug.Log($"Fetching User: {screenName}");
        if (request.result == UnityWebRequest.Result.Success)
        {
            resultText.text = request.downloadHandler.text; 
        }
        else
        {
            resultText.text = "User not found.";
        }
        Debug.Log($"Server Response: {request.downloadHandler.text}");
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
