using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Int_ObjectSelected : MonoBehaviour
{
    #region Global Variables
    private PlayerInput _pyiPlayerInput; //DEFINE LOS CONTROLES DEL JUGADOR
    public Ray rayDetectInteractable; //RAYO QUE DETECTA OBJETOS INTERACTUABLES
    public LayerMask layLayerInteractable; //CAPA INTERACTUABLE DEL OBJETO (ASIGNAR EN INSPECTOR)
    [SerializeField] float numMaxDistanceRay = 0.5f; //DISTANCIA DEL RAYO PARA SELECCIONAR OBJETOS
    public Camera camMainCamera; //CAMARA PRINCIPAL PARA EMITIR EL RAYO DESDE ESA POSICION
    private Transform _traTransformCollision; //TRANSFORMACION DEL OBJETO COLISIONADO
    public GameObject gamCameraFollowTarget; //OBJETO QUE SIGUE LA VIRTUAL CAMERA DEL JUGADOR
    private CinemachineBasicMultiChannelPerlin _cbmVirtualCameraNoise; //PROPIEDAD QUE HACE FLOTAR LA CAMARA SIMULANDO RESPIRACION
    public CinemachineVirtualCamera cvcVirtualCamera; //CAMARA VIRTUAL DEL JUGADOR
    private Transform _traHighlightedObject; //OBJETO RESALTADO POR EL OUTLINE
    private RaycastHit _ryhPointCollison; //COLISION DEL RAYO
    #endregion
    #region Hidding Variables   
    public GameObject gamHideSpot; //EL PUNTO DE VISTA DENTRO DEL CASILLERO (ASIGNAR EN INSPECTOR)
    public bool booIsHiding = false; //ESTADO ESCONDIDO
    private Quaternion _quaOriginalRotation; //ROTACION ORIGINAL DE LA CAMARA ANTES DE ESCONDERSE
    private Vector3 _ve3OriginalPosition; //POSICION ORIGINAL DEL JUGADOR ANTES DE ESCONDERSE
    [SerializeField] private ControladorJuego controladorJuego;
    #endregion
    private void Start()
    {
        //SE INICIALIZAN LAS VARIABLES
        _pyiPlayerInput = GetComponent<PlayerInput>();
        _cbmVirtualCameraNoise = cvcVirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }
    void Update()
    {
        //CREA UN RAYO PARA DETECTAR OBJETOS INTERACTUABLES
        rayDetectInteractable = new(camMainCamera.transform.position, camMainCamera.transform.forward);
        //VISUALIZA EL RAYO EN EL EDITOR PARA DEPURACIÓN (NO VISIBLE EN EL JUEGO)
        Debug.DrawRay(rayDetectInteractable.origin, rayDetectInteractable.direction * numMaxDistanceRay, Color.white);
        //COMPRUEBA SI EL RAYO COLISIONA CON UN OBJETO INTERACTUABLE Y SI EL JUGADOR PRECIONA LA TECLA DE INTERACCIÓN
        if (Physics.Raycast(rayDetectInteractable.origin, rayDetectInteractable.direction, out _ryhPointCollison, numMaxDistanceRay, layLayerInteractable))
        {
            if (_ryhPointCollison.collider.gameObject.layer != LayerMask.NameToLayer("Interactable"))
            {
                return;
            }
            //OBTIENE EL OBJETO GOLPEADO POR EL RAYO
            Transform traHitObject = _ryhPointCollison.collider.transform;
            if (traHitObject.TryGetComponent<Outline>(out Outline o))
            {

            }
            else
            {
                Debug.Log("No tiene script outline...");
                return;
            }
            //SI NO ESTA ESCONDIDO SE ACTIVA EL DELINEADO
            if (!booIsHiding)
            {
                //ACTIVA EL DELINEADO DEL OBJETO GOLPEADO
                traHitObject.GetComponentInParent<Outline>().enabled = true;
            }            
            //DESACTIVA EL DELINEADO DEL OBJETO ANTERIOR GOLPEADO
            if (_traHighlightedObject != null && _traHighlightedObject != traHitObject)
            {
                _traHighlightedObject.GetComponentInParent<Outline>().enabled = false;
            }
            //ACTUALIZA EL OBJETO DELINEADO ACTUAL
            _traHighlightedObject = traHitObject;

            if (_pyiPlayerInput.actions["Interact"].WasPressedThisFrame())
            {
                _ryhPointCollison.collider.transform.GetComponentInParent<Outline>().enabled = false;
                //OBTIENE EL NOMBRE DE LA ETIQUETA DEL OBJETO COLISIONADO
                string strNameTag = _ryhPointCollison.collider.tag;
                //DETERMINA LA ACCIÓN A REALIZAR SEGÚN LA ETIQUETA DEL OBJETO
                switch (strNameTag)
                {
                    case "Locker":
                        try
                        {
                            //INTENTA ENCONTRAR UN OBJETO HIJO LLAMADO "INSIDE" DENTRO DEL CASILLERO PARA POSICIONAR LA CÁMARA
                            _traTransformCollision = _ryhPointCollison.transform.Find("Inside");
                            FnHiding(_traTransformCollision); //LLAMA A LA FUNCIÓN PARA ESCONDERSE EN EL CASILLERO
                        }
                        catch (System.Exception)
                        {
                            //MANEJA CUALQUIER EXCEPCIÓN QUE OCURRA AL BUSCAR EL OBJETO "INSIDE"
                            return;
                        }
                        break;
                    case "Puzzle":
                        if (!EventSystem.current.IsPointerOverGameObject())
                        {
                            // Send a function to the object we are aiming at
                            _ryhPointCollison.transform.gameObject.SendMessage("ActivateObject", 0, SendMessageOptions.DontRequireReceiver);
                        }
                        break;
                    default:
                        //SI NO ES UNA OPCION INTERACTUABLE, NO HACE NADA
                        return;
                }
            }
        }
        else
        {
            //SI EL RAYO NO COLISIONA CON UN OBJETO INTERACTUABLE, DESACTIVA EL DELINEADO ACTUAL
            if (_traHighlightedObject != null)
            {
                _traHighlightedObject.GetComponentInParent<Outline>().enabled = false;
                _traHighlightedObject = null;
            }
        }
    }
    public void FnHiding(Transform ptraHideSpot)
    {
        //ALTERNAR ENTRE ESCONDERSE Y NO ESCONDERSE
        booIsHiding = !booIsHiding;
        //SI EL JUGADOR SE ESTÁ ESCONDIENDO, LE DICE AL JUGADOR QUE SE ESCONDA Y LE DA LA UBICACIÓN DEL PUNTO DE VISTA DENTRO DE ESTE CASILLERO
        if (booIsHiding)
        {
            //GUARDA LA POSICIÓN Y ROTACIÓN ORIGINALES DEL OBJETO QUE LA CÁMARA ESTÁ SIGUIENDO
            _ve3OriginalPosition = gamCameraFollowTarget.transform.position;
            _quaOriginalRotation = gamCameraFollowTarget.transform.rotation;
            //DESACTIVA EL RUIDO DE LA CÁMARA VIRTUAL
            _cbmVirtualCameraNoise.m_AmplitudeGain = 0.05f;
            //MUEVE Y ROTA EL OBJETO QUE LA CÁMARA DE CINEMACHINE ESTÁ SIGUIENDO AL PUNTO DE VISTA DENTRO DEL CASILLERO
            gamCameraFollowTarget.transform.SetPositionAndRotation(ptraHideSpot.transform.position, ptraHideSpot.transform.rotation);
            controladorJuego.ActivarTemporizador();
        }
        //DE LO CONTRARIO, LE DICE AL JUGADOR QUE DEJE DE ESCONDERSE
        else
        {
            //MUEVE Y ROTA EL OBJETO QUE LA CÁMARA DE CINEMACHINE ESTÁ SIGUIENDO DE VUELTA A SU POSICIÓN Y ROTACIÓN ORIGINAL
            gamCameraFollowTarget.transform.SetPositionAndRotation(_ve3OriginalPosition, _quaOriginalRotation);
            //DESACTIVA EL RUIDO DE LA CÁMARA VIRTUAL
            _cbmVirtualCameraNoise.m_AmplitudeGain = 0.5f;
            controladorJuego.DesactivarTemporizador();
        }
    }
}