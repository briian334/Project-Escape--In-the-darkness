using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    //VARIABLES PARA MODIFICAR LA VELOCIDAD DE MOVIMIENTO Y ROTACION DE CAMARA
    public float MovementSpeed = 1.0f;
    public float RotationSpeed = 1.0f;

    Animator anim;
    // Referencia a la cámara
    Camera playerCamera;
    private float cameraRotationX = 0f; // Variable para almacenar la rotación acumulada en X
    void Start()
    {
        //ESTO HACE QUE EL CURSOR SE QUEDE BLOQUEADO Y SE ESCONDA A LA HORA DE JUGAR
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        anim = GetComponentInChildren<Animator>();
        playerCamera = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        //VARIABLES PARA EL MOVIMIENTO DEL PERSONAJE
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direccion = new Vector3 (horizontal, 0f, vertical).normalized;
        // Si el jugador se está moviendo
        
        if (direccion.magnitude >= 0.1f)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                // Reproduce la animación "Run"
                anim.SetFloat("Movement", 1, 0.1f, Time.deltaTime);
                transform.Translate(new Vector3(horizontal, 0.0f, vertical) * Time.deltaTime * MovementSpeed * 2);
            }
            else
            {
                //PERMITE MOVER UN OBJETO MEDIANTE VECTORES Y SE USA DELTATIME PARA CONTROLAR LA VELOCIDAD POR FPS
                transform.Translate(new Vector3(horizontal, 0.0f, vertical) * Time.deltaTime * MovementSpeed);
                anim.SetFloat("Movement", 0.5f, 0.1f, Time.deltaTime);
            }
        }
        else
        {
            anim.SetFloat("Movement", 0, 0.1f, Time.deltaTime);
        }
        float rotationY = Input.GetAxis("Mouse X");
        transform.Rotate(new Vector3(0, rotationY * Time.deltaTime * RotationSpeed * 2, 0));

        // Agrega restricciones a la rotación en el eje X para la cámara
        float rotationX = Input.GetAxis("Mouse Y");
        cameraRotationX -= rotationX * Time.deltaTime * RotationSpeed * 2;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90f, 90f); // Restringe la rotación entre -90 y 90 grados

        playerCamera.transform.localRotation = Quaternion.Euler(cameraRotationX, 0f, 0f);
    }
}
