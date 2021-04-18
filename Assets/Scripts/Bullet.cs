using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Bullet : MonoBehaviourPunCallbacks
{
    public PhotonView phoView;
    public float destroyTime;
    public string creatorName;

    void Awake()
    {
        if(phoView.IsMine)
        {
            StartCoroutine(BulletTimeout());
        }        
    }

    IEnumerator BulletTimeout()
    {
        yield return new WaitForSeconds(destroyTime);
        GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void DestroyObject()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(!phoView.IsMine)
        {
            return;
        }

        PhotonView target = collision.gameObject.GetComponent<PhotonView>();

        if(collision.gameObject.CompareTag("Player") && (!target.IsMine || target.IsRoomView))
        {
            collision.GetComponent<PlayerController>().healthDecrease(phoView.Owner.NickName);
            StopCoroutine(BulletTimeout());
            
            GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.AllBuffered);
        }

        else
        {
            StopCoroutine(BulletTimeout());

            GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.AllBuffered);
        }
    }
}
