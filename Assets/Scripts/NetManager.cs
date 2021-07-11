using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetManager : MonoBehaviourPunCallbacks
{
    #region Private Serializable Fields


    #endregion

    #region Private Fields

    /// <summary>
    /// Version number
    /// </summary>
    private const string gameVersion = "1.0";

    #endregion

    #region Public Fields

    //public byte numPlayers = 1;

    #endregion

    #region MonoBehaviour CallBacks


    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
    /// </summary>
    void Awake()
    {
        // #Critical
        //this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization phase
    /// </summary>
    void Start()
    {
        //Connect();
        PhotonNetwork.AuthValues = new AuthenticationValues();
    }

    void Update()
    {
        Debug.Log("Total Current Rooms: " + PhotonNetwork.CountOfRooms.ToString());
        Debug.Log("Current Room: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Players expected: " + PhotonNetwork.CurrentRoom.MaxPlayers.ToString() + ". Players In: " + PhotonNetwork.CurrentRoom.PlayerCount.ToString());
        Debug.Log("UserID " + PhotonNetwork.AuthValues.UserId);
    }


    #endregion

    #region MonoBehaviourPunCallBacks CallBacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN");
        //Connect();
        base.OnConnectedToMaster();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("OnDisconnected() was called by PUN");
        base.OnDisconnected(cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed() was call by PUN");
        //PhotonNetwork.CreateRoom(null, new RoomOptions() {MaxPlayers = numPlayers});
        base.OnJoinRandomFailed(returnCode, message);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() was called by PUN");
        base.OnJoinedRoom();
    }

    #endregion

    #region Public Metods

    /// <summary>
    /// Start the connection process.
    /// - If already connected, we attempt joining a random room
    /// - if not yet connected, Connect this application instance to Photon Cloud Network
    /// </summary>
    public void Connect()
    {
        //check if connected
        if (PhotonNetwork.IsConnected)
        {
            //#Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
            //PhotonNetwork. no se q a pasado, a aparecido esto de repente
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.GameVersion = gameVersion;
            //PhotonNetwork. no se q a pasado, a aparecido esto de repente
        }
    }

    public void Disconnect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    public bool CreateTheRoom(byte numPlayers)
    {
        Disconnect();
        if (numPlayers == 1)
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.OfflineMode = true;
        }
        else
        {
            PhotonNetwork.OfflineMode = false;
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        PhotonNetwork.JoinLobby(new TypedLobby("ThaRoom", LobbyType.Default));
        return PhotonNetwork.CreateRoom("Sala"+numPlayers.ToString(), new RoomOptions() { IsVisible = true, MaxPlayers = numPlayers });
    }

    public bool JoinTheRoom()
    {
        Disconnect();
        PhotonNetwork.OfflineMode = false;
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
        return PhotonNetwork.JoinRoom("Sala2");
    }

    public void SetOffline(bool x)
    {
        if (x)
        {
            Disconnect();
            PhotonNetwork.OfflineMode = x;
            PhotonNetwork.CreateRoom("Offline", new RoomOptions() { MaxPlayers = 1 });
        }
        else
        {
            PhotonNetwork.OfflineMode = x;
        }
      
    }

    public void SetId(string x)
    {
        PhotonNetwork.AuthValues.UserId = x;
    }

    #endregion

}