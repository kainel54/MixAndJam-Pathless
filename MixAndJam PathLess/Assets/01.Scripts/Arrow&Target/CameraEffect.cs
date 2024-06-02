using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffect : MonoBehaviour
{
    private Material materialClone;
    private Material originalMaterial;

    private MovementInput movement;
    [SerializeField] private Cyan.Blit blit;
    [SerializeField] private float lerpTime = 1;

    private void Awake()
    {
        movement = FindObjectOfType<MovementInput>();

        if (blit == null)
            return;

        originalMaterial = blit.settings.blitMaterial;
        materialClone = new Material(blit.settings.blitMaterial);
        blit.settings.blitMaterial = materialClone;
    }

    private void Update()
    {
        if(blit == null) return;

        float alphaValue = movement.isRunning ? (movement.isBoosting ? 0.02f : 0.01f) : 0;

        blit.settings.blitMaterial.SetFloat("_Alpha",Mathf.Lerp(blit.settings.blitMaterial.GetFloat("Alpha"),alphaValue,lerpTime*Time.deltaTime));
    }

    private void OnDestroy()
    {
        if(blit == null ) return;

        blit.settings.blitMaterial = originalMaterial;
    }
}
