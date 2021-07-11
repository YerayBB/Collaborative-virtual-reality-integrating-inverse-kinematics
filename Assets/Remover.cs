using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Remover : MonoBehaviour
{
    NetManagerV nm;
    private bool justone;
    // Start is called before the first frame update
    void Start()
    {
        nm = GameObject.Find("NetworkManager").GetComponent<NetManagerV>();
        if(gameObject.transform.parent.gameObject.GetComponent<PhotonView>().IsMine){
            transform.GetChild(0).gameObject.SetActive(false);
        }
        justone = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!justone)
        {
            if (nm.neverever)
            {
                if (nm.avatarOn) gameObject.SetActive(false);
                justone = true;
            }
        }
    }
}
