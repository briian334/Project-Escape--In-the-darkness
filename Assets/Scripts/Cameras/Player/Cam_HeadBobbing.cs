using DG.Tweening;
using UnityEngine;

public class HeadBobbing : MonoBehaviour
{
    // Ajusta estos valores para cambiar la cantidad de movimiento y la velocidad del efecto de "head bobbing"
    public float bobbingAmount = 0.1f;
    public float bobbingSpeed = 0.5f;

    // Referencia al controlador del personaje
    public CharacterController characterController;

    private Vector3 initialPosition;
    private Sequence bobbingSequence;

    void Start()
    {
        characterController = GetComponentInParent<CharacterController>();
        // Guarda la posición inicial de la cámara
        initialPosition = transform.localPosition;

        // Crea una secuencia para el efecto de "head bobbing"
        bobbingSequence = DOTween.Sequence();

        // Añade un movimiento hacia la derecha
        bobbingSequence.Append(transform.DOLocalMoveX(initialPosition.x + bobbingAmount, bobbingSpeed));

        // Añade un movimiento hacia la izquierda
        bobbingSequence.Append(transform.DOLocalMoveX(initialPosition.x - bobbingAmount, bobbingSpeed));

        // Configura la secuencia para repetirse indefinidamente
        bobbingSequence.SetLoops(-1, LoopType.Yoyo);

        // Pausa la secuencia al inicio
        bobbingSequence.Pause();
    }

    void Update()
    {
        // Comprueba si el personaje está en movimiento
        if (characterController.velocity.magnitude > 0)
        {
            // Si el personaje está en movimiento y la secuencia no está jugando, inicia la secuencia
            if (!bobbingSequence.IsPlaying())
            {
                bobbingSequence.Play();
            }
        }
        else
        {
            // Si el personaje no está en movimiento y la secuencia está jugando, pausa la secuencia
            if (bobbingSequence.IsPlaying())
            {
                bobbingSequence.Pause();
            }
        }
    }
}
