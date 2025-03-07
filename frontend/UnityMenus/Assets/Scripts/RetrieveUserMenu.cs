using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using System;

[Serializable]
public class PlayerData
{
    public string playerid;
    public string ScreenName;

    public string FirstName;
    public string LastName;
    public string DateStartedPlaying;
    public int Score;
}

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
        UnityWebRequest request = UnityWebRequest.Get(apiUrl + UnityWebRequest.EscapeURL(screenName));
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        Debug.Log($"Fetching User: {screenName}");

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;

            if (jsonResponse.Contains("error"))
            {
                resultText.text = "User not found.";
            }
            else
            {
                FormatUserData(jsonResponse);
            }
        }
        else
        {
            resultText.text = "User not found.";
        }

        Debug.Log($"Server Response: {request.downloadHandler.text}");
    }

    private void FormatUserData(string json)
    {
        PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);

        if (playerData != null)
        {
            string formattedText =
                $"<color=#710E0E>Player ID: </color>{playerData.playerid}\n" +
                $"<color=#710E0E>Screen Name:</color>{playerData.ScreenName}\n" +
                $"<color=#710E0E>Last Name:</color> {playerData.LastName}\n" +
                $"<color=#710E0E>Date Started:</color> {playerData.DateStartedPlaying}\n" +
                $"<color=#710E0E>Score:</color> {playerData.Score}";

            resultText.text = formattedText;
        }
        else
        {
            resultText.text = "Error parsing player data.";
        }
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
