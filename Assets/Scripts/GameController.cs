using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class GameController : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public TMP_Text ping;
    public TMP_Text playerName;
    public Slider healtBar;
    public GameObject disconnectUI;
    bool off = false;
    public GameObject feedGrid;
    public GameObject infoCell;
    public GameObject rtGameUI;
    public GameObject respawnUI;
    public GameObject scoreboardUI;
    public GameObject scoreboardCell;
    public TMP_Text respawnTimerText;
    public static GameController gCon;
    public Transform[] spawnPoints;
    public float timer;
    public GameObject[] listOfPlayers;
    public GameObject startInfo;
    public GameObject endGameUI;
    public TMP_Text endGameText;
    public TMP_Text roomName;
    public TMP_Text fps;
    public Transform reloadImage;
    public GameObject reloadingUI;

    void Awake()
    {
        gCon = this;

        int ranSpawnPoint = Random.Range(0, spawnPoints.Length);

        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(spawnPoints[ranSpawnPoint].position.x, spawnPoints[ranSpawnPoint].position.y, spawnPoints[ranSpawnPoint].position.z), transform.rotation, 0);
        playerName.text = PhotonNetwork.NickName;
        roomName.text = "Room name: " + PhotonNetwork.CurrentRoom.Name;
    }

    private void Update()
    {
        reloadImage.position = Input.mousePosition;
        ping.text = "Ping: " + PhotonNetwork.GetPing();
        fps.text = "FPS: " + (int)Mathf.Round(1 / Time.deltaTime);
        
        if(reloadingUI.activeSelf)
        {
            reloadImage.GetComponent<Image>().fillAmount -= 0.5f * Time.deltaTime;
        }

        if (respawnUI.activeSelf)
        {
            timer -= Time.deltaTime;
            int tempTimer = (int)Mathf.Round(timer);
            respawnTimerText.text = "Respawning in: " + tempTimer;
        }

        if(Input.GetKey(KeyCode.Tab))
        {
            scoreboardUI.SetActive(true);
        }
        else
        {
            scoreboardUI.SetActive(false);
        }

        if(off && Input.GetKeyDown(KeyCode.Escape) && !PlayerController.pCon.end)
        {
            disconnectUI.SetActive(false);
            off = false;
        }
        else if(!off && Input.GetKeyDown(KeyCode.Escape) && !PlayerController.pCon.end)
        {
            disconnectUI.SetActive(true);
            off = true;
        }
    }

    public void disconnectRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("MenuScene");
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        GameObject obj = Instantiate(infoCell, new Vector2(0, 0), Quaternion.identity);
        obj.transform.SetParent(feedGrid.transform.transform, false);
        obj.GetComponent<TMP_Text>().text = player.NickName + " joined the game";
        syncColors();
    }

    public override  void OnPlayerLeftRoom(Player player)
    {
        GameObject obj = Instantiate(infoCell, new Vector2(0, 0), Quaternion.identity);
        obj.transform.SetParent(feedGrid.transform.transform, false);
        obj.GetComponent<TMP_Text>().text = player.NickName + " left the game";
    }

    public void infoFunc(string text)
    {
        GetComponent<PhotonView>().RPC("RPC_infoFunc", RpcTarget.AllBuffered, text);
    }

    [PunRPC]
    void RPC_infoFunc(string text)
    {
        GameObject obj = Instantiate(infoCell, new Vector2(0, 0), Quaternion.identity);
        obj.transform.SetParent(feedGrid.transform.transform, false);
        obj.GetComponent<TMP_Text>().text = text;
    }

    public void syncColors()
    {
        GetComponent<PhotonView>().RPC("RPC_syncColors", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void RPC_syncColors()
    {
        Player[] players = PhotonNetwork.PlayerList;
        listOfPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject obj in listOfPlayers)
        {
            foreach (Player p in players)
            {
                if (obj.GetComponent<PhotonView>().Owner.NickName == p.NickName)
                {
                    int color = int.Parse(p.CustomProperties["Color"].ToString());
                    MeshRenderer rend = obj.GetComponent<PlayerController>().body;
                    switch (color)
                    {
                        case 0:
                            rend.material.color = Color.red;
                            break;

                        case 1:
                            rend.material.color = Color.blue;
                            break;

                        case 2:
                            rend.material.color = Color.green;
                            break;

                        case 3:
                            rend.material.color = Color.yellow;
                            break;

                        case 4:
                            rend.material.color = Color.magenta;
                            break;

                        case 5:
                            rend.material.color = Color.grey;
                            break;

                        case 6:
                            rend.material.color = Color.cyan;
                            break;

                        case 7:
                            rend.material.color = Color.black;
                            break;
                    }
                }
            }
        }
    }
}
