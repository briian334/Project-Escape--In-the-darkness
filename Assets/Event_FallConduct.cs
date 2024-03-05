using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_FallConduct : MonoBehaviour
{
    public GameObject gamConduct;

    private void OnTriggerEnter(Collider other)
    {
        gamConduct.GetComponent<Animator>().enabled = true;
        
    }
}
