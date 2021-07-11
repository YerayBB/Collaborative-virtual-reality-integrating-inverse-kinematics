using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LeftHandVRDummy : MonoBehaviour
{   
    public GameObject target = null;


    void Start()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            Transform aux = GameObject.FindGameObjectsWithTag("MainCamera")[0].transform.parent;
            target = aux.Find("ViveControllers/Left/RenderModel/DeviceTracker").gameObject;
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
