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
        // Guarda la posici�n inicial de la c�mara
        initialPosition = transform.localPosition;

        // Crea una secuencia para el efecto de "head bobbing"
        bobbingSequence = DOTween.Sequence();

        // A�ade un movimiento hacia la derecha
        bobbingSequence.Append(transform.DOLocalMoveX(initialPosition.x + bobbingAmount, bobbingSpeed));

        // A�ade un movimiento hacia la izquierda
        bobbingSequence.Append(transform.DOLocalMoveX(initialPosition.x - bobbingAmount, bobbingSpeed));

        // Configura la secuencia para repetirse indefinidamente
        bobbingSequence.SetLoops(-1, LoopType.Yoyo);

        // Pausa la secuencia al inicio
        bobbingSequence.Pause();
    }

    void Update()
    {
        // Comprueba si el personaje est� en movimiento
        if (characterController.velocity.magnitude > 0)
        {
            // Si el personaje est� en movimiento y la secuencia no est� jugando, inicia la secuencia
            if (!bobbingSequence.IsPlaying())
            {
                bobbingSequence.Play();
            }
        }
        else
        {
            // Si el personaje no est� en movimiento y la secuencia est� jugando, pausa la secuencia
            if (bobbingSequence.IsPlaying())
            {
                bobbingSequence.Pause();
            }
        }
    }
}
