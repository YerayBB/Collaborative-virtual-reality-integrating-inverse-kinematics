using HTC.UnityPlugin.Vive;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class MenuController : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public GameObject[] models = new GameObject[2];
    public int selected = 0;
    public GameObject avatarless;
    public GameObject[] subcom = new GameObject[3];

    public GameObject basemenu;
    public GameObject secondmenu;

    public bool avatarOn = true;
    public bool neverever = false;

    private string[] descrip = { "Fijar", "Desfijar" };
    private bool windowmode = false;//false = modelselection
    [SerializeField]
    private PhotonView sc = null;

    [Header("VRIKCalibrator")]
    [Tooltip("Reference to the VRIK component on the avatar.")] public VRIK ik;
    [Tooltip("The settings for VRIK calibration.")] public VRIKCalibrator.Settings settings;
    [Tooltip("The HMD.")] public Transform headTracker = null;
    [Tooltip("(Optional) A tracker on the body of the player on the belt area.")] public Transform bodyTracker = null;
    [Tooltip("(Optional) A hand controller device placed in the player's left hand.")] public Transform leftHandTracker = null;
    [Tooltip("(Optional) A hand controller device placed in the player's right hand.")] public Transform rightHandTracker = null;
    [Tooltip("(Optional) A tracker placed anywhere on the ankle of the player's left leg.")] public Transform leftFootTracker = null;
    [Tooltip("(Optional) A tracker placed anywhere on the ankle of the player's right leg.")] public Transform rightFootTracker = null;


    public GameObject hDummy = null;
    public GameObject lhDummy = null;
    public GameObject rhDummy = null;
    public GameObject bDummy = null;
    public GameObject lfDummy = null;
    public GameObject rfDummy = null;

    private float timecounter = 0;
    private bool done = false;
    private bool done2 = false;

    [Tooltip("Obstacle, requires a cilinderadjust and being unactive")]public GameObject column;
    public ControllerManagerV cmv;

    [Header("Datacollector options")]
    public DataCollector collector;
    //public bool conobs = true;
    /*
    [Tooltip("Name of the file")] public string filename = "TEST";
    [Tooltip("Adds an index to the filename")] public bool indexed;
    [Tooltip("Reset the index of the file")] public bool reset;*/

    #region Private metods
    private void LoadReferences()
    {
        GameObject vrbase = GameObject.FindGameObjectsWithTag("MainCamera")[0].transform.parent.gameObject;
        if (headTracker == null)
        {
            headTracker = GameObject.FindGameObjectsWithTag("MainCamera")[0].transform;
            if (PhotonNetwork.CurrentRoom.MaxPlayers != 1)
            {
                if (hDummy == null) hDummy = PhotonNetwork.Instantiate("HeadDummy", Vector3.zero, Quaternion.identity);
                //sc.RPC("SetHeadReference", RpcTarget.OthersBuffered, hDummy.GetComponent<PhotonView>().ViewID);
            }
        }
        Transform controllers = vrbase.transform.Find("ViveControllers");
        if (leftHandTracker == null)
        {
            leftHandTracker = controllers.Find("Left/RenderModel/DeviceTracker");
            if (PhotonNetwork.CurrentRoom.MaxPlayers != 1)
            {
                if (lhDummy == null) lhDummy = PhotonNetwork.Instantiate("LeftHandDummy", Vector3.zero, Quaternion.identity);
                //sc.RPC("SetLHReference", RpcTarget.OthersBuffered, lhDummy.GetComponent<PhotonView>().ViewID);
            }
        }
        if (rightHandTracker == null)
        {
            rightHandTracker = controllers.Find("Right/RenderModel/DeviceTracker");
            if (PhotonNetwork.CurrentRoom.MaxPlayers != 1)
            {
                if (rhDummy == null) rhDummy = PhotonNetwork.Instantiate("RightHandDummy", Vector3.zero, Quaternion.identity);
                //sc.RPC("SetRHReference", RpcTarget.OthersBuffered, rhDummy.GetComponent<PhotonView>().ViewID);
            }
        }
        Transform trackers = vrbase.transform.Find("ViveTrackers");
        if (bodyTracker == null)
        {
            bodyTracker = trackers.GetChild(0).Find("DeviceTracker");
            if (PhotonNetwork.CurrentRoom.MaxPlayers != 1)
            {
                if (bDummy == null) bDummy = PhotonNetwork.Instantiate("BodyDummy", Vector3.zero, Quaternion.identity);
                //sc.RPC("SetBodyReference", RpcTarget.OthersBuffered, bDummy.GetComponent<PhotonView>().ViewID);
            }
        }
        if (leftFootTracker == null)
        {
            leftFootTracker = trackers.GetChild(1).Find("DeviceTracker");
            if (PhotonNetwork.CurrentRoom.MaxPlayers != 1)
            {
                if (lfDummy == null) lfDummy = PhotonNetwork.Instantiate("LeftFootDummy", Vector3.zero, Quaternion.identity);
                //sc.RPC("SetLFReference", RpcTarget.OthersBuffered, lfDummy.GetComponent<PhotonView>().ViewID);
            }
        }
        if (rightFootTracker == null)
        {
            rightFootTracker = trackers.GetChild(2).Find("DeviceTracker");
            if (PhotonNetwork.CurrentRoom.MaxPlayers != 1)
            {
                if (rfDummy == null) rfDummy = PhotonNetwork.Instantiate("RightFootDummy", Vector3.zero, Quaternion.identity);
                //sc.RPC("SetRFReference", RpcTarget.OthersBuffered, rfDummy.GetComponent<PhotonView>().ViewID);
            }
        }
    }

    private bool NotReady()
    {
        return headTracker == null || leftFootTracker == null || leftHandTracker == null || rightFootTracker == null || rightHandTracker == null || bodyTracker == null;
    }

    private void BalanceFeet()
    {
        if (NotReady()) LoadReferences();
        Vector3 aux = leftFootTracker.transform.eulerAngles;
        Vector3 auxO = leftFootTracker.GetComponent<VivePoseTracker>().rotOffset;
        auxO.x = (-90 - aux.x)%360;
        leftFootTracker.GetComponent<VivePoseTracker>().rotOffset = auxO;

        aux = rightFootTracker.transform.eulerAngles;
        auxO = rightFootTracker.GetComponent<VivePoseTracker>().rotOffset;
        auxO.x = (-90 - aux.x)%360;
        rightFootTracker.GetComponent<VivePoseTracker>().rotOffset = auxO;
    }
    #endregion

    #region MonoBehaviour CallBacks
    private void Start()
    {
        /*subcom[0] = gameObject.transform.Find("Toggle group").gameObject;
        subcom[1] = gameObject.transform.Find("Button").gameObject;
        subcom[2] = gameObject.transform.Find("Text").gameObject;*/
        /*if (photonView.IsMine)
        {*/
        //gameObject.GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectsWithTag("PlayerCamera")[0].GetComponent<Camera>();
        GameObject auxcmv = GameObject.FindGameObjectsWithTag("GameController")[0];
        auxcmv.tag = PhotonNetwork.IsMasterClient ? "Puzzle1" : "Puzzle2";
        /*the old way
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject[] pieces = GameObject.FindGameObjectsWithTag("Puzzle1");
            foreach (GameObject one in pieces)
            {
                if (one.GetComponent<PhotonView>() != null)
                {
                    one.GetComponent<PhotonView>().RequestOwnership();
                }
            }
            auxcmv.tag = "Puzzle1";
        }
        else
        {
            GameObject[] pieces = GameObject.FindGameObjectsWithTag("Puzzle2");
            foreach (GameObject one in pieces)
            {
                if (one.GetComponent<PhotonView>() != null)
                {
                    one.GetComponent<PhotonView>().RequestOwnership();
                }
            }
            auxcmv.tag = "Puzzle2";
        }*/
        /*The New One*/
        GameObject[] pieces = GameObject.FindGameObjectsWithTag("Puzzle1");
        PhotonView onepv;
        Rigidbody rb;
        foreach (GameObject one in pieces)
        {
            onepv = one.GetComponent<PhotonView>();
            if (onepv != null)
            {
                if (PhotonNetwork.IsMasterClient) onepv.RequestOwnership();
                else
                {
                    rb = one.GetComponent<Rigidbody>();
                    rb.useGravity = false;
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                }
            }
        }
        pieces = GameObject.FindGameObjectsWithTag("Puzzle2");
        foreach (GameObject one in pieces)
        {
            onepv = one.GetComponent<PhotonView>();
            if (onepv != null)
            {
                if (!PhotonNetwork.IsMasterClient) onepv.RequestOwnership();
                else
                {
                    rb = one.GetComponent<Rigidbody>();
                    rb.useGravity = false;
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                }
            }
        }

        

        cmv = auxcmv.GetComponent<ControllerManagerV>();
        
        Debug.Log("Finished");
        /*GameObject vrbase = GameObject.FindGameObjectsWithTag("MainCamera")[0].transform.parent.gameObject;
        headTracker = vrbase.transform.Find("Camera");
        Transform controllers = vrbase.transform.Find("ViveControllers");
        leftHandTracker = controllers.Find("Left/RenderModel/DeviceTracker");
        rightHandTracker = controllers.Find("Right/RenderModel/DeviceTracker");
        Transform trackers = vrbase.transform.Find("ViveTrackers");
        bodyTracker = trackers.GetChild(0).Find("DeviceTracker");
        leftFootTracker = trackers.GetChild(1).Find("DeviceTracker");
        rightFootTracker = trackers.GetChild(2).Find("DeviceTracker");*/
        LoadReferences();

        collector = GameObject.Find("DataCollector").GetComponent<DataCollector>();

        done = true;
        /*}*/
    }

    // Update is called once per frame
    void Update()
    {
        if (neverever)
        {


            if (NotReady() && done)
            {
                Debug.Log("NOT READY");
                LoadReferences();
            }
            if (windowmode)
            {
                if (ViveInput.GetPressDownEx(HandRole.RightHand, ControllerButton.FullTrigger) && ViveInput.GetPressEx(HandRole.LeftHand, ControllerButton.FullTrigger))
                {
                    timecounter = Time.time;
                    //Debug.Log(this.photonView.Owner);
                    BalanceFeet();
                    Debug.Log("HA " + timecounter);
                }
                if (ViveInput.GetPressDownEx(HandRole.LeftHand, ControllerButton.FullTrigger) && ViveInput.GetPressEx(HandRole.RightHand, ControllerButton.FullTrigger))
                {
                    timecounter = Time.time;
                    //Debug.Log(this.photonView.Owner);
                    BalanceFeet();
                    Debug.Log("HAHA " + timecounter);
                }
                if (ViveInput.GetPressEx(HandRole.RightHand, ControllerButton.FullTrigger) && ViveInput.GetPressEx(HandRole.LeftHand, ControllerButton.FullTrigger))
                {
                    if (Time.time - timecounter >= 2.9 && !done2)
                    {
                        if (PhotonNetwork.CurrentRoom.MaxPlayers != 1)
                        {
                            Debug.Log("SUFFEEEEEER");
                            sc.RPC("CalibratePlayer", RpcTarget.OthersBuffered, models[selected].GetComponent<PhotonView>().ViewID);
                            Debug.LogWarningFormat("YOU CALL THAT A WEAPON?");
                        }
                        done2 = true;
                    }
                    if (Time.time - timecounter >= 3)
                    {
                        Debug.Log("THE TIME HAS COOOOME");
                        //Debug.Log(this.photonView.Owner);
                        CalibrationOn();
                        timecounter = Time.time;
                    }
                }
            }
            /*}*/
        }
        else
        {
            Debug.Log("NEVEEEEER PLS");
            NetManagerV go = GameObject.Find("NetworkManager").GetComponent<NetManagerV>();
            avatarOn = go.avatarOn;
            neverever = go.neverever;
            if (neverever)
            {
                if (avatarOn)
                {
                    Debug.Log("CON AVATAR");
                    if (avatarOn)
                    {
                        bool sincro = GameObject.Find("NetworkManager").GetComponent<NetManagerV>().sincro;
                        models[0] = PhotonNetwork.Instantiate("Andrew-MED_Unity", new Vector3(4, 0, 5), Quaternion.identity);
                        models[1] = PhotonNetwork.Instantiate("Sandra-MED_Unity", new Vector3(4, 0, 5), Quaternion.identity);
                        sc = GameObject.Find("SecondCalibrator").GetComponent<PhotonView>();
                        models[0].GetComponent<AnimatorController>().SetAnimations(sincro);
                        models[1].GetComponent<AnimatorController>().SetAnimations(sincro);
                    }

                    basemenu.SetActive(true);
                    secondmenu.SetActive(false);
                }
                else
                {
                    Debug.Log("LA BOLA");
                    basemenu.SetActive(false);
                    secondmenu.SetActive(true);
                    gameObject.SetActive(true);
                }
            }
        }
    }

    void OnEnable()
    {
        
    }
    #endregion

    #region Public metods
    public void ChangeModel(int x)
    {
        if (selected != x) selected = x;
    }
    
    public void Locked()
    {
        subcom[0].SetActive(subcom[0].activeSelf ^ true);
        windowmode = windowmode ^ true;
        subcom[1].GetComponentInChildren<Text>().text = descrip[windowmode ? 1 : 0];
        subcom[2].SetActive(subcom[2].activeSelf ^ true);
    }



    public void CalibrationOn()
    {
        GameObject player;
        if (avatarOn)
        {
            Debug.Log("YOU NOT BELONG HEEEEREEEEE");
            player = models[selected];
            int aux = selected == 1 ? 0 : 1;
            PhotonNetwork.Destroy(models[aux]);

            cmv.avatarvr = player;

            Debug.Log(Time.time);
            Debug.Log("MY POWER IS UNMATCH");
            Debug.Log(Time.time);
            float hahas = Time.time;
            //while (Time.time - hahas <= 10) Debug.Log("MATAME YA PLEASE"+ (Time.time-hahas));
            Debug.Log(Time.time);
            /*preparar los puntos de ancla del calibrator si es necesario para cada modelo*/
            //VRIK ik = player.AddComponent<VRIK>();
            /*player.GetComponent<Animator>().enabled = true;
            player.GetComponent<AudioSource>().enabled = true;
            player.GetComponent<PloyerControler>().enabled = true;
            player.GetComponent<StepCollider>().enabled = true;
            player.GetComponent<AnimatorController>().enabled = true;*/
            ik = player.GetComponent<VRIK>();


            /* sik.OnPreInitiate();
             sik.Initiate(player.transform);
             sik.OnPostInitiate();*/
            Debug.Log(Time.time);
            Debug.Log("MY POWERS WERE MATCHED");
            VRIKCalibrator.Calibrate(ik, settings, headTracker, bodyTracker, leftHandTracker, rightHandTracker, leftFootTracker, rightFootTracker);

            //player.SetActive(true);
            /*soloplayer mode aka cilindro*/
            /*if (conobs)
            {
                column.GetComponent<CilinderAdjust>().player = player;
                column.SetActive(true);
                collector.obstacle = column;
            }*/
            /*else
            {
                GameObject dummy = new GameObject("ObstacleDummy");
                dummy.transform.position = new Vector3(0,0,0);
                collector.obstacle = dummy;
            }*/
            //activar el datacollector
            /*collector.filename = filename;
            collector.resetb = reset;
            collector.indexed = indexed;*/

            Debug.LogAssertionFormat("HEEELLO");
            Debug.Log("LEFTING");
            Vector3 le = leftHandTracker.Find("Left Hand Target").transform.localPosition;
            le.x *= -1;
            leftHandTracker.Find("Left Hand Target").transform.localPosition = le;
            Debug.Log("LEFTED");


            HandPoser[] patta = player.GetComponentsInChildren<HandPoser>();
            int lindex = patta[0].name == "LeftHand" ? 0 : 1;
            patta[lindex].poseRoot = leftHandTracker.Find("Hand").transform;
            patta[lindex == 1 ? 0 : 1].poseRoot = rightHandTracker.Find("Hand").transform;

            /*player.transform.Find("LeftHand").GetComponent<HandPoser>().poseRoot = leftHandTracker.Find("Hand").transform;
            player.transform.Find("RightHand").GetComponent<HandPoser>().poseRoot = rightHandTracker.Find("Hand").transform;*/
        }
        else
        {
            if (hDummy == null) if (hDummy == null) hDummy = PhotonNetwork.Instantiate("HeadDummy", Vector3.zero, Quaternion.identity);
            player = hDummy;
            cmv.avatarvr = player;
            if (rhDummy == null) rhDummy = PhotonNetwork.Instantiate("RightHandDummy", Vector3.zero, Quaternion.identity);
            if (lhDummy == null) lhDummy = PhotonNetwork.Instantiate("LeftHandDummy", Vector3.zero, Quaternion.identity);
        }

            collector.GetComponent<PhotonView>().RPC("AddPlayerDC", RpcTarget.All, player.GetComponent<PhotonView>().ViewID, !PhotonNetwork.IsMasterClient);

            //collector.Activate();

            cmv.SwitchMode(true);
        

       

        Destroy(gameObject);
    }
    #endregion
}
