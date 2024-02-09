using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mov_Climb : MonoBehaviour
{
    public PlayerInput pyiPlayerInput;    
    public float climbSpeed = 3.0f; // Velocidad de subida    
    private bool isClimbing = false; // Estado de subida

    void Start()
    {
        pyiPlayerInput = GetComponent<PlayerInput>();
    }

    
    void Update()
    {
        if (isClimbing)
        {
            transform.Translate(Vector3.up * climbSpeed * Time.deltaTime);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.CompareTag("Box")) // Asegúrate de que la caja tenga la etiqueta "Caja"
        {
            isClimbing = pyiPlayerInput.actions["Interact"].WasPressedThisFrame();
            Debug.Log("SUBEEEEE");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Box"))
        {
            isClimbing = false;
        }
    }
}
