using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TurretRotation : MonoBehaviourPunCallbacks
{
    public float rotationSpeed;
    void Update()
    {
        if (photonView.IsMine)
        {
            Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(-angle + 90, Vector3.up), Time.deltaTime * rotationSpeed);
        }
    }
}
