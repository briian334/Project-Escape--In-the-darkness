using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalContador : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private ControladorJuego controladorJuego;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            controladorJuego.DesactivarTemporizador();
        }
    }
}
