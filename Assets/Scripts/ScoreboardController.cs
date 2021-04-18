using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;

public class ScoreboardController : MonoBehaviourPunCallbacks
{
    public static ScoreboardController scorCon;
    public string[,] scoreBoard = new string[2, 5];
    public Player[] players;
    public string[] names;
    public GameObject leaderboardCellPrefab;
    public GameObject leaderboardParent;
    public List<GameObject> listOfCells;
    public List<LeaderboardCellScript> leaderboardCells;
    public PhotonView phView;
    Hashtable customProperties;

    private void Awake()
    {
        scorCon = this;
        players = PhotonNetwork.PlayerList;
        int a = 0;
        foreach (Player p in players)
        {
            GameObject obj = Instantiate(leaderboardCellPrefab, new Vector2(0, 0), Quaternion.identity);
            obj.name = p.NickName;
            obj.transform.SetParent(leaderboardParent.transform.transform, false);
            listOfCells.Add(obj);
            leaderboardCells.Add(obj.GetComponent<LeaderboardCellScript>());
            leaderboardCells[a].cellName.text = p.NickName;
            leaderboardCells[a].cellScore.text = p.CustomProperties["Score"].ToString();
            Debug.Log(p.NickName + " has score " + p.CustomProperties["Score"].ToString());
            a++;
        }
    }

    public void UpdateScoreboard(string playerName)
    {
        phView.RPC("RPC_UpdateScoreboard", RpcTarget.All, playerName);
    }

    [PunRPC]
    void RPC_UpdateScoreboard(string playerName)
    {
        int indexObj = listOfCells.FindIndex(a => a.gameObject.name == playerName);
        int score = int.Parse(leaderboardCells[indexObj].cellScore.text);
        score++;

        if (score == 10)
        {
            foreach (GameObject g in GameController.gCon.listOfPlayers)
            {
                g.GetComponent<PlayerController>().endGame(playerName);
            }

        }
        foreach (Player p in players)
        {
            if (p.NickName == playerName)
            {
                int color = int.Parse(p.CustomProperties["Color"].ToString());
                customProperties = new Hashtable
                {
                    { "Score", score },
                    { "Color", color }
                };
                p.SetCustomProperties(customProperties);
            }
        }
        leaderboardCells[indexObj].cellScore.text = score.ToString();

        SortScoreboard();
    }

    public void SortScoreboard()
    {
        Transform parent = GameController.gCon.scoreboardUI.GetComponent<Transform>();
        Transform[] ledCells = new Transform[parent.childCount];
        int[] playerScores = new int[parent.childCount];
        string[] childNames = new string[parent.childCount];

        int c = 0;
        foreach (Transform child in parent)
        {
            ledCells[c] = child;
            c++;
        }

        c = 0;
        foreach (Transform child in parent)
        {
            playerScores[c] = int.Parse(child.GetComponent<LeaderboardCellScript>().cellScore.text);
            c++;
        }

        c = 0;

        foreach (string g in childNames)
        {
            childNames[c] = ledCells[c].name;
            c++;
        }

        Array.Sort(playerScores, childNames);
        Array.Reverse(childNames);

        c = 0;

        foreach (string u in childNames)
        {
            foreach (Transform k in ledCells)
            {
                if (u == k.name)
                {
                    k.SetSiblingIndex(c);
                }
            }
            c++;
        }
    }

    public void DeleteCell(Player playerName)
    {
        int indexObjToDele = listOfCells.FindIndex(a => a.gameObject.name == playerName.NickName);
        GameObject objToDelete = listOfCells[indexObjToDele];
        listOfCells.RemoveAt(indexObjToDele);
        leaderboardCells.RemoveAt(indexObjToDele);
        Destroy(objToDelete);
    }

    public void CreateCell(Player playerName)
    {
        GameObject obj = Instantiate(leaderboardCellPrefab, new Vector2(0, 0), Quaternion.identity);
        obj.name = playerName.NickName;
        obj.transform.SetParent(leaderboardParent.transform.transform, false);
        listOfCells.Add(obj);
        int indexObj = listOfCells.FindIndex(a => a.gameObject.name == obj.name);
        leaderboardCells.Add(obj.GetComponent<LeaderboardCellScript>());
        leaderboardCells[indexObj].cellName.text = playerName.NickName;
        leaderboardCells[indexObj].cellScore.text = playerName.CustomProperties["Score"].ToString();
    }

    public override void OnPlayerLeftRoom(Player player)
    {
        DeleteCell(player);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        CreateCell(newPlayer);
        SortScoreboard();
    }
}
