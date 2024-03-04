using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spotlight : MonoBehaviour
{

    public Light flashlight; // Linterna

    void Start()
    {
        flashlight.enabled = false; // Asegurarse de que la linterna est� apagada inicialmente
    }

    public void ActivarObjeto()
    {
        // Esta funci�n ahora solo activar� la linterna la primera vez.
        flashlight.enabled = true; // Aseg�rate de que la linterna est� encendida al activar el objeto.
        Destroy(gameObject); // Destruir el objeto despu�s de activarlo.
    }

    void Update()
    {
        flashlight.enabled = false; // Asegurarse de que la linterna est� apagada inicialmente
    }

}
