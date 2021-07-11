using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepCollider : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] fxsteps;

    private AudioSource audioSource;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        /*Debug.Log(this.name + " trigger con " + other.name);*/
        stepon();
    }

    public void stepon()
    {
        AudioClip clip = fxsteps[UnityEngine.Random.Range(0, fxsteps.Length)];
        audioSource.PlayOneShot(clip);
    }
}
