using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mov_HideLockers : MonoBehaviour
{
    #region Variables
    private PlayerInput _pyiPlayerInput; //DEFINE LOS CONTROLES DEL JUGADOR
    public GameObject gamHideSpot; //EL PUNTO DE VISTA DENTRO DEL CASILLERO (ASIGNAR EN INSPECTOR)
    public bool booIsHiding = false; //ESTADO ESCONDIDO
    public Ray rayDetectInteractable; //RAYO QUE DETECTA OBJETOS INTERACTUABLES   
    public LayerMask layLayerInteractable; //CAPA INTERACTUABLE DEL OBJETO (ASIGNAR EN INSPECTOR)
    [SerializeField] float numMaxDistanceRay = 0.5f; //DISTANCIA DEL RAYO PARA SELECCIONAR OBJETOS
    private Quaternion _quaOriginalRotation; //ROTACION ORIGINAL DE LA CAMARA ANTES DE ESCONDERSE
    private Vector3 _ve3OriginalPosition; //POSICION ORIGINAL DEL JUGADOR ANTES DE ESCONDERSE
    public GameObject gamCameraFollowTarget; //OBJETO QUE SIGUE LA VIRTUAL CAMERA DEL JUGADOR
    private CinemachineBasicMultiChannelPerlin _cbmVirtualCameraNoise; //PROPIEDAD QUE HACE FLOTAR LA CAMARA SIMULANDO RESPIRACION
    public CinemachineVirtualCamera cvcVirtualCamera; //CAMARA VIRTUAL DEL JUGADOR
    public Camera camMainCamera; //CAMARA PRINCIPAL PARA EMITIR EL RAYO DESDE ESA POSICION
    private Transform _TraPointRLocker; //POSICION DEL OBJETO "INSIDE" DEL CASILLERO
    #endregion
    private void Start()
    {
        //SE INICIALIZAN LAS VARIABLES
        _pyiPlayerInput = GetComponent<PlayerInput>();
        _cbmVirtualCameraNoise = cvcVirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }
    void Update()
    {
        rayDetectInteractable = new(camMainCamera.transform.position, camMainCamera.transform.forward);
        Debug.DrawRay(rayDetectInteractable.origin, rayDetectInteractable.direction * numMaxDistanceRay, Color.white);

        // Si el jugador está lo suficientemente cerca y presiona la tecla de escondite
        if (Physics.Raycast(rayDetectInteractable.origin, rayDetectInteractable.direction, out RaycastHit ryhPointCollison, numMaxDistanceRay, layLayerInteractable))
        {

            if (_pyiPlayerInput.actions["Interact"].WasPressedThisFrame())
            {
                string strNameTag = ryhPointCollison.collider.tag;
                switch (strNameTag)
                {
                    //Tags
                    case "Locker":
                        _TraPointRLocker = ryhPointCollison.transform.Find("Inside");
                        FnHiding(_TraPointRLocker);
                        break;
                    default: return;
                }
            }
            return;
        }
    }
    public void FnHiding(Transform ptraHideSpot)
    {
        // Alternar entre esconderse y no esconderse
        booIsHiding = !booIsHiding;

        // Si el jugador se está escondiendo, le dice al jugador que se esconda y le da la ubicación del punto de vista dentro de este casillero
        if (booIsHiding)
        {
            // Guarda la posición y rotación originales del objeto que la cámara está siguiendo
            _ve3OriginalPosition = gamCameraFollowTarget.transform.position;
            _quaOriginalRotation = gamCameraFollowTarget.transform.rotation;
            // Desactiva el ruido de la cámara virtual
            _cbmVirtualCameraNoise.m_AmplitudeGain = 0.05f;
            // Mueve y rota el objeto que la cámara de Cinemachine está siguiendo al punto de vista dentro del casillero
            gamCameraFollowTarget.transform.SetPositionAndRotation(ptraHideSpot.transform.position, ptraHideSpot.transform.rotation);
        }
        // De lo contrario, le dice al jugador que deje de esconderse
        else
        {
            // Mueve y rota el objeto que la cámara de Cinemachine está siguiendo de vuelta a su posición y rotación original
            gamCameraFollowTarget.transform.SetPositionAndRotation(_ve3OriginalPosition, _quaOriginalRotation);
            // Desactiva el ruido de la cámara virtual
            _cbmVirtualCameraNoise.m_AmplitudeGain = 0.5f;
        }
    }
}