using UnityEngine;
using System.Collections.Generic;

public class NpcTargeting : MonoBehaviour
{
    public Transform target;
    public List<Transform> potentialTargets = new List<Transform>();
    private NpcAI npcAI;

    private void Awake()
    {
        NpcAI.OnNewNpcAdded += AddNewNpcToPotentialTargets;
        npcAI = GetComponent<NpcAI>();
    }

    public void OnDestroy()
    {
        NpcAI.OnNewNpcAdded -= AddNewNpcToPotentialTargets;
    }

    public void AddNewNpcToPotentialTargets(Transform newNpc)
    {
        if (newNpc != this.transform)
        {
            potentialTargets.Add(newNpc);
        }
    }

    public Transform GetClosestTarget(Transform currentNpcTransform, float detectRange)
    {
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform potentialTarget in potentialTargets)
        {
            if (potentialTarget != null)
            {
                float distance = Vector2.Distance(currentNpcTransform.position, potentialTarget.position);
                if (distance < closestDistance && distance <= detectRange)
                {
                    closestDistance = distance;
                    closestTarget = potentialTarget;
                }
            }
        }

        return closestTarget;
    }

    public void RemoveTarget(Transform targetToRemove)
    {
        if (potentialTargets.Contains(targetToRemove))
        {
            potentialTargets.Remove(targetToRemove);
        }
    }

    public void FindPotentialTargets(NpcAI currentNpc)
    {
        NpcAI[] targetables = FindObjectsOfType<NpcAI>();
        foreach (NpcAI targetable in targetables)
        {
            if (targetable != currentNpc)
            {
                potentialTargets.Add(targetable.transform);
            }
        }
    }





     public void FindNewTargetIfNeeded()
    {
        if (target == null || !potentialTargets.Contains(target) ||
            Vector2.Distance(transform.position, target.position) > npcAI.npcMovement.detectRange)
        {
            target =  GetClosestTarget(transform, npcAI.npcMovement.detectRange);
            if (target != null)
            {
                npcAI.npcMovement.UpdateAnimator((Vector2)target.position - (Vector2)transform.position);
            }
        }
    }
}
