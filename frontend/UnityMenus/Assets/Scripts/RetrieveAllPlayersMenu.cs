using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using System;

[Serializable]
public class PlayerDataList
{
    public List<PlayerData> players;
}



public class RetrieveAllPlayersMenu : MonoBehaviour
{
    public TMP_Text resultText;
    public TMP_Text titleText;
    public RectTransform contentPanel; 
    private string apiUrl = "http://localhost:3000/player/";

    private List<PlayerData> users = new();
    private int selectedIndex = 0;
    private bool isDataLoaded = false;

    void Start()
    {
        titleText.text = "<color=#FFD700>All Players</color>";
        StartCoroutine(FetchAllPlayers());
    }

    private IEnumerator FetchAllPlayers()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;

            if (jsonResponse.Contains("error") || jsonResponse == "[]")
            {
                resultText.text = "No players found.";
            }
            else
            {
                ParseAndDisplayPlayers(jsonResponse);
            }
        }
        else
        {
            resultText.text = "Failed to retrieve players.";
        }
    }

    private void ParseAndDisplayPlayers(string json)
    {
        PlayerDataList dataList = JsonUtility.FromJson<PlayerDataList>("{\"players\":" + json + "}");

        if (dataList.players != null && dataList.players.Count > 0)
        {
            users = dataList.players;
            users.Sort((x, y) => x.ScreenName.CompareTo(y.ScreenName)); // Sort alphabetically
            isDataLoaded = true;
            UpdateUI();
        }
        else
        {
            resultText.text = "No users found.";
        }
    }

    private void UpdateUI()
    {
        string displayText = "";
        for (int i = 0; i < users.Count; i++)
        {
            Color textColor = (i == selectedIndex) ? Color.yellow : Color.white;
            displayText += $"<color={(i == selectedIndex ? "#FFFF00" : "#FFFFFF")}>" +
                           $"{users[i].ScreenName} - {users[i].Score}</color>\n";
        }
        resultText.text = displayText;
    }

    void Update()
    {
        if (!isDataLoaded) return;

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex + 1) % users.Count;
            UpdateUI();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex - 1 + users.Count) % users.Count;
            UpdateUI();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoBack();
        }
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
