using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Text;

public class AddPlayerMenu : MonoBehaviour
{
    public TMP_InputField screenNameInput;
    public TMP_InputField firstNameInput;
    public TMP_InputField lastNameInput;
    public TMP_InputField scoreInput;
    public TMP_Text resultText;

    private string apiUrl = "http://localhost:3000/player";

    public void SubmitPlayer()
    {
        string screenName = screenNameInput.text.Trim();
        string firstName = firstNameInput.text.Trim();
        string lastName = lastNameInput.text.Trim();
        string scoreText = scoreInput.text.Trim();

        if (string.IsNullOrEmpty(screenName) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(scoreText))
        {
            resultText.text = "<color=red>All fields are required!</color>";
            return;
        }

        if (!int.TryParse(scoreText, out int score))
        {
            resultText.text = "<color=red>Score must be a number!</color>";
            return;
        }

        PlayerData newPlayer = new PlayerData
        {
            ScreenName = screenName,
            FirstName = firstName,
            LastName = lastName,
            Score = score,
            DateStartedPlaying = DateTime.UtcNow.ToString("yyyy-MM-dd")
        };

        StartCoroutine(SendPlayerData(newPlayer));
    }

    private IEnumerator SendPlayerData(PlayerData player)
    {
        string json = JsonUtility.ToJson(player);
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            resultText.text = "<color=green>Player added successfully!</color>";
        }
        else
        {
            resultText.text = "<color=red>Failed to add player.</color>";
        }
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}


