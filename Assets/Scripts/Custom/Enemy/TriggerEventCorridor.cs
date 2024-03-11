using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEventCorridor : MonoBehaviour
{
    public GameObject Guardia;
    private void OnTriggerEnter(Collider other)
    {
        Guardia.SetActive(true);
        Destroy(this.gameObject);
    }
}
