using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MenuController : MonoBehaviourPunCallbacks
{
    public string VersionName = "0.1";
    public GameObject menuCanvas;
    public GameObject playCanvas;
    public TMP_InputField nameInputField;
    public TMP_InputField joinRoomInputField;
    public TMP_InputField createRoomInputField;
    public GameObject playButton;
    public TMP_Dropdown colorDropdown;
    RoomOptions roomOptions;
    Hashtable customProperties;

    private void Awake()
    {
        roomOptions = new RoomOptions();
        PhotonNetwork.ConnectUsingSettings();
        roomOptions.MaxPlayers = 5;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected");
    }

    public void nameInputFieldInput()
    {
        if(nameInputField.text.Length >= 3 && nameInputField.text.Length <= 8)
        {
            playButton.SetActive(true);
        }

        else
        {
            playButton.SetActive(false);
        }
    }

    public void setName()
    {
        playCanvas.SetActive(false);
        string randomNum = Random.Range(0, 10000).ToString();
        PhotonNetwork.NickName = nameInputField.text + randomNum;
    }

    public void createGame()
    {
        customProperties = new Hashtable
        {
            { "Score", 0 },
            { "Color", colorDropdown.value }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
        PhotonNetwork.CreateRoom(createRoomInputField.text, roomOptions, null);
    }

    public void joinGame()
    {
        customProperties = new Hashtable
        {
            { "Score", 0 },
            { "Color", colorDropdown.value }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
        PhotonNetwork.JoinOrCreateRoom(joinRoomInputField.text, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        
        PhotonNetwork.LoadLevel("GameScene");
    }
}
