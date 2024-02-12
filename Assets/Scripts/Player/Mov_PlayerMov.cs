using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class Mov_PlayerMov : MonoBehaviour
{
    //VARIABLES QUE CAPTURAN LOS MOVIMIENTOS DEL JUGADOR, VELOCIDAD, GRAVEDAD Y ROTACION DE CAMARA)
    public PlayerInput pyiPlayerInput;
    private CharacterController _chcPlayerController;
    public CinemachineVirtualCamera cvcVirtualCamera;
    public CinemachineComponentBase ccbAimPOVComponent;
    public Camera camMainCamera;
    public Animator aniPlayerAnimator;
    private Vector3 _ve3CameraForward;
    private Vector3 _ve3CameraRight;
    private Vector2 _ve2PlayerPosition;
    private Vector3 _ve3PlayerMovement;
    private Vector3 _ve3CameraRotation;
    public bool booIsPlayerCrouch = false;
    private bool booIsPlayerRunning;    
    private float numPlayerCurrentSpeed;
    [SerializeField] float numPlayerSprintSpeed = 6f;
    [SerializeField] float numPlayerGravity = 9.81f;
    [SerializeField] float numPlayerNormSpeed = 4f;
    public float crouchHeight = 1.0f;
    public float crouchSpeed = 2f;
    private float originalHeight;
    Vector3 Offset;
    [SerializeField] LayerMask roof;
    float numCamRotationSpeed = 1f;
    private float rayDistance = 1.0f;

    void Awake()
    {
        //SE INICIALIZAN LOS CONTROLES DEL JUGADOR
        pyiPlayerInput = GetComponent<PlayerInput>();
        _chcPlayerController = GetComponent<CharacterController>();        
        //SE BLOQUEA Y SE OCULTA EL CURSOR
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //SE BUSCA LA CAMARA VIRTUAL DE CINEMACHINE
        cvcVirtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        if (cvcVirtualCamera != null)
        {
            //SI LA ENCUENTRA, SE HACE REFERENCIA AL COMPONENTE AIM DE ESA CAMARA (PARA OBTENER EL VALOR DE LA SENSIBILIDAD)        
           ccbAimPOVComponent = cvcVirtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Aim);            
        }
        else
        {
            Debug.LogError("No se pudo encontrar la CinemachineVirtualCamera por nombre.");
        }
    }
    private void Start()
    {
        camMainCamera = CinemachineCore.Instance.GetActiveBrain(0).OutputCamera;
        originalHeight = _chcPlayerController.height;
    }
    void FixedUpdate()
    {
        //SE OBTIENE EL VALOR DE LA SENSIBILIDAD EN X PARA MOVER LA CAMARA Y ROTAR EN SU MISMO EJE
        if (ccbAimPOVComponent is CinemachinePOV)
        {
            CinemachinePOV pov = (CinemachinePOV)ccbAimPOVComponent;
            numCamRotationSpeed = pov.m_HorizontalAxis.m_MaxSpeed * 500;
        }
        //SE INICIALIZAN VECTORES DE POSICION Y MOVIMIENTO PARA EL JUGADOR
        _ve2PlayerPosition = pyiPlayerInput.actions["Move"].ReadValue<Vector2>();
        _ve3PlayerMovement = new(_ve2PlayerPosition.x, 0, _ve2PlayerPosition.y);
        Vector2 vector = pyiPlayerInput.actions["Look"].ReadValue<Vector2>();
        _ve3CameraRotation = new(0,vector.x,0);
        //SE OBTIENEN LOS VECTORES DE DIRECCION DE LA CAMARA
        _ve3CameraForward = camMainCamera.transform.forward;
        _ve3CameraRight = camMainCamera.transform.right;
        
        //PROYECTA LOS VECTORES DE DIRECCIÓN EN EL PLANO HORIZONTAL (Y = 0)
        _ve3CameraForward.y = 0f;
        _ve3CameraRight.y = 0f;
        _ve3CameraForward.Normalize();
        _ve3CameraRight.Normalize();

        //VECTOR DE DIRECCION EN EL ESPACIO GLOBAL DONDE EL JUGADOR SE MUEVE
        Vector3 ve3DesiredMov = _ve3CameraForward * _ve2PlayerPosition.y + _ve3CameraRight * _ve2PlayerPosition.x;

        //SE LE PASA EL VECTOR DE POSICION AL CHARACTER CONTROLLER PARA DETERMINAR SI EL JUGADOR SE MUEVE O NO
        _chcPlayerController.Move(numPlayerCurrentSpeed * ve3DesiredMov.normalized * Time.deltaTime);
        
        //SE COMPRUEBA SI EL JUGADOR ESTA CORRIENDO
        booIsPlayerRunning = pyiPlayerInput.actions["Sprint"].IsPressed();
        if (booIsPlayerRunning)
        {
            if (booIsPlayerCrouch)
            {
                //SI ESTA AGACHADO LA VELOCIDAD DISMINUYE
                numPlayerCurrentSpeed = crouchSpeed;
            }
            else
            {
                //SI ESTA CORRIENDO SE CAMBIA LA VELOCIDAD ACTUAL POR LA DE CORRIENDO
                numPlayerCurrentSpeed = numPlayerSprintSpeed;
            }
            
        }
        else if(booIsPlayerCrouch)
        {
            //SI ESTA AGACHADO LA VELOCIDAD DISMINUYE
            numPlayerCurrentSpeed = crouchSpeed;
        }
        else
        {
            //SI NO, SE MANTIENE LA VELOCIDAD ESTANDAR
            numPlayerCurrentSpeed = numPlayerNormSpeed;
        }
        

        //SI EL PERSONAJE SE ESTÁ MOVIENDO, ROTA EL PERSONAJE PARA QUE MIRE EN LA DIRECCIÓN DE LA CAMARA
        if (_ve3CameraRotation != Vector3.zero)
        {
            Quaternion quaCameraRotation = Quaternion.Euler(0f, camMainCamera.transform.eulerAngles.y, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, quaCameraRotation, numCamRotationSpeed * Time.deltaTime);
            Debug.Log("MIRANDO HACIA LOS LADOS");
        }
        float rayDistance = 10f;
        RaycastHit hit;
        Ray rayPlayerHead = new Ray(transform.position, transform.up);
        booIsPlayerCrouch = pyiPlayerInput.actions["Crouch"].IsPressed();
        if (booIsPlayerCrouch)
        {
            Crouch();
        }
        else
        {
            if (Physics.Raycast(rayPlayerHead, out hit, rayDistance, roof))
            {
                Debug.DrawRay(rayPlayerHead.origin, rayPlayerHead.direction * rayDistance, Color.red);
                Debug.Log("ARRIBA HAY ALGO");
                Crouch();
            }
            else
            {
                StandUp();
            }
        }
    }

    public void FnSetAnimation()
    {
        aniPlayerAnimator.SetFloat("X",_ve2PlayerPosition.x);
        aniPlayerAnimator.SetFloat("Y",_ve2PlayerPosition.y);
    }
    void Crouch()
    {        
        float targetHeight = booIsPlayerCrouch ? crouchHeight : originalHeight;
        _chcPlayerController.height = Mathf.Lerp(_chcPlayerController.height, targetHeight, 100f * Time.deltaTime);
        transform.localScale = new(crouchHeight, crouchHeight, crouchHeight);
    }

    void StandUp()
    {        
        float targetHeight = booIsPlayerCrouch ? crouchHeight : originalHeight;    
        _chcPlayerController.height = Mathf.Lerp(_chcPlayerController.height, targetHeight, 100f * Time.deltaTime);
        transform.localScale = new(originalHeight, originalHeight, originalHeight);
    }
}