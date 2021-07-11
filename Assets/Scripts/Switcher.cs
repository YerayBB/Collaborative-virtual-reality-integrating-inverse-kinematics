using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switcher : MonoBehaviour
{
    public void clic()
    {
        gameObject.SetActive(gameObject.activeSelf ^ true);
    }
}
