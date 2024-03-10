using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ClaustrophobiaEffect : MonoBehaviour
{
    public bool isClaustrophobic = false;
    public float darknessLevel = 0f;
    public float shakeIntensity = 0f;
    public float shakeDecayRate = 0.1f;
    public float darknessDecayRate = 0.02f;
    public float claustrophobiaDuration = 10f;

    private float elapsedTime = 0f;

    private void Update()
    {
        if (isClaustrophobic)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime < claustrophobiaDuration)
            {
                float t = elapsedTime / claustrophobiaDuration;
                shakeIntensity = Mathf.Lerp(1f, 0f, t);
                darknessLevel = Mathf.Lerp(1f, 0f, t * 0.5f);
            }
            else
            {
                StopClaustrophobiaEffect();
            }

            // Apply camera shake
            if (shakeIntensity > 0)
            {
                transform.position += Random.insideUnitSphere * shakeIntensity;
            }

            // Apply darkness
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 45f + (1f - darknessLevel) * 30f, Time.deltaTime * 5f);
        }
    }

    public void StartClaustrophobiaEffect()
    {
        isClaustrophobic = true;
        elapsedTime = 0f;
        shakeIntensity = 1f;
        darknessLevel = 1f;
    }

    public void StopClaustrophobiaEffect()
    {
        isClaustrophobic = false;
        shakeIntensity = 0f;
        darknessLevel = 0f;
    }
}
