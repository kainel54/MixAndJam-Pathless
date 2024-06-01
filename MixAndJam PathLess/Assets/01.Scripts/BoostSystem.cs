using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BoostSystem : MonoBehaviour
{
    private bool hasAvailableBoost;
    private MovementInput movement;
    private ArrowSystem arrowSystem;

    [Header("Boost Values")]
    public float boostAmount;

    [Header("Parameters")]
    [SerializeField] float boostDrainSpeed = .1f;
    [SerializeField] float boostGainAmount = .3f;

    [Header("Visuals")]
    [SerializeField] Renderer boostMesh;

    [Header("UI")]
    [SerializeField] Slider boostSlider;

    private void Update()
    {
        if (movement.isRunning)
            boostAmount -= boostDrainSpeed;

        boostSlider.value = Mathf.Lerp(boostSlider.value, boostAmount, 0.2f);

        if (boostAmount <= 0 && hasAvailableBoost)
        {
            hasAvailableBoost = false;
            movement.isRunning = false;
        }
        else
        {
            hasAvailableBoost = true;
        }
    }

    private void VisualSetup()
    {
        boostMesh.material.SetFloat("_Opacity", 0);
    }

    public void ActivateBoostVisual()
    {
        boostMesh.material.SetFloat("_TileAmount", 0);
        boostMesh.material.DOFloat(1,"_Opacity", 0);
        boostMesh.material.DOFloat(1,"_TileAmount", 0);
        boostMesh.material.DOFloat(0, "_Opacity", 0.2f).SetDelay(.2f) ;
    }

    public void AddToBoost()
    {
        float total = boostAmount + boostGainAmount;
        boostAmount = Mathf.Clamp(boostAmount + boostGainAmount, 0, 1);
    }

    void SetBoost(float boost)
    {
        boostAmount = boost;
    }
}
