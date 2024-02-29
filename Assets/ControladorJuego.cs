using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControladorJuego : MonoBehaviour
{
    [SerializeField] private float tiempoMaximo;
    [SerializeField] private Slider slider;
    private float tiempoActual;
    private bool tiempoActivado = false;

   

    // Update is called once per frame
    private void Update()
    {
        if (tiempoActivado)
        {
            CambiarContador();
        }
    }

    private void CambiarContador()
    {
        //Reducir tiempo actual
        tiempoActual -= Time.deltaTime;

        //Si todavía hay tiempo
        if (tiempoActual >= 0)
        {
            slider.value = tiempoActual;
        }

        //Cuando se te acabe el tiempo
        if (tiempoActual <= 0)
        {
            Debug.Log("Perdiste");
            CambiarTemporizador(false);
        }
    }


    private void CambiarTemporizador(bool estado)
    {
        tiempoActivado = estado;
    }


    public void ActivarTemporizador()
    {
        tiempoActual = tiempoMaximo;
        slider.maxValue = tiempoMaximo;
        CambiarTemporizador(true);
    }

    public void DesactivarTemporizador()
    {
        CambiarTemporizador(false);
    }
}
