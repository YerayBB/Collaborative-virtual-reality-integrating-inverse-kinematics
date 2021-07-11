using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class CenterScene : MonoBehaviour
{

    public GameObject lighthouse1, lighthouse2, peutracker;
    public GameObject modelLight1, modelLight2, modelTrack;

    public uint idTracker, idLighthouse1, idLighthouse2;
    private bool unavez;
    // Use this for initialization
    void Start()
    {
        unavez = false;
        var error = ETrackedPropertyError.TrackedProp_Success;
        uint i = 0;
        while (i < 16)
        {
            var result = new System.Text.StringBuilder((int)64);
            OpenVR.System.GetStringTrackedDeviceProperty(i, ETrackedDeviceProperty.Prop_RenderModelName_String, result, 64, ref error);
            //assigno id nou a cada device
            if (result.ToString().Contains("tracker"))
            {
                idTracker = i;
            }
            if (result.ToString().Contains("basestation"))
            {
                idLighthouse1 = i;
                idLighthouse2 = i + 1;
                ++i;
            }
            //  Debug.Log(result.ToString());

            ++i;
        }

        lighthouse1.GetComponent<SteamVR_TrackedObject>().index = (SteamVR_TrackedObject.EIndex)idLighthouse1;
        lighthouse2.GetComponent<SteamVR_TrackedObject>().index = (SteamVR_TrackedObject.EIndex)idLighthouse2;
        //	Debug.Log ("posicio1: " + lighthouse1.GetComponent<Transform>().position);
        //	Debug.Log ("posicio2: " + lighthouse2.transform.position);
        peutracker.GetComponent<SteamVR_TrackedObject>().index = (SteamVR_TrackedObject.EIndex)idTracker;

        modelLight1.GetComponent<SteamVR_RenderModel>().index = (SteamVR_TrackedObject.EIndex)idLighthouse1;
        modelLight2.GetComponent<SteamVR_RenderModel>().index = (SteamVR_TrackedObject.EIndex)idLighthouse2;
        // modelTrack.GetComponent<SteamVR_RenderModel>().index = (SteamVR_TrackedObject.EIndex)idTracker;

                 

    }

    // Update is called once per frame
    void Update()
    {
        if (!unavez)
        {
            transform.position = (lighthouse1.transform.position + lighthouse2.transform.position) / 2;
            Vector3 aux = new Vector3(transform.position.x, 0, transform.position.z);
            transform.position = aux;
            unavez = true;
        }
    }
}