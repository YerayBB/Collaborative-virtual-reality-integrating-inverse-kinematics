using UnityEngine;
using UnityEngine.UI;

public class ActivaToggle : MonoBehaviour
{
    private Toggle toggle = null;
    #region Monobehaviour CallBacks
    // Start is called before the first frame update
    void Start()
    {
        toggle = gameObject.GetComponent<Toggle>();
    }
    #endregion

    #region public metods
    public void Change()
    {
        toggle.interactable ^= true;
    }
    #endregion
}
