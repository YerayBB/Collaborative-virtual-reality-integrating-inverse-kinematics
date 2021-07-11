using UnityEngine;
using Photon.Pun;

public class CilinderAdjust : MonoBehaviourPunCallbacks, IPunObservable
{
    #region Private variables
    private readonly float radio = 0.23f;
    #endregion

    #region Public variables
    public GameObject player = null;
    #endregion

    #region Monobehaviour CallBacks
    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
        if (player == null)
        {
            Debug.Log("HOLAAAA!");
            gameObject.SetActive(false);
            Invoke("Reenable", 0.01f);
        }
        else
        {
            transform.parent = GameObject.Find("The Center").transform;
            Vector3 actual = gameObject.transform.localScale;
            actual.x = player.transform.localScale.x * radio * 2;
            gameObject.transform.localScale = new Vector3(actual.x, 3, actual.x);
            float aux = actual.x / 2;
            /*actual = gameObject.transform.position;
            gameObject.transform.position = new Vector3(3.15f + aux, actual.y, 1.3f + aux);*/
            transform.localPosition = new Vector3(aux, 0, aux);
        }
    }
    #endregion

    #region Private metods
    private void Reenable()
    {
        gameObject.SetActive(true);
    }
    #endregion

    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(gameObject.activeSelf);
        }
        else
        {
            gameObject.SetActive((bool)stream.ReceiveNext());
        }
    }

    #endregion
}
