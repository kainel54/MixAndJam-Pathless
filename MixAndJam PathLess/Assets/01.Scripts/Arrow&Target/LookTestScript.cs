using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTestScript : MonoBehaviour
{
    private Transform target;
    private ArrowTarget arrowTargetSystem;

    private void Awake()
    {
        target = FindObjectOfType<MovementInput>().transform;
        arrowTargetSystem = GetComponentInChildren<ArrowTarget>();
    }

    private void Update()
    {
        if(arrowTargetSystem.isAvilable)
            transform.LookAt(target);
    }
}
