using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using Photon.Pun;

public class SecondCalibrator : MonoBehaviour
{

    [Header("VRIKCalibrator")]
    [Tooltip("Reference to the VRIK component on the avatar.")] public VRIK ik;
    [Tooltip("The settings for VRIK calibration.")] public VRIKCalibrator.Settings settings;
    [Tooltip("The HMD.")] public Transform headTracker = null;
    [Tooltip("(Optional) A tracker on the body of the player on the belt area.")] public Transform bodyTracker = null;
    [Tooltip("(Optional) A hand controller device placed in the player's left hand.")] public Transform leftHandTracker = null;
    [Tooltip("(Optional) A hand controller device placed in the player's right hand.")] public Transform rightHandTracker = null;
    [Tooltip("(Optional) A tracker placed anywhere on the ankle of the player's left leg.")] public Transform leftFootTracker = null;
    [Tooltip("(Optional) A tracker placed anywhere on the ankle of the player's right leg.")] public Transform rightFootTracker = null;

    private bool oncourse = false;
    private bool sincronize = true;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!Ready() && !oncourse)
        {
            oncourse = true;
            LoadReferences();
        }

    }

    public void SetSyncronize(bool x)
    {
        sincronize = x;
    }

    private bool Ready()
    {
        return !(headTracker == null || bodyTracker == null || leftHandTracker == null || rightHandTracker == null || leftFootTracker == null || rightFootTracker == null);
    }

    private void LoadReferences()
    {
        GameObject[] aux;
        if (headTracker == null)
        {
            aux = GameObject.FindGameObjectsWithTag("Head");
            foreach (GameObject target in aux)
            {
                if (!target.GetComponent<PhotonView>().IsMine)
                {
                    headTracker = target.transform;
                    break;
                }
            }
        }
        if (bodyTracker == null)
        {
            aux = GameObject.FindGameObjectsWithTag("Body");
            foreach (GameObject target in aux)
            {
                if (!target.GetComponent<PhotonView>().IsMine)
                {
                    bodyTracker = target.transform;
                    break;
                }
            }
        }
        if (leftHandTracker == null)
        {
            aux = GameObject.FindGameObjectsWithTag("LHand");
            foreach (GameObject target in aux)
            {
                if (!target.GetComponent<PhotonView>().IsMine)
                {
                    leftHandTracker = target.transform;
                    break;
                }
            }
        }
        if (rightHandTracker == null)
        {
            aux = GameObject.FindGameObjectsWithTag("RHand");
            foreach (GameObject target in aux)
            {
                if (!target.GetComponent<PhotonView>().IsMine)
                {
                    rightHandTracker = target.transform;
                    break;
                }
            }
        }
        if (leftFootTracker == null)
        {
            aux = GameObject.FindGameObjectsWithTag("LFoot");
            foreach (GameObject target in aux)
            {
                if (!target.GetComponent<PhotonView>().IsMine)
                {
                    leftFootTracker = target.transform;
                    break;
                }
            }
        }
        if (rightFootTracker == null)
        {
            aux = GameObject.FindGameObjectsWithTag("RFoot");
            foreach (GameObject target in aux)
            {
                if (!target.GetComponent<PhotonView>().IsMine)
                {
                    rightFootTracker = target.transform;
                    break;
                }
            }
        }
        oncourse = false;
    }


    #region PUNRPC

    [PunRPC]
    public void SetHeadReference(int head)
    {
        if (headTracker == null)
        {
            headTracker = PhotonView.Find(head).transform;
        }  
    }

    [PunRPC]
    public void SetLHReference(int lhand)
    {
        if (leftHandTracker == null)
        {
            leftHandTracker = PhotonView.Find(lhand).transform;
        }
    }

    [PunRPC]
    public void SetRHReference(int rhand)
    {
        if (rightHandTracker == null)
        {
            rightHandTracker = PhotonView.Find(rhand).transform;
        }
    }

    [PunRPC]
    public void SetBodyReference(int body)
    {
        if (bodyTracker == null)
        {
            bodyTracker = PhotonView.Find(body).transform;
        }
    }

    [PunRPC]
    public void SetLFReference(int lfoot)
    {
        if (leftFootTracker == null)
        {
            leftFootTracker = PhotonView.Find(lfoot).transform;
        }
    }

    [PunRPC]
    public void SetRFReference(int rfoot)
    {
        if (rightFootTracker == null)
        {
            rightFootTracker = PhotonView.Find(rfoot).transform;
        }
    }

    [PunRPC]
    public void CalibratePlayer(int secondplayer)
    {
        PhotonView pv = PhotonView.Find(secondplayer);
        /*if (PhotonNetwork.IsMasterClient)
        {
            pv.RPC("StopSet", RpcTarget.Others, sincronize);
        }*/
        ik = pv.GetComponent<VRIK>();
        Debug.Log("WHO HAS DISTURBED MY DREAM");

        VRIKCalibrator.Calibrate(ik, settings, headTracker, bodyTracker, leftHandTracker, rightHandTracker, leftFootTracker, rightFootTracker);

        Debug.LogAssertionFormat("RPC INCOMMING");
        Debug.Log("LEFTING RPC");
        Vector3 le = leftHandTracker.Find("Left Hand Target").transform.localPosition;
        le.x *= -1;
        leftHandTracker.Find("Left Hand Target").transform.localPosition = le;
        Debug.Log("LEFTED RPC");

        
    }
    #endregion

}
