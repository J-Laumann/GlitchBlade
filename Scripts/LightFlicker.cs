using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public float minWait, maxWait;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        Invoke("StartFlicker", Random.Range(minWait, maxWait));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartFlicker()
    {
        anim.Play("LightFlicker");
        Invoke("StartFlicker", Random.Range(minWait, maxWait));
    }
}
