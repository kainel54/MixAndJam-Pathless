using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ArrowSystem : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnTargetHit;
    [HideInInspector] public UnityEvent OnInputStart;
    [HideInInspector] public UnityEvent OnInputRelease;
    [HideInInspector] public UnityEvent<float> OnArrowRelease;
    [HideInInspector] public UnityEvent OnTargetLost;


    private TargetSystem targetSystem;
    private ArrowTarget lockedTarget;
    private Coroutine arrowSystemCooldown;
    private MovementInput movement;
    

    [Header("Arrow Settings")]
    [SerializeField] float arrowCooldown = .5f;
    [SerializeField] ParticleSystem wrongArrowEmission;
    [SerializeField] ParticleSystem correctArrowEmission;
    [SerializeReference] Transform arrowReleasePoint;


    public bool active;

    [Header("Charge Settings")]
    public bool isCharging = false;
    public bool releaseCooldown = false;
    [SerializeField] float chargeDuration = .8f;
    [SerializeField] Ease chargeEase;
    private float chargeAmount;
    public float middleChargePrecision = .5f;

    [Header("Parameters")]
    [SerializeField] float slowDownInterval;

    [Header("UI Connections")]
    public Slider arrowPrecisionSlider;

    [Header("Input")]
    public InputActionReference fireAction;

    public ParticleSystem particleTest;
    private void Awake()
    {
        targetSystem = GetComponent<TargetSystem>();
        movement = GetComponent<MovementInput>();

        fireAction.action.performed += FireAction_performed;
        fireAction.action.canceled += FireAction_canceled;
    }

    private void Update()
    {
        GetComponent<Animator>().SetBool("isCharging", isCharging);
        GetComponent<Animator>().SetBool("releaseCooldown", releaseCooldown);
        GetComponent<Animator>().SetBool("isRunning", FindObjectOfType<MovementInput>().isRunning);
    }

    private void FireAction_performed(InputAction.CallbackContext obj)
    {
        StartFire();
    }

    private void FireAction_canceled(InputAction.CallbackContext obj)
    {
        CancelFire(true);
    }

    private void StartFire()
    {
        if (!active)
            return;
        if (targetSystem.currentTarget == null || !targetSystem.currentTarget.isAvilable)
            return;

        isCharging = true;
        OnInputStart.Invoke();

        DOVirtual.Float(0, 1, chargeDuration, SetChargeAmount).SetId(0).SetEase(chargeEase);
    }

    private void SetChargeAmount(float charge)
    {
        chargeAmount = charge;
        arrowPrecisionSlider.value = chargeAmount;
    }

    private void CancelFire(bool input)
    {
        if (input)
        {
            if (targetSystem.currentTarget == null || !targetSystem.currentTarget.isAvilable)
                return;
            if (!isCharging)
                return;

            OnInputRelease.Invoke();

            CheckArrowRelease();
        }
    }

    private void CheckArrowRelease()
    {
        DisableSystemForPeriod();
        StartCoroutine(ReleaseCooldown());

        if (HalfCharge())
        {
            chargeAmount = 0.5f;
        }
        if (FullCharge())
        {
            chargeAmount = 1;
        }

        OnArrowRelease.Invoke(chargeAmount);

        if(chargeAmount>=1-middleChargePrecision||(chargeAmount>.5f-middleChargePrecision&& chargeAmount < .5f + middleChargePrecision))
        {
            if (chargeAmount > .5f - middleChargePrecision && chargeAmount < .5f + middleChargePrecision)
                StartCoroutine(SlowTime());
            ReleaseCorrectArrow();
        }
        else
        {
            ReleaseWrongArrow();
        }
    }

    private void ReleaseWrongArrow()
    {
        lockedTarget = targetSystem.currentTarget;
        wrongArrowEmission.transform.position = arrowReleasePoint.position;
        wrongArrowEmission.transform.LookAt(targetSystem.currentTarget.transform);
        wrongArrowEmission.Play();
    }

    private void ReleaseCorrectArrow()
    {
        lockedTarget = targetSystem.currentTarget;
        correctArrowEmission.transform.position = lockedTarget.transform.position;
        var shape = correctArrowEmission.shape;
        shape.position = correctArrowEmission.transform.InverseTransformPoint(arrowReleasePoint.position);
        correctArrowEmission.Play();
    }

    private IEnumerator SlowTime()
    {
        float scale = 0.2f;
        Time.timeScale = scale;
        yield return new WaitForSeconds(slowDownInterval/(1/scale));
        Time.timeScale = 1;
    }

    private bool FullCharge()
    {
        return chargeAmount >= 1 - middleChargePrecision;
    }

    public bool HalfCharge()
    {
        return chargeAmount > .5f - middleChargePrecision && chargeAmount < .5f+middleChargePrecision;
    }

    private IEnumerator ReleaseCooldown()
    {
        releaseCooldown = true;
        yield return new WaitForSeconds(0.4f);
        releaseCooldown = false;
    }

    private void DisableSystemForPeriod()
    {
        active = false;

        if (arrowSystemCooldown != null) 
            StopCoroutine(arrowSystemCooldown);
        arrowSystemCooldown = StartCoroutine(ArrowCooldown());
        IEnumerator ArrowCooldown()
        {
            yield return new WaitForSeconds(arrowCooldown);
            active = true;
        }
    }

    public void TargetHit(Vector3 dir)
    {
        OnTargetHit.Invoke();

        active = true;
        releaseCooldown = false;
        lockedTarget.DisableTarget(dir);

        var shape = particleTest.shape;
        shape.position = transform.InverseTransformPoint(lockedTarget.transform.position);
        particleTest.Play();
    }

    
}
