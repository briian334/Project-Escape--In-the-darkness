using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mov_Player : MonoBehaviour
{
    float glNumPlayerSpeed = 5f;
    CharacterController glChPlayerControl;
    void Start()
    {
       glChPlayerControl = GetComponent<CharacterController>();
    }
    void Update()
    {
        //VARIABLES PARA DETECTAR EL MOVIMIENTO DEL JUGADOR
        Vector3 ve3PlayerInput = Vector3.zero;
        float numPlayerMovHorizon = Input.GetAxis("Horizontal");
        float numPlayerMovVerti = Input.GetAxis("Vertical");
        if (ve3PlayerInput.magnitude >= 0.1f)
        {

        }
    }
    private void fnMovement(Vector3 pVe3PlayerInput)
    {
        glChPlayerControl.SimpleMove(pVe3PlayerInput.normalized * glNumPlayerSpeed);
    }
}