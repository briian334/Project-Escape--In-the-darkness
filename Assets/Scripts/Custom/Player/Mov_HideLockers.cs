using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mov_HideLockers : MonoBehaviour
{
    public GameObject player;
    private PlayerInput PlayerInput;
    public GameObject hideSpot; // El punto de vista dentro del casillero
    public bool isHiding = false;
    public Ray rayDetectHigh; //RAYO QUE DETECTA OBJETOS ESCALABLES
    public Camera camVirtualCamera;
    // Distancia dentro de la cual el jugador puede esconderse
    public LayerMask layInteractable;
    [SerializeField] float numMaxDistanceRay = 0.5f;
    private Quaternion originalRotation;
    private Vector3 originalPosition;
    public GameObject cameraFollowTarget; // El objeto que la cámara de Cinemachine está siguiendo
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise; // El componente de ruido de la cámara virtual
    public CinemachineVirtualCamera virtualCamera; // La cámara virtual de Cinemachine
    private Transform _gamPointRLocker;

    private void Start()
    {
        PlayerInput = player.GetComponent<PlayerInput>();
        virtualCameraNoise = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }
    void Update()
    {
        //SE DA REFERENCIA DE POSICION AL RAYO (EL ORIGEN SALE DESDE EL GAMEOBJECT VACIO)
        rayDetectHigh = new(camVirtualCamera.transform.position, camVirtualCamera.transform.forward);
        Debug.DrawRay(rayDetectHigh.origin, rayDetectHigh.direction * numMaxDistanceRay, Color.white);

        // Si el jugador está lo suficientemente cerca y presiona la tecla de escondite
        if (Physics.Raycast(rayDetectHigh.origin, rayDetectHigh.direction, out RaycastHit ryhPointCollison, numMaxDistanceRay, layInteractable))
        {
            //Layer Interact
            if (PlayerInput.actions["Interact"].WasPressedThisFrame())
            {
                string strNameTag = ryhPointCollison.collider.tag;
                switch (strNameTag)
                {
                    //Tags
                    case "Locker":
                        _gamPointRLocker = ryhPointCollison.transform.Find("Inside");
                        Debug.DrawRay(rayDetectHigh.origin, rayDetectHigh.direction * numMaxDistanceRay, Color.red);
                        FnHiding(_gamPointRLocker);
                        break;
                    default: return;
                }
            }
            return;
        }
    }
    public void FnHiding(Transform hideSpot)
    {
        // Alternar entre esconderse y no esconderse
        isHiding = !isHiding;

        // Si el jugador se está escondiendo, le dice al jugador que se esconda y le da la ubicación del punto de vista dentro de este casillero
        if (isHiding)
        {
            // Guarda la posición y rotación originales del objeto que la cámara está siguiendo
            originalPosition = cameraFollowTarget.transform.position;
            originalRotation = cameraFollowTarget.transform.rotation;
            // Desactiva el ruido de la cámara virtual
            virtualCameraNoise.m_AmplitudeGain = 0.05f;
            // Mueve y rota el objeto que la cámara de Cinemachine está siguiendo al punto de vista dentro del casillero
            cameraFollowTarget.transform.SetPositionAndRotation(hideSpot.transform.position, hideSpot.transform.rotation);
        }
        // De lo contrario, le dice al jugador que deje de esconderse
        else
        {
            // Mueve y rota el objeto que la cámara de Cinemachine está siguiendo de vuelta a su posición y rotación original
            cameraFollowTarget.transform.SetPositionAndRotation(originalPosition, originalRotation);
            // Desactiva el ruido de la cámara virtual
            virtualCameraNoise.m_AmplitudeGain = 0.5f;
        }
    }
}