using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Int_ObjectSelected : MonoBehaviour
{
    #region Global Variables
    public static Int_ObjectSelected Instance { get; private set; }
    private PlayerInput _pyiPlayerInput;
    public Ray rayDetectInteractable;
    public LayerMask layLayerInteractable;
    [SerializeField] float numMaxDistanceRay = 0.5f;
    public Camera camMainCamera;
    private Transform _traTransformCollision;
    public GameObject gamCameraFollowTarget;
    private CinemachineBasicMultiChannelPerlin _cbmVirtualCameraNoise;
    public CinemachineVirtualCamera cvcVirtualCamera;
    private Transform _traHighlightedObject;
    private RaycastHit _ryhPointCollison;
    #endregion
    #region Hidding Variables   
    public GameObject gamHideSpot;
    public bool booIsHiding = false;
    private Quaternion _quaOriginalRotation;
    private Vector3 _ve3OriginalPosition;
    [SerializeField] private ControladorJuego controladorJuego;
    [SerializeField] private Fade fade;


    // Variables para el efecto de claustrofobia
    [SerializeField] private float maxBreathIntensity = 1f; // Intensidad máxima de la respiración
    //[SerializeField] private float breathIncreaseRate = 0.3f; // Tasa de aumento de la intensidad de la respiración
    [SerializeField] private float maxBreathRepeatInterval = 2f; // Intervalo máximo entre respiraciones
    private float currentBreathIntensity = 0f; // Intensidad actual de la respiración
    private float breathTimer = 0f; // Temporizador para controlar la duración del aumento de la intensidad de la respiración
    private float breathRepeatTimer = 0f; // Temporizador para controlar la repetición de la respiración
    private bool isBreathing = false; // Indicador de si el personaje está respirando
    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _pyiPlayerInput = GetComponent<PlayerInput>();
        _cbmVirtualCameraNoise = cvcVirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }

    void Update()
    {
        rayDetectInteractable = new(camMainCamera.transform.position, camMainCamera.transform.forward);
        Debug.DrawRay(rayDetectInteractable.origin, rayDetectInteractable.direction * numMaxDistanceRay, Color.white);

        if (Physics.Raycast(rayDetectInteractable.origin, rayDetectInteractable.direction, out _ryhPointCollison, numMaxDistanceRay, layLayerInteractable))
        {
            if (_ryhPointCollison.collider.gameObject.layer != LayerMask.NameToLayer("Interactable"))
            {
                return;
            }

            Transform traHitObject = _ryhPointCollison.collider.transform;
            if (traHitObject.GetComponentInParent<Outline>())
            {

            }
            else
            {
                Debug.Log("No tiene script outline...");
                return;
            }

            if (!booIsHiding)
            {
                traHitObject.GetComponentInParent<Outline>().enabled = true;
            }

            if (_traHighlightedObject != null && _traHighlightedObject != traHitObject)
            {
                _traHighlightedObject.GetComponentInParent<Outline>().enabled = false;
            }

            _traHighlightedObject = traHitObject;

            if (_pyiPlayerInput.actions["Interact"].WasPressedThisFrame())
            {
                _ryhPointCollison.collider.transform.GetComponentInParent<Outline>().enabled = false;
                string strNameTag = _ryhPointCollison.collider.tag;

                switch (strNameTag)
                {
                    case "Locker":
                        try
                        {
                            _traTransformCollision = _ryhPointCollison.transform.Find("Inside");
                            FnHiding(_traTransformCollision);
                        }
                        catch (System.Exception)
                        {
                            return;
                        }
                        break;
                    case "Puzzle":
                        if (!EventSystem.current.IsPointerOverGameObject())
                        {
                            _ryhPointCollison.transform.gameObject.SendMessage("ActivateObject", 0, SendMessageOptions.DontRequireReceiver);
                        }
                        break;
                    default:
                        return;
                }
            }
        }
        else
        {
            if (_traHighlightedObject != null)
            {
                _traHighlightedObject.GetComponentInParent<Outline>().enabled = false;
                _traHighlightedObject = null;
            }
        }

        // Aplicar efecto de claustrofobia si el jugador está dentro del locker
        if (booIsHiding)
        {
            IncreaseBreathIntensity(); // Aumentar la intensidad de la respiración
            _cbmVirtualCameraNoise.m_AmplitudeGain = currentBreathIntensity; // Aplicar el ruido de la cámara según la intensidad de la respiración

            if (!isBreathing && breathRepeatTimer >= maxBreathRepeatInterval)
            {
                StartBreathing();
            }
        }

        if (isBreathing)
        {
            breathRepeatTimer += Time.deltaTime;
            if (breathRepeatTimer >= maxBreathRepeatInterval)
            {
                breathRepeatTimer = 0f;
                isBreathing = false;
            }
        }
    }

    public void FnHiding(Transform ptraHideSpot)
    {
        booIsHiding = !booIsHiding;

        if (booIsHiding)
        {
            _ve3OriginalPosition = gamCameraFollowTarget.transform.position;
            _quaOriginalRotation = gamCameraFollowTarget.transform.rotation;
            _cbmVirtualCameraNoise.m_AmplitudeGain = 0.05f;
            gamCameraFollowTarget.transform.SetPositionAndRotation(ptraHideSpot.transform.position, ptraHideSpot.transform.rotation);
            controladorJuego.ActivarTemporizador();
            breathTimer = 0f; // Reiniciar el temporizador de la respiración al entrar al casillero
            fade.Blink2();
            
        }
        else
        {
            gamCameraFollowTarget.transform.SetPositionAndRotation(_ve3OriginalPosition, _quaOriginalRotation);
            _cbmVirtualCameraNoise.m_AmplitudeGain = 0.5f;
            controladorJuego.DesactivarTemporizador();
            currentBreathIntensity = 0f; // Reiniciar la intensidad de la respiración al salir del locker
            fade.FadeOut2();

        }
    }

    private void IncreaseBreathIntensity()
    {
        // Incrementar la intensidad de la respiración gradualmente hasta alcanzar la intensidad máxima
        if (currentBreathIntensity < maxBreathIntensity)
        {
            breathTimer += Time.deltaTime;
            currentBreathIntensity = Mathf.Lerp(0f, maxBreathIntensity, breathTimer / 2f); // Ajusta el 5f al tiempo deseado para el aumento
        }
        else
        {
            currentBreathIntensity = maxBreathIntensity; // Mantener la intensidad en el máximo después de alcanzarlo
        }
    }

    private void StartBreathing()
    {
        isBreathing = true;
        breathRepeatTimer = 0f; // Reiniciar el temporizador de repetición de la respiración
        // Aquí puedes reproducir un sonido de respiración o realizar otras acciones relacionadas con la respiración
    }
}
