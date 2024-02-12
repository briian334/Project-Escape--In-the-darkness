using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class Mov_Movement : MonoBehaviour
{
    #region Movimiento
    public PlayerInput pyiPlayerInput;
    private CharacterController _chcPlayerController;
    private Vector2 _ve2PlayerPosition;
    private Vector3 _ve3PlayerMovement;
    [SerializeField] float numPlayerSpeed = 3.5f;

    #endregion
    #region Camera
    private Vector2 _ve2CameraPosition;
    private Vector3 _ve3CameraRotation;
    private float _cinemachineTargetPitch;
    private GameObject _gamMainCamera;
    public GameObject _gamCameraTarget;
    [SerializeField] float numRotationSpeed = 15f;
    [SerializeField] float _rotationVelocity;
    [SerializeField] float numBottomClamp = -45.0f;
    [SerializeField] float numTopClamp = 45.0f;
    #endregion
    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return pyiPlayerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
        }
    }
    private void Awake()
    {
        //SE INICIALIZAN LOS CONTROLES DEL JUGADOR
        pyiPlayerInput = GetComponent<PlayerInput>();
        _chcPlayerController = GetComponent<CharacterController>();
        // get a reference to our main camera
        if (_gamMainCamera == null)
        {
            _gamMainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }
    private void Start()
    {
        //SE BLOQUEA Y SE OCULTA EL CURSOR
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void FixedUpdate()
    {
        FnMovement();
    }
    private void LateUpdate()
    {
        FnCameraRotation();
    }
    public void FnMovement()
    {
        //SE INICIALIZAN VECTORES DE POSICION Y MOVIMIENTO PARA EL JUGADOR
        _ve2PlayerPosition = pyiPlayerInput.actions["Move"].ReadValue<Vector2>();
        _ve3PlayerMovement = new(_ve2PlayerPosition.x, 0, _ve2PlayerPosition.y);
        //SE LE PASA EL VECTOR DE POSICION AL CHARACTER CONTROLLER PARA DETERMINAR SI EL JUGADOR SE MUEVE O NO
        _chcPlayerController.Move(numPlayerSpeed * _ve3PlayerMovement.normalized * Time.deltaTime);
    }
    public void FnCameraRotation()
    {
        //Don't multiply mouse input by Time.deltaTime
        float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
        _ve2CameraPosition = pyiPlayerInput.actions["Look"].ReadValue<Vector2>();
        // if there is an input
        if (_ve2CameraPosition.sqrMagnitude >= 0.01f)
        {
            _cinemachineTargetPitch += _ve2CameraPosition.y * numRotationSpeed * deltaTimeMultiplier;
            _rotationVelocity = _ve2CameraPosition.x * numRotationSpeed * deltaTimeMultiplier;

            // clamp our pitch rotation
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, numBottomClamp, numTopClamp);

            // Update Cinemachine camera target pitch
            _gamCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

            // rotate the player left and right
            transform.Rotate(Vector3.up * _rotationVelocity);
        }
    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
