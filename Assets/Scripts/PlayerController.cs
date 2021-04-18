using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public PhotonView photonView;
    public Transform bulletSpawn;
    public GameObject bulletPrefab;
    public MeshRenderer body;
    public GameObject cam;
    public float turnSpeed;
    public float rideSpeed;
    public bool fireCooldown = false;
    public float fireCooldownTime;
    public MeshRenderer[] rend;
    public bool canMove = true;
    public bool end = false;
    public int hp = 5;
    public Transform target;
    public float smoothSpeed = 10f;
    public Vector3 offset;
    public static PlayerController pCon;

    void Awake()
    {
        pCon = this;
        GameController.gCon.syncColors();
        StartCoroutine(hideInfo());
        cam = Camera.main.gameObject;
        if(photonView.IsMine)
        {          
            cam.SetActive(true);           
        }
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            if (canMove == true  && end == false)
            {
                float horizontalSpeed = Input.GetAxis("Horizontal");
                float verticalSpeed = Input.GetAxis("Vertical");
                transform.Rotate(Vector3.up, horizontalSpeed * turnSpeed * Time.deltaTime);
                transform.Translate(Vector3.forward * verticalSpeed * rideSpeed * Time.deltaTime);

                if (Input.GetKeyDown(KeyCode.Mouse0) && fireCooldown == false)
                {
                    Fire();
                    StartCoroutine(fireAtCooldown());
                }

                if (target != null)
                {
                    Vector3 desiredPosition = target.position + offset;
                    Vector3 smoothedPosition = Vector3.Lerp(cam.transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
                    cam.transform.position = smoothedPosition;

                    cam.transform.LookAt(target);
                }
            }
        }
    }

    IEnumerator hideInfo()
    {
        yield return new WaitForSeconds(2);
        GameController.gCon.startInfo.SetActive(false);
    }

    public void endGame(string winner)
    {
        canMove = false;
        end = true;
        StopCoroutine(respawnTimer());
        GameController.gCon.rtGameUI.SetActive(false);
        GameController.gCon.respawnUI.SetActive(false);
        if (PhotonNetwork.LocalPlayer.NickName == winner)
        {
            GameController.gCon.endGameText.text = "Good job you won!";
        }
        else
        {
            GameController.gCon.endGameText.text = "Maybe next time!";
        }
        GameController.gCon.endGameUI.SetActive(true);
    }

    [PunRPC]
    void RPC_endGame(string winner)
    {
        if(photonView.IsMine)
        {
            canMove = false;
            end = true;
            StopCoroutine(respawnTimer());
            GameController.gCon.rtGameUI.SetActive(false);
            GameController.gCon.respawnUI.SetActive(false);
            if(PhotonNetwork.LocalPlayer.NickName == winner)
            {
                GameController.gCon.endGameText.text = "Good job you won!";
            }
            else
            {
                GameController.gCon.endGameText.text = "Maybe next time!";
            }
            GameController.gCon.endGameUI.SetActive(true);
            }
        else
        {
            canMove = false;
            end = true;
            StopCoroutine(respawnTimer());
            GameController.gCon.rtGameUI.SetActive(false);
            GameController.gCon.respawnUI.SetActive(false);
            if (PhotonNetwork.LocalPlayer.NickName == winner)
            {
                GameController.gCon.endGameText.text = "Good job you won!";
            }
            else
            {
                GameController.gCon.endGameText.text = "Maybe next time!";
            }
            GameController.gCon.endGameUI.SetActive(true);
        }
    }

    void Fire()
    {
        if(canMove == true && end == false)
        {
            GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, bulletSpawn.position, bulletSpawn.rotation, 0);
            bullet.GetComponent<Bullet>().creatorName = photonView.Owner.NickName;

            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 10f;
        }
    }

    IEnumerator fireAtCooldown()
    {
        fireCooldown = true;
        GameController.gCon.reloadingUI.SetActive(true);
        yield return new WaitForSeconds(fireCooldownTime);
        fireCooldown = false;
        GameController.gCon.reloadingUI.SetActive(false);
        GameController.gCon.reloadImage.GetComponent<Image>().fillAmount = 1;    
    }

    public void healthDecrease(string ownerName)
    {
        photonView.RPC("RPC_healthDecrease", RpcTarget.AllBuffered, ownerName);
    }

    [PunRPC]
    void RPC_healthDecrease(string ownerName)
    {
        if (!photonView.IsMine)
            return;

        hp--;
        
        if(hp > 0)
        {
            GameController.gCon.healtBar.value = hp;
        }

        else if(hp == 0)
        {  
            GameController.gCon.infoFunc(ownerName + " killed " + PhotonNetwork.NickName);
            StartCoroutine(respawnTimer());
            ScoreboardController.scorCon.UpdateScoreboard(ownerName);          
        }
    }

    IEnumerator respawnTimer()
    {
        if(end == false)
        {
            canMove = false;
            photonView.RPC("RPC_respawnPrep", RpcTarget.AllBuffered, false);
            GameController.gCon.rtGameUI.SetActive(false);
            GameController.gCon.respawnUI.SetActive(true);
            GameController.gCon.reloadingUI.SetActive(false);
            GameController.gCon.timer = 3;
            yield return new WaitForSeconds(3f);
        
            GameController.gCon.respawnUI.SetActive(false);
            GameController.gCon.rtGameUI.SetActive(true);
            photonView.RPC("RPC_respawnPrep", RpcTarget.AllBuffered, true);
            hp = 5;
            GameController.gCon.healtBar.value = hp;

            int ranSpawnPoint = Random.Range(0, GameController.gCon.spawnPoints.Length);
            transform.position = new Vector3(GameController.gCon.spawnPoints[ranSpawnPoint].position.x, GameController.gCon.spawnPoints[ranSpawnPoint].position.y, GameController.gCon.spawnPoints[ranSpawnPoint].position.z);
            canMove = true;
        }
    }

    [PunRPC]
    void RPC_respawnPrep(bool state)
    {
        foreach(MeshRenderer meshRen in rend)
        {
            meshRen.enabled = state;
        }
    }
}
