using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleDestroyTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject Solucion;


    void OnDestroy()
    {
        //Debug.Log("DESTRUIDO " + name);
        Solucion.SetActive(true);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
