using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;
using UnityEngine;

public class ControllerManagerV : MonoBehaviour
{

    [Header("Right controller")]
    public GameObject rightRenderModel;
    public GameObject rightGrabber;
    public GameObject rightLaserPointer;

    [Header("Left controller")]
    public GameObject leftRenderModel;
    public GameObject leftGrabber;
    public GameObject leftLaserPointer;

    private HashSet<GameObject> rightGrabbingSet = new HashSet<GameObject>();
    private HashSet<GameObject> leftGrabbingSet = new HashSet<GameObject>();

    public GameObject avatarvr;

    // Start is called before the first frame update
    void Start()
    {
        SwitchMode(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SwitchMode(bool grab)
    {
        rightLaserPointer.SetActive(!grab);
        leftLaserPointer.SetActive(!grab);

        rightGrabber.SetActive(grab);
        rightGrabber.transform.GetChild(0).GetChild(0).tag = tag;
        leftGrabber.SetActive(grab);
        leftGrabber.transform.GetChild(0).GetChild(0).tag = tag;
    }

    public void OnGrabbed(BasicGrabbable grabbedObj)
    {
        ViveColliderButtonEventData viveEventData;
        if (!grabbedObj.grabbedEvent.TryGetViveButtonEventData(out viveEventData)) { return; }

        switch (viveEventData.viveRole.ToRole<HandRole>())
        {
            case HandRole.RightHand:
                if (rightGrabbingSet.Add(grabbedObj.gameObject) && rightGrabbingSet.Count == 1)
                {
                    //UpdateActivity();
                }
                break;

            case HandRole.LeftHand:
                if (leftGrabbingSet.Add(grabbedObj.gameObject) && leftGrabbingSet.Count == 1)
                {
                    //UpdateActivity();
                }
                break;
        }
    }

    public void OnRelease(BasicGrabbable releasedObj)
    {
        ViveColliderButtonEventData viveEventData;
        if (!releasedObj.grabbedEvent.TryGetViveButtonEventData(out viveEventData)) { return; }

        switch (viveEventData.viveRole.ToRole<HandRole>())
        {
            case HandRole.RightHand:
                if (rightGrabbingSet.Remove(releasedObj.gameObject) && rightGrabbingSet.Count == 0)
                {
                    //UpdateActivity();
                }
                break;

            case HandRole.LeftHand:
                if (leftGrabbingSet.Remove(releasedObj.gameObject) && leftGrabbingSet.Count == 0)
                {
                    //UpdateActivity();
                }
                break;
        }
    }
}
