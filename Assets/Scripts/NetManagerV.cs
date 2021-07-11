using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class NetManagerV : MonoBehaviourPunCallbacks, IOnEventCallback
{
    #region Private serializable fields

    /*[SerializeField]
    private byte maxplayers = 2;*/
    [SerializeField]
    private InputField statusConnection;
    [SerializeField]
    private InputField statusRoom;
    [SerializeField]
    private InputField statusPlayers;
    [SerializeField]
    private Button buttonDisconnect;
    [SerializeField]
    private Button buttonNext;
    [SerializeField]
    private Button buttonSingle;
    [SerializeField]
    private Button buttonHost;
    [SerializeField]
    private Button buttonJoin;
    [SerializeField]
    private Button buttonStart;

    #endregion

    #region Public fields

    public bool wannaMaster;
    public bool sincro = true;
    public bool avatarOn = true;
    public bool neverever = false;

    #endregion

    #region Private Fields

    private const string gameVersion = "1.0";
    private const byte optionsSet = 0;
    private bool hasStarted = false;
    private bool externalcauses = false;

    #endregion

    #region MonoBehaviour CallBacks

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        neverever = false;
        //Connect();
        statusConnection.text = "Desconectado";
        statusPlayers.text = "0/0";
        statusRoom.text = "No en sala";
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogErrorFormat("I AM THE MASTER");
        }
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("In Room: " + PhotonNetwork.CurrentRoom.Name);
            Debug.Log("Players expected: " + PhotonNetwork.CurrentRoom.MaxPlayers.ToString() + "/Players in: " + PhotonNetwork.CurrentRoom.PlayerCount.ToString());
        }
    }

    void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    #endregion

    #region MonoBehaviourPunCallBacks CallBacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster called by pun on mode "+ (PhotonNetwork.OfflineMode ? "Offline":"Online"));
        statusConnection.text = "Conectando";
        if (PhotonNetwork.OfflineMode)
        {
            PhotonNetwork.CreateRoom("Sala1", new Photon.Realtime.RoomOptions() { MaxPlayers = 1 });
        }
        else
        {
            if (wannaMaster)
            {
                PhotonNetwork.CreateRoom("SalaMulti", new Photon.Realtime.RoomOptions() { MaxPlayers = 2 });
            }
            else
            {
                PhotonNetwork.JoinRoom("SalaMulti");
            }
        }
        /*
        PhotonNetwork.JoinRandomRoom();
        //PhotonNetwork.JoinRoom("Sala");*/
        base.OnConnectedToMaster();
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.LogWarningFormat("OnDisconnected called by pun, was caused by ", cause);
        buttonNext.interactable = false;
        buttonDisconnect.interactable = false;
        if (!externalcauses)
        {
            statusConnection.text = "Desconectado";
            statusPlayers.text = "0/0";
            statusRoom.text = "No en sala";
        }
        else externalcauses = false;
        buttonHost.interactable = true;
        buttonSingle.interactable = true;
        buttonJoin.interactable = true;
        base.OnDisconnected(cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed called by pun, we create one room, cause?: " + message + "FAKE MESAGE");
        /*PhotonNetwork.CreateRoom("Sala", new Photon.Realtime.RoomOptions() { MaxPlayers = maxplayers });*/
        base.OnJoinRandomFailed(returnCode, message);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom called by PUN, we are in the room: " + PhotonNetwork.CurrentRoom.Name);
        statusConnection.text = "Conectado " + (PhotonNetwork.OfflineMode ? "single":"multi");
        statusRoom.text = PhotonNetwork.CurrentRoom.Name;
        statusPlayers.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString() + "/" + PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
        buttonDisconnect.interactable = true;
        buttonNext.interactable = true;
        buttonHost.interactable = false;
        buttonSingle.interactable = false;
        buttonJoin.interactable = false;
        if (PhotonNetwork.IsMasterClient) buttonStart.interactable = true;
        else buttonStart.interactable = false;
        base.OnJoinedRoom();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRoomFailed called by pun, cause?: " + message);
        statusConnection.text = "Error de conexión";
        buttonDisconnect.interactable = false;
        buttonNext.interactable = false;
        buttonHost.interactable = true;
        buttonSingle.interactable = true;
        buttonJoin.interactable = true;
        externalcauses = true;
        Disconnect();
        //PhotonNetwork.CreateRoom("Sala", new Photon.Realtime.RoomOptions() { MaxPlayers = maxplayers });
        base.OnJoinRoomFailed(returnCode, message);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnCreateRoomFalied called by pun");
        statusConnection.text = "Error al crear sala";
        buttonDisconnect.interactable = false;
        buttonNext.interactable = false;
        buttonHost.interactable = true;
        buttonSingle.interactable = true;
        buttonJoin.interactable = true;
        externalcauses = true;
        Disconnect();
        base.OnCreateRoomFailed(returnCode, message);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("OnPlayerEnteredRoom called by pun");
        statusPlayers.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString() + "/" + PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
        base.OnPlayerEnteredRoom(newPlayer);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        Debug.Log("EVENT RECEIVED CODE " + eventCode.ToString());

        if (eventCode == optionsSet)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                bool[] aux = (bool[])photonEvent.CustomData;
                sincro = aux[0];
                avatarOn = aux[1];

                Debug.Log("STARSTEVENTNM");

                buttonStart.interactable = true;
                hasStarted = true;
            }
            neverever = true;
        }
    }

    #endregion

    #region Public Metods

    public void SetMaster(bool mas)
    {
        wannaMaster = mas;
    }

    public void SetCauses(bool x)
    {
        externalcauses = x;
    }

    public void SetSincronize(bool x)
    {
        sincro = x;
    }

    public void SetAvatarOn(bool x)
    {
        avatarOn = x;
    }

    public void DelayedConnect(bool off)
    {
        PhotonNetwork.OfflineMode = off;
        if (!off)
        {
            Debug.Log("Pero que");
            Invoke("Connect", 0.01f);
        }
    }

    public void SesionStartEvent()
    {
        byte evCode = 0;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        bool[] data = {sincro, avatarOn};
        PhotonNetwork.RaiseEvent(evCode, data, raiseEventOptions, sendOptions);
    }

    public void Connect()
    {

        if (!PhotonNetwork.OfflineMode)
        {
            if (PhotonNetwork.IsConnected)//caso reintentar conectar sala
            {
                Debug.Log("reiniciando conexion");
                Debug.LogErrorFormat("ESTO VA A EXPLOTAR");
                Disconnect();
                Debug.Log("Really llegaste aqui? BOOOM");
                Connect();
                return;
                //PhotonNetwork.JoinRandomRoom();
                //PhotonNetwork.JoinRoom("Sala");
            }
            else
            {
                Debug.Log("Connectando");
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
    }

    public void Disconnect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    #endregion

}
