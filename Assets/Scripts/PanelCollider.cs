using UnityEngine;
using Photon.Pun;

public class PanelCollider : MonoBehaviour
{
    #region MonoBehaviourCallBacks
    private void OnTriggerStay(Collider other)
    {
        
        if (other.tag == gameObject.tag)
        {
            Debug.Log(other.name + " colided");
            gameObject.transform.Find(other.name).gameObject.SetActive(true);
            //Destroy(other.gameObject);
            PhotonNetwork.Destroy(other.gameObject);
        }
    }
    #endregion
}
