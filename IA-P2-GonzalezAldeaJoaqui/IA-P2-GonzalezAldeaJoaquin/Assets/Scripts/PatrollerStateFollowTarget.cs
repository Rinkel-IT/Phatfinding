using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Patroller;

public class PatrollerStateFollowTarget : State<PatrollerState>
{
    private Patroller patroller;
    private List<PF_Nodes> path = new();
    private float arriveThreshold = 0.1f;

    public PatrollerStateFollowTarget(FSM<PatrollerState> fsm, Patroller patroller)
    {
        this._fsm = fsm;
        this.patroller = patroller;
    }

    public override void Enter()
    {
        Debug.Log($"[{patroller.name}] Entrando en FollowTarget");
        path = new List<PF_Nodes>();
        CalculatePathToAlert();
    }

    public override void Execute()
    {
        // Si ve al jugador, actualiza su posición como destino
        if (patroller.fov.targetVisible != null)
        {
            Vector3 playerPos = patroller.fov.targetVisible.position;
            float distanceToPlayer = Vector3.Distance(patroller.transform.position, playerPos);

            // Si ya está cerca del jugador, no recalcula ni se mueve
            if (distanceToPlayer < 1.5f)
            {
                // Podés mirar al jugador si querés
                Vector3 dirToPlayer = (playerPos - patroller.transform.position).normalized;
                if (dirToPlayer != Vector3.zero)
                    patroller.transform.rotation = Quaternion.LookRotation(dirToPlayer);

                return; // no seguir moviéndose
            }

            PatrollerAlert.Alert(playerPos);
            CalculatePathToAlert();
        }

        // Movimiento por el path
        if (path.Count > 0)
        {
            Vector3 target = path[0].transform.position;
            Vector3 dir = (target - patroller.transform.position).normalized;
            patroller.transform.position += dir * patroller.speed * Time.deltaTime;

            if (dir != Vector3.zero)
                patroller.transform.rotation = Quaternion.LookRotation(dir);

            if (Vector3.Distance(patroller.transform.position, target) < arriveThreshold)
            {
                path.RemoveAt(0);
            }
        }
        else
        {
            // Si no ve al jugador al llegar al destino, vuelve a patrullar
            if (patroller.fov.targetVisible == null)
            {
                PatrollerAlert.Clear();
                _fsm.Transition(PatrollerState.Patrol);
            }
        }
    }


    private Vector3 lastPathTarget;
    private float recalcThreshold = 1.5f;

    private void CalculatePathToAlert()
    {
        if (!PatrollerAlert.HasAlert) return;

        Vector3 alertPos = PatrollerAlert.lastKnownPlayerPosition.Value;

        //  Evitar recalcular si el nuevo destino está muy cerca del anterior
        if (Vector3.Distance(alertPos, lastPathTarget) < recalcThreshold && path.Count > 0)
            return;

        lastPathTarget = alertPos;

        PF_Nodes start = patroller.GetClosestNode();
        PF_Nodes end = patroller.grid.nodeMatrix
            .OrderBy(n => Vector3.Distance(n.transform.position, alertPos))
            .FirstOrDefault();

        if (end != null)
            path = Pathfinding.Astar(start, end);
    }

}
