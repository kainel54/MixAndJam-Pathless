using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class TargetSystem : MonoBehaviour
{
    public static TargetSystem instance;
    private MovementInput movement;
    private ArrowSystem arrowSystem;

    [HideInInspector] public Vector3 lerpedTargetPos;

    public List<ArrowTarget> targets;
    public List<ArrowTarget> reachableTargets;

    [HideInInspector] public UnityEvent<Transform> OnTargetChange;

    [Header("Target")]
    public ArrowTarget currentTarget;
    public ArrowTarget storedTarget;

    [Header("Parameters")]
    [SerializeField] float screenDistanceWeight = 1;
    [SerializeField] float positionDistanceWeight = 8;
    public float minReachDistance = 70;
    public float targetDisableCooldown = 4;

    [Header("User Interface")]
    public RectTransform rectImage;
    public float rectSizeMultiplier = 2;

    [Header("Focus Point")]
    [SerializeField] RectTransform focusPointRect;
    [SerializeField] float deltaSpeed = 100;

    private void Awake()
    {
        instance = this;
        Cursor.lockState = CursorLockMode.Locked;
        movement = GetComponent<MovementInput>();
        arrowSystem = GetComponent<ArrowSystem>();
    }

    private void Update()
    {
        SetFocusPoint();

        if (reachableTargets.Count < 1 || !arrowSystem.active)
        {
            storedTarget = null;
            rectImage.gameObject.SetActive(false);

        }

        Debug.Log("檣策蝶天天天天");
        CheckTargetChange();

        currentTarget = reachableTargets[TargetIndex()];

        rectImage.gameObject.SetActive(true);
        rectImage.transform.position = ClampedScreenPosition(currentTarget.transform.position);
        float distanceFromTarget = Vector3.Distance(currentTarget.transform.position, transform.position);
        rectImage.sizeDelta = new Vector2(Mathf.Clamp(115 - (distanceFromTarget - rectSizeMultiplier), 50, 200), Mathf.Clamp(115 - (distanceFromTarget - rectSizeMultiplier), 50, 200));
    }


    private int TargetIndex()
    {
        float[] distances = new float[reachableTargets.Count];

        for (int i = 0; i < reachableTargets.Count; i++)
        {
            distances[i] =
                (Vector2.Distance(Camera.main.WorldToScreenPoint(reachableTargets[i].transform.position), MiddleOfScreen() * screenDistanceWeight) +
                (Vector3.Distance(transform.position, reachableTargets[i].transform.position) * positionDistanceWeight));
        }

        float minDistance = Mathf.Min(distances);

        int index = 0;

        for (int i = 0; i < distances.Length; i++)
        {
            if (minDistance == distances[i])
                index = i;
        }

        Debug.Log(index);
        return index;

    }

    private Vector2 MiddleOfScreen()
    {
        return new Vector2(Screen.width/2,Screen.height/2);  
    }

    private void CheckTargetChange()
    {
        if (storedTarget != currentTarget)
        {
            storedTarget = currentTarget;
            rectImage.DOComplete();
            rectImage.DOScale(4,0.2f).From();
        }
    }

    private void SetFocusPoint()
    {
        Vector3 targetPositionX;
        if (currentTarget != null && movement.isRunning)
            targetPositionX = new Vector3(ClampedScreenPosition(currentTarget.transform.position).x, Screen.height / 2, 0);
        else
            targetPositionX = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        focusPointRect.position = Vector3.MoveTowards(focusPointRect.position, targetPositionX, deltaSpeed*Time.deltaTime);
    }

    private Vector3 ClampedScreenPosition(Vector3 targetPos)
    {
        Vector3 worldtoScreenPos = Camera.main.WorldToScreenPoint(targetPos);
        Vector3 clampedPosition = new Vector3(Mathf.Clamp(worldtoScreenPos.x,0,Screen.width),Mathf.Clamp(worldtoScreenPos.y,0,Screen.height),worldtoScreenPos.z);
        return clampedPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(lerpedTargetPos,1f);
    }

    public void StopTargetFocus()
    {
        //arrowSystem.CancelFire(false);
        currentTarget = null;
    }
}
