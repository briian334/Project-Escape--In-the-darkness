using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spotlight : MonoBehaviour
{

    public Light flashlight; // Linterna

    void Start()
    {
        flashlight.enabled = false; // Asegurarse de que la linterna esté apagada inicialmente
    }

    public void ActivarObjeto()
    {
        // Esta función ahora solo activará la linterna la primera vez.
        flashlight.enabled = true; // Asegúrate de que la linterna esté encendida al activar el objeto.
        Destroy(gameObject); // Destruir el objeto después de activarlo.
    }

    void Update()
    {
        flashlight.enabled = false; // Asegurarse de que la linterna esté apagada inicialmente
    }

}
