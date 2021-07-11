using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PloyerControler : MonoBehaviourPunCallbacks, IPunObservable
{
    //public float moveDistance = 1;
    public bool isMoving = false;
    public bool holding = false;
    
    //public GameObject playermodel = null;
    //private Renderer renderer = null;

    // Start is called before the first frame update
    void Start()
    {
       // renderer = playermodel.GetComponent<Renderer>();
       // playermodel.transform.localEulerAngles.Set(playermodel.transform.localEulerAngles.x, playermodel.transform.localEulerAngles.y+1, playermodel.transform.localEulerAngles.z);

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Time.deltaTime);
        ///pre vr
        /// CanIdle();
        /// CanMove();
        
    }

    [PunRPC]
    public void HoldingSet(bool x)
    {
        if (photonView.IsMine)
        {
            holding = x;
        }
    }

    [PunRPC]
    public void StopSet(bool x)
    {
        if (photonView.IsMine)
        {
            GetComponent<Animator>().SetBool("Stop", x);
        }
    }

    public void HoldCall(bool x)
    {
        photonView.RPC("HoldingSet", RpcTarget.All, x);
    }

    /*void CanIdle()
    {
        if (Input.GetKey(KeyCode.D))
        {
            playermodel.transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 1.0f, transform.eulerAngles.z);
        }
        if (Input.GetKey(KeyCode.A))
        {
            playermodel.transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y - 1, transform.eulerAngles.z);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            playermodel.transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 180, transform.eulerAngles.z);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            isMoving = true;
            /* //because of the animation
            playermodel.GetComponent<StepCollider>().stepon();*//*
        }

    }*/

    /*void CanMove()
    {
        if (isMoving)
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += transform.forward * Time.deltaTime*moveDistance;
            }
            if (Input.GetKeyUp(KeyCode.W))
            {
                isMoving = false;
            }
        }
    }*/

    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(holding);
        }
        else
        {
            this.holding = (bool)stream.ReceiveNext();
        }
    }

    #endregion

}
