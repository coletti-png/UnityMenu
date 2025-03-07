using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Text;


public class EditDeleteUserMenu : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_InputField searchInput;
    public TMP_InputField screenNameInput;
    public TMP_InputField scoreInput;
    public TMP_Text resultText;

    private List<PlayerData> users = new();
    private int selectedIndex = -1;
    private string apiUrl = "http://localhost:3000/player/";
    private bool isDataLoaded = false;

    void Start()
    {
        StartCoroutine(FetchAllUsers());
    }

    private IEnumerator FetchAllUsers()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;

            if (jsonResponse.Contains("error") || jsonResponse == "[]")
            {
                resultText.text = "No users found.";
                users.Clear();
            }
            else
            {
                ParseAndDisplayUsers(jsonResponse);
            }
        }
        else
        {
            resultText.text = "Failed to retrieve users.";
        }
    }

    private void ParseAndDisplayUsers(string json)
    {
        PlayerDataList dataList = JsonUtility.FromJson<PlayerDataList>("{\"players\":" + json + "}");

        if (dataList.players != null && dataList.players.Count > 0)
        {
            users = dataList.players;
            users.Sort((x, y) => x.ScreenName.CompareTo(y.ScreenName));
            isDataLoaded = true;
            selectedIndex = 0;
            DisplaySelectedUser();
        }
        else
        {
            resultText.text = "No users found.";
        }
    }

    private void DisplaySelectedUser()
    {
        if (users.Count == 0 || selectedIndex < 0) return;

        PlayerData user = users[selectedIndex];
        screenNameInput.text = user.ScreenName;
        scoreInput.text = user.Score.ToString();
        titleText.text = $"Editing: <color=#FFD700>{user.ScreenName}</color>";
    }

    public void ScrollUsers(int direction)
    {
        if (!isDataLoaded || users.Count == 0) return;

        selectedIndex = Mathf.Clamp(selectedIndex + direction, 0, users.Count - 1);
        DisplaySelectedUser();
    }

    public void SearchUser()
    {
        string searchQuery = searchInput.text.Trim().ToLower();
        if (string.IsNullOrEmpty(searchQuery)) return;

        int foundIndex = users.FindIndex(user => user.ScreenName.ToLower().Contains(searchQuery));
        if (foundIndex >= 0)
        {
            selectedIndex = foundIndex;
            DisplaySelectedUser();
        }
        else
        {
            resultText.text = "User not found.";
        }
    }

    public void EditUser()
    {
        if (!isDataLoaded || users.Count == 0 || selectedIndex < 0) return;

        string newScreenName = screenNameInput.text.Trim();
        if (string.IsNullOrEmpty(newScreenName))
        {
            resultText.text = "Screen Name cannot be empty!";
            return;
        }

        if (!int.TryParse(scoreInput.text, out int newScore))
        {
            resultText.text = "Score must be a valid number!";
            return;
        }

        PlayerData selectedUser = users[selectedIndex];

        // Check if screen name is already taken
        foreach (var user in users)
        {
            if (user.ScreenName == newScreenName && user.playerid != selectedUser.playerid)
            {
                resultText.text = "Screen Name is already taken!";
                return;
            }
        }

        selectedUser.ScreenName = newScreenName;
        selectedUser.Score = newScore;

        StartCoroutine(UpdateUser(selectedUser));
    }

    private IEnumerator UpdateUser(PlayerData user)
    {
        string json = JsonUtility.ToJson(user);
        UnityWebRequest request = new UnityWebRequest(apiUrl + user.playerid, "PUT");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            resultText.text = "User updated successfully!";
            StartCoroutine(FetchAllUsers());  // Refresh list
        }
        else
        {
            resultText.text = "Failed to update user.";
        }
    }

    public void DeleteUser()
    {
        if (!isDataLoaded || users.Count == 0 || selectedIndex < 0) return;

        PlayerData selectedUser = users[selectedIndex];
        StartCoroutine(DeleteUserFromDB(selectedUser.playerid));
    }

    private IEnumerator DeleteUserFromDB(string playerid)
    {
        UnityWebRequest request = UnityWebRequest.Delete(apiUrl + playerid);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            resultText.text = "User deleted successfully.";
            StartCoroutine(FetchAllUsers());  // Refresh list
        }
        else
        {
            resultText.text = "Failed to delete user.";
        }
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
