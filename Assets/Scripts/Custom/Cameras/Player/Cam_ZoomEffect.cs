using UnityEngine;
using Cinemachine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class Cam_ZoomEffect : MonoBehaviour
{
    public CinemachineVirtualCamera cvcVirtualCamera; // CAMARA DEL JUGADOR
    public float numZoomTarget = 35f; // EL VALOR OBJETIVO PARA HACER ZOOM
    public float numAnimDuration = 0.8f; // DURACIÓN DE LA ANIMACIÓN EN SEGUNDOS
    public PlayerInput pyiPlayerInput; // MAPA DE CONTROLES DEL JUGADOR
    private float _numInitialFOV; //FOV INICIAL ANTES DE APLICAR EL ZOOM
    private Tweener _tweSequence; // SECUENCIA DEL TIPO DOTWEEN
    public Mov_ClimbBoxes Mov_ClimbBoxes;
    private void Start()
    {
        // GUARDA EL FOV INICIAL PARA PODER VOLVER A ÉL DESPUÉS DEL ZOOM
        _numInitialFOV = cvcVirtualCamera.m_Lens.FieldOfView;
    }

    private void LateUpdate()
    {
        if (!Mov_ClimbBoxes.booIsClimbing)
        {
            // SI EL JUGADOR PRESIONA LA TECLA DE ZOOM, INICIA EL EFECTO DE ZOOM
            if (pyiPlayerInput.actions["Zoom"].IsPressed())
            {
                // VERIFICA SI YA HAY UNA ANIMACIÓN EN CURSO Y LA DETIENE ANTES DE COMENZAR UNA NUEVA
                if (_tweSequence != null && _tweSequence.IsActive()) _tweSequence.Kill();

                // INICIA LA ANIMACIÓN DE ZOOM HACIA EL VALOR OBJETIVO
                _tweSequence = DOTween.To(() => cvcVirtualCamera.m_Lens.FieldOfView, x => cvcVirtualCamera.m_Lens.FieldOfView = x, numZoomTarget, numAnimDuration);
            }

            // SI EL JUGADOR SUELTA LA TECLA DE ZOOM, VUELVE AL FOV INICIAL
            if (!pyiPlayerInput.actions["Zoom"].IsPressed())
            {
                // VERIFICA SI YA HAY UNA ANIMACIÓN EN CURSO Y LA DETIENE ANTES DE COMENZAR UNA NUEVA
                if (_tweSequence != null && _tweSequence.IsActive()) _tweSequence.Kill();

                // ANIMA DE REGRESO AL FOV INICIAL
                _tweSequence = DOTween.To(() => cvcVirtualCamera.m_Lens.FieldOfView, x => cvcVirtualCamera.m_Lens.FieldOfView = x, _numInitialFOV, numAnimDuration);
            }
        }
    }
}