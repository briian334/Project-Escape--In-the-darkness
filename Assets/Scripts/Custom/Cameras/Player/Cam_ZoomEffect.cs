using UnityEngine;
using Cinemachine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class Cam_ZoomEffect:MonoBehaviour
    {
    public CinemachineVirtualCamera vcam;
    public float zoomTarget = 35f; // el valor objetivo para hacer zoom
    public float duration = 0.8f; // duración de la animación en segundos
    public PlayerInput playerInput; // la tecla para activar el zoom

    private float initialFOV;

    private Tweener tweener;

    private void Start()
    {
        // Guarda el FOV inicial para poder volver a él después del zoom
        initialFOV = vcam.m_Lens.FieldOfView;
    }

    private void LateUpdate()
    {       
            // Si el jugador presiona la tecla de zoom, inicia el efecto de zoom
            if (playerInput.actions["Zoom"].IsPressed())
            {
                if (tweener != null && tweener.IsActive()) tweener.Kill();
                tweener = DOTween.To(() => vcam.m_Lens.FieldOfView, x => vcam.m_Lens.FieldOfView = x, zoomTarget, duration);
            }

        // Si el jugador suelta la tecla de zoom, vuelve al FOV inicial
        if (!playerInput.actions["Zoom"].IsPressed())
        {
            if (tweener != null && tweener.IsActive()) tweener.Kill();
            tweener = DOTween.To(() => vcam.m_Lens.FieldOfView, x => vcam.m_Lens.FieldOfView = x, initialFOV, duration);
        }
    }
}