using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Box_Movement : MonoBehaviour
{
    #region Variables
    [SerializeField] float numPlayerStrength = 5f; //FUERZA CON LA QUE EL JUGADOR EMPUJA OBJETOS
    public PlayerInput pyiPlayerInput; //INSTANCIA AL SISTEMA DE ENTRADA (CONTROLES DE MOVIMIENTO)   
    public CharacterController chcCharacterController; //INSTANCIA AL CONTROLADOR DE FISICAS DEL JUGADOR
    public FirstPersonController firstPersonController; //INSTANCIA AL SCRIPT DE MOVIMIENTO DEL JUGADOR
    #endregion
    private void Start()
    {
        //SE INICIALIZAN LOS CONTROLES DEL JUGADOR
        pyiPlayerInput = GetComponent<PlayerInput>();
        //SE INICIALIZA EL CONTROL DE FISICAS DEL JUGADOR
        chcCharacterController = GetComponent<CharacterController>();
        //SE INICIALIZA EL SCRIPT DE MOVIMIENTO
        firstPersonController = GetComponent<FirstPersonController>();
    }
    private void OnControllerColliderHit(ControllerColliderHit pcchObjectHit)
    {
        //SI EL JUGADOR ESTA AGACHADO, NO PUEDE EMPUJAR
        if (firstPersonController.booIsPlayerCrouch)
            return;
        //SE OBTIENE EL RIGIDBODY DE LA CAJA A EMPUJAR
        Rigidbody rgbObjectCollision = pcchObjectHit.collider.attachedRigidbody;
        //SE OBTIENE EL TAG DEL OBJETO COLISIONADO Y SE CREA UNA CONDICION
        bool booIsaBox = pcchObjectHit.collider.CompareTag("Box");
        //SI EL RIGIDBODY ES NULO O KINEMATICO SE SALE DEL SCRIPT
        if (rgbObjectCollision == null || rgbObjectCollision.isKinematic)       
            return;
        //PARA EVITAR COLISIONAR CON OBJETOS DEBAJO DEL JUGADOR
        if (pcchObjectHit.moveDirection.y < -0.3)
            return;
        
        if (booIsaBox)
        {
            //SE VERIFICA QUE EL JUGADOR MANTIENE PRESIONADA LA TECLA PARA DICHA ACCION (EMPUJAR)
            if (pyiPlayerInput.actions["Push"].IsPressed())
            {
                Debug.Log("Empujanding...");
                //SE EJECUTA EL MOVIMIENTO A TRAVES DEL VECTOR DE POSICION PARA EMPUJAR LA CAJA
                Vector3 ve3Direccion = new(pcchObjectHit.moveDirection.x, 0f, pcchObjectHit.moveDirection.z);
                rgbObjectCollision.velocity = ve3Direccion * numPlayerStrength / rgbObjectCollision.mass;               
            }           
        }
    }
}