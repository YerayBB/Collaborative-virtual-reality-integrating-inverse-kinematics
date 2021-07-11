using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    #region private serializable variables
    [SerializeField]
    private PloyerControler ployerControler = null;
    [SerializeField]
    public Animator animator = null;
    #endregion

    #region private variables
   
    #endregion

    #region Monobehaviour CallBacks
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (ployerControler.isMoving) animator.SetBool("Moving", true);
        else animator.SetBool("Moving", false);
    }
    #endregion

    #region Public metods

    public void SetAnimations(bool off)
    {
        animator.SetBool("Stop", off);
    }

    #endregion
}
