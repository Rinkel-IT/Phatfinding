using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] float range = 10f;
    [SerializeField] float angle = 90f;
    [SerializeField] LayerMask obstaclesMask;
    [SerializeField] LayerMask playerMask;

    public Transform targetVisible; // Si ve al jugador, se guarda acá

    void Update()
    {
        targetVisible = DetectPlayer();
    }

    public bool IsInFOV(Transform target)
    {
        Vector3 myPosition = transform.position;
        Vector3 targetPosition = target.position;
        Vector3 directionToTarget = targetPosition - myPosition;

        if (directionToTarget.sqrMagnitude > range * range) return false;

        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
        if (angleToTarget > angle / 2) return false;

        return !Physics.Linecast(myPosition, targetPosition, obstaclesMask);
    }

    public Transform DetectPlayer()
    {
        var players = Physics.OverlapSphere(transform.position, range, playerMask);

        foreach (var hit in players)
        {
            Vector3 dir = hit.transform.position - transform.position;

            if (Vector3.Angle(transform.forward, dir) > angle / 2) continue;
            if (Physics.Linecast(transform.position, hit.transform.position, obstaclesMask)) continue;

            return hit.transform; // jugador encontrado
        }

        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);

        Color arcColor = targetVisible != null ? Color.red : Color.yellow;
        arcColor.a = 0.05f;
        Handles.color = arcColor;

        Vector3 leftSide = Quaternion.Euler(0, -angle / 2, 0) * transform.forward * range;
        Handles.DrawSolidArc(transform.position, Vector3.up, leftSide, angle, range - 0.2f);

        if (targetVisible != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, targetVisible.position);
        }
    }
}
