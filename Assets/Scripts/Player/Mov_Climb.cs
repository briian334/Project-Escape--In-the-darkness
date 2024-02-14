using DG.Tweening;
using StarterAssets;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Mov_Climb : MonoBehaviour
{
    #region Variables:
    [SerializeField] LayerMask layerMask;
    [SerializeField] float numMaxDistanceRay = 1f;
    CharacterController characterController;
    PlayerInput playerInput;
    bool booIsClimbing;
    bool booWantClimb;
    #endregion
    #region pruebas
    Animator animator;
    FirstPersonController firstPersonController;
    IEnumerator ienClimbing;
    BasicRigidBodyPush basicRigidBodyPush;
    [SerializeField] LayerMask ducto;
    public Transform puntoDeEntrada; // Punto de entrada al ducto
    public float duracionAnimacion = 2f; // Duración de la animación
    #endregion

    private void Start()
    {
        characterController = GetComponentInParent<CharacterController>();
        playerInput = GetComponentInParent<PlayerInput>();
        animator = GetComponentInParent<Animator>();
        firstPersonController = GetComponentInParent<FirstPersonController>();
        basicRigidBodyPush = GetComponentInParent<BasicRigidBodyPush>();
    }
    private void FixedUpdate()
    {
        ienClimbing = FnClimb();
        //Vector3 ve3CurrentPostion = gameobject.transform.position;
        Ray rayPlayerHead = new Ray(transform.position, transform.forward);
        RaycastHit ryhPlayerHeadHit; //DETECTA COLISION PARA ESCALAR
        //booIsClimbing = false;
        if (Physics.Raycast(rayPlayerHead.origin, rayPlayerHead.direction, out ryhPlayerHeadHit, numMaxDistanceRay, layerMask))
        {
                basicRigidBodyPush.canPush = true;
                booWantClimb = playerInput.actions["Climb"].IsPressed();
                Debug.DrawRay(rayPlayerHead.origin, rayPlayerHead.direction * numMaxDistanceRay, Color.red);
                if (booWantClimb && !booIsClimbing)
                {
                    basicRigidBodyPush.canPush = false;
                    booIsClimbing = true;
                    Debug.Log($"Point: {ryhPlayerHeadHit.point}");
                    StartCoroutine(ienClimbing);
                }                        
        }
        else
        {
            Debug.DrawRay(rayPlayerHead.origin, rayPlayerHead.direction * numMaxDistanceRay, Color.white);
            basicRigidBodyPush.canPush = false;
        }
        if (booIsClimbing == false)
        {
            StopCoroutine(ienClimbing);
        }
        if (Physics.Raycast(rayPlayerHead.origin, rayPlayerHead.direction, out RaycastHit ductoHit, numMaxDistanceRay, ducto))
        {
            if (playerInput.actions["Climb"].IsPressed())
            {
                Debug.DrawRay(rayPlayerHead.origin, rayPlayerHead.direction * numMaxDistanceRay, Color.blue);
                // Mover el personaje hacia el punto de entrada del ducto
                
                characterController.transform.DOMoveY(puntoDeEntrada.transform.position.y, 1.5f);
                characterController.transform.DOScaleY(0.5f, duracionAnimacion);                
                characterController.transform.DOMove(puntoDeEntrada.position, duracionAnimacion)
                    .SetEase(Ease.InOutQuad); // Aplicar una curva de easing para suavizar la animación                
            }
            
        }
    }
    //CORUTINA PARA EL EVENTO DE ESCALAR
    IEnumerator FnClimb()
    {
        if (booIsClimbing)
        {
            firstPersonController.enabled = false;
            animator.applyRootMotion = true;
            animator.SetBool("IsClimbing", true);
            yield return new WaitForSecondsRealtime(2);
            animator.SetBool("IsClimbing", false);
            firstPersonController.enabled = true;
            booIsClimbing = false;
            animator.applyRootMotion = false;
        }        
    }
}