using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mov_ClimbBoxes : MonoBehaviour
{
    #region Variables:
    public CharacterController chcCharacterController; //INSTANCIA AL CONTROLADOR DE FISICAS DEL JUGADOR
    public PlayerInput pyiPlayerInput; //INSTANCIA AL SISTEMA DE ENTRADA (CONTROLES DE MOVIMIENTO)   
    public Ray rayDetectHigh; //RAYO QUE DETECTA OBJETOS ESCALABLES
    private Transform _traReferencePoint; //PUNTO DE REFERENCIA PARA LA INTERPOLACION
    private Transform _traOriginalScale; //ESCALA ORIGINAL DEL JUGADOR
    public IEnumerator ienClimbBoxes; //IENUMERATOR QUE HACE REFERNCIA A UNA CORRUTINA
    public IEnumerator ienClimbConduct; //IENUMERATOR QUE HACE REFERNCIA A UNA CORRUTINA
    public bool booIsClimbing; //VARIABLE BOOLEANA QUE INDICA SI EL JUGADOR ESTA ESCALANDO

    [Header("Object Detection")]
    [SerializeField] LayerMask layObjectUpper; //OBJETO DE TIPO "MASCARA" SE ENCARGA DE ESPECIFICAR QUE CAPA PODRA SER ESCALADA    
    [SerializeField] float numMaxDistanceRay = 1f; //DISTANCIA DEL RAYO QUE DETECTA OBJETOS ESCALABLES        
    [SerializeField] private float _numAnimationTime = 2f; //DURACION DE ANIMACION ESCALAR
    [SerializeField] float numFactorScale = 0.5f; //ESCALA DEL JUGADOR (TAMANO)
   
    #endregion
    private void Start()
    {
        //SE INICIALIZAN LOS OBJETOS DE REFERENCIA AL JUGADOR (EN FUNCION DEL PADRE)
        chcCharacterController = GetComponentInParent<CharacterController>();
        pyiPlayerInput = GetComponentInParent<PlayerInput>();
        _traOriginalScale = GetComponentInParent<Transform>();

    }
    private void FixedUpdate()
    {
        //SE DA REFERENCIA DE POSICION AL RAYO (EL ORIGEN SALE DESDE EL GAMEOBJECT VACIO)
        rayDetectHigh = new(transform.position, transform.forward);
        Debug.DrawRay(rayDetectHigh.origin, rayDetectHigh.direction * numMaxDistanceRay, Color.white);
        //SE CREA UNA CONDICION PARA SABER SI ES QUE EL RAYO COLISIONA CON UN OBJETO ESCALABLE (MEDIANTE LA CAPA)  
        if (Physics.Raycast(rayDetectHigh.origin, rayDetectHigh.direction, out RaycastHit ryhPointCollison, numMaxDistanceRay, layObjectUpper))
        {            
           
            //EN CASO DE SER CIERTA, SE COMPRUEBA SI EL JUGADOR PRESIONA LA TECLA ESPERADA PARA REALIZAR DICHA ACCION
            if (pyiPlayerInput.actions["Climb"].IsPressed() && booIsClimbing == false)
            {
                //SE OBTIENE LA ETIQUETA DEL OBJETO DETECTADO
                string strHitCollisionTag = ryhPointCollison.transform.tag;
                //SE COMPARA EN UN SWITCH PARA EJECUTAR UNA CORRUTINA
                switch (strHitCollisionTag)
                {
                    case "Box":
                        //SE OBTIENE EL OBJETO REFERENCIA
                        _traReferencePoint = ryhPointCollison.transform.Find("PointReference");
                        //SE OBTIENE EL RIGIDBODY DE LA CAJA
                        Rigidbody rgbBox = ryhPointCollison.transform.GetComponent<Rigidbody>();
                        if (rgbBox != null)
                        {
                            //SE REALIZA LA INTERPOLACION ENTRE EL JUGADOR Y EL OBJETO ESCALABLE
                            ienClimbBoxes = CrtClimbBoxes(rgbBox, _traReferencePoint);
                            //SE EJECUTA LA CORRUTINA
                            StartCoroutine(ienClimbBoxes);
                            //SE PINTA EL RAYO EN DESARROLLO (PARA PRUEBAS, DESPUES ELIMINAR)
                            Debug.DrawRay(rayDetectHigh.origin, rayDetectHigh.direction * numMaxDistanceRay, Color.blue);
                        }
                        return;                  
                    case "Conduct":
                        //SE OBTIENE EL OBJETO REFERENCIA
                        _traReferencePoint = ryhPointCollison.transform.Find("PointReference");
                        //SE REALIZA LA INTERPOLACION ENTRE EL JUGADOR Y EL OBJETO ESCALABLE
                        ienClimbConduct = CrtClimbConduct(_traReferencePoint);
                        //SE EJECUTA LA CORRUTINA
                        StartCoroutine(ienClimbConduct);
                        //SE PINTA EL RAYO EN DESARROLLO (PARA PRUEBAS, DESPUES ELIMINAR)
                        Debug.DrawRay(rayDetectHigh.origin, rayDetectHigh.direction * numMaxDistanceRay, Color.blue);
                        break;
                    default:
                        Debug.Log("Tag no existente.");
                        return;
                }
                booIsClimbing = true;
            }
        }
        //SI EL ESTADO "ESCALANDO" ESTA INACTIVO, Y LA CORRUTINA NO ES NULA, PARAMOS CORRUTINAS ACTIVAS
        if (ienClimbBoxes != null && booIsClimbing == false)        
            StopCoroutine(ienClimbBoxes);     
        if (ienClimbConduct != null && booIsClimbing == false)       
            StopCoroutine(ienClimbConduct);        
    }
    IEnumerator CrtClimbBoxes(Rigidbody prgbBox, Transform ptraReferencePoint)
    {
        //SE ESTABLECE EL RIGIDBODY COMO CINEMÁTICO PARA EVITAR INTERFERENCIAS FÍSICAS DURANTE LA ESCALADA
        prgbBox.isKinematic = true;
        //SE CREA UNA SECUENCIA DOTWEEN PARA LAS ANIMACIONES DE MOVIMIENTO
        Sequence climbSequence = DOTween.Sequence();
        //TWEEN PARA MOVER AL JUGADOR VERTICALMENTE HACIA LA POSICIÓN DE REFERENCIA
        Tween firstStep = chcCharacterController.transform.DOMoveY(ptraReferencePoint.transform.position.y, _numAnimationTime)
            .SetEase(Ease.InOutQuad);
        //TWEEN PARA MOVER AL JUGADOR A LA POSICIÓN DE REFERENCIA
        Tween secondStep = chcCharacterController.transform.DOMove(ptraReferencePoint.position, _numAnimationTime)
            .SetEase(Ease.InOutQuad);
        //AGREGAR LOS TWEENS A LA SECUENCIA EN ORDEN
        climbSequence.Append(firstStep);
        climbSequence.Append(secondStep);
        //ESPERAR A QUE SE COMPLETE LA SECUENCIA
        yield return climbSequence.WaitForCompletion();
        //SE RESTABLECE EL RIGIDBODY A SU ESTADO ORIGINAL PARA REANUDAR LAS INTERACCIONES FÍSICAS
        prgbBox.isKinematic = false;
        //SE DESACTIVA EL ESTADO "ESCALANDO"
        booIsClimbing = false;
    }
    IEnumerator CrtClimbConduct(Transform ptraReferencePoint)
    {
        //SE CREA UNA SECUENCIA DOTWEEN
        Sequence seqClimbSequence = DOTween.Sequence();
        //TWEEN PARA MOVER AL JUGADOR HACIA ARRIBA
        Tween tweFirstStepMove = chcCharacterController.transform.DOMoveY(ptraReferencePoint.transform.position.y, _numAnimationTime)
            .SetEase(Ease.InOutQuad);
        //AGREGAR AMBOS TWEENS A LA SECUENCIA
        seqClimbSequence.Append(tweFirstStepMove);
        //TWEEN PARA ESCALAR AL JUGADOR HACIA ABAJO AL MISMO TIEMPO
        Tween twePlayerScaleDown = chcCharacterController.transform.DOScale(_traOriginalScale.localScale * numFactorScale, _numAnimationTime)
            .SetEase(Ease.InOutQuad);
        //TWEEN PARA MOVER AL JUGADOR HACIA ADELANTE DESPUÉS DE BAJAR
        Tween tweeSecondStepMove = chcCharacterController.transform.DOMove(ptraReferencePoint.position, _numAnimationTime)
            .SetEase(Ease.InOutQuad);
        //AGREGAR EL TWEEN DE ESCALADO A LA SECUENCIA
        seqClimbSequence.Append(twePlayerScaleDown);
        //JOIN PARA QUE SCALEDOWN SE EJECUTE AL MISMO TIEMPO QUE FIRSTSTEPMOVE       
        seqClimbSequence.Join(tweeSecondStepMove);
        //ESPERAR A QUE SE COMPLETE LA SECUENCIA
        yield return seqClimbSequence.WaitForCompletion();
        //SE DESACTIVA EL ESTADO "ESCALANDO"
        booIsClimbing = false;
    }
}