using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HeadVRDummy : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject target = null;


    void Start()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            target = GameObject.FindGameObjectsWithTag("MainCamera")[0];
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            if (target != null)
            {
                transform.position = target.transform.position;
                transform.rotation = target.transform.rotation;
            }
        }
    }
}
