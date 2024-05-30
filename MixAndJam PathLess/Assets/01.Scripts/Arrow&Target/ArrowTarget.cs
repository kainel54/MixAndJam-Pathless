using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ArrowTarget : MonoBehaviour
{
    private Transform player;
    private MovementInput playerMovement;
    private Renderer detectorRenderer;
    private Color originalColor;

    [Header("Bool")]
    public bool isAvilable = true;
    public bool isReachable = false;

    

    private void Awake()
    {
        playerMovement = FindObjectOfType<MovementInput>();
        player = playerMovement.transform;
    }

    private void Update()
    {
        Vector3 playerToObject = transform.position - playerMovement.transform.position;
        bool isBehindPlayer = (Vector3.Dot(Camera.main.transform.forward,playerToObject) < 0)&&playerMovement.isRunning;

        if (Vector3.Distance(transform.position, player.position) < TargetSystem.instance.minReachDistance && !isBehindPlayer && !isReachable)
        {
            isReachable = true;
            if(TargetSystem.instance.targets.Contains(this))
                TargetSystem.instance.targets.Add(this);
        }
        if ((Vector3.Distance(transform.position, player.position) > TargetSystem.instance.minReachDistance || isBehindPlayer)&& isReachable){
            isReachable = false;
            if (TargetSystem.instance.reachableTargets.Contains(this))
                TargetSystem.instance.reachableTargets.Remove(this);
            if (TargetSystem.instance.currentTarget == this)
                TargetSystem.instance.StopTargetFocus();
        }
    }

    private void OnBecameVisible()
    {
        Debug.Log("카메라 안임");
        if (!TargetSystem.instance.targets.Contains(this) && isAvilable)
        {
            TargetSystem.instance.targets.Add (this);

            if (this.isReachable)
            {
                TargetSystem.instance.reachableTargets.Add(this);
            }
        }
    }

    private void OnBecameInvisible()
    {
        Debug.Log("카메라 밖임");
        if (TargetSystem.instance.targets.Contains(this))
        {
            TargetSystem.instance.targets.Remove(this);

            if (TargetSystem.instance.reachableTargets.Contains(this))
                TargetSystem.instance.reachableTargets.Remove (this);
        }

        if(TargetSystem.instance.currentTarget == this)
        {
            TargetSystem.instance.StopTargetFocus();
        }
    }
}
