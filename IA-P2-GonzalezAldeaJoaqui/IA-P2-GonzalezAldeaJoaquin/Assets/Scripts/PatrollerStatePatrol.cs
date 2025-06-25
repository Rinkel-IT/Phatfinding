using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Patroller;

public class PatrollerStatePatrol : State<PatrollerState>
{
    private Patroller patroller;
    private int currentWaypoint = 0;
    private int direction = 1; // 1 = ida, -1 = vuelta
    private List<PF_Nodes> path = new();
    private float arriveThreshold = 0.1f;

    public PatrollerStatePatrol(FSM<PatrollerState> fsm, Patroller patroller)
    {
        this._fsm = fsm;
        this.patroller = patroller;
    }

    public override void Enter()
    {
        Debug.Log($"[{patroller.name}] Entrando en Patrol");
        path = new List<PF_Nodes>();
        CalculatePathToNextWaypoint();
    }

    public override void Execute()
    {
        // Si hay una alerta pero aún no veo al jugador, ir a investigarla
        if (PatrollerAlert.HasAlert && patroller.fov.targetVisible == null)
        {
            _fsm.Transition(PatrollerState.FollowTarget);
            return;
        }


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
            UpdateWaypointIndex();
            CalculatePathToNextWaypoint();
        }
        if (patroller.fov.targetVisible != null)
        {
            PatrollerAlert.Alert(patroller.fov.targetVisible.position);
            _fsm.Transition(PatrollerState.FollowTarget); 
            return;
        }


    }

    private void UpdateWaypointIndex()
    {
        currentWaypoint += direction;

        if (currentWaypoint >= patroller.route.Count || currentWaypoint < 0)
        {
            direction *= -1; // cambiar dirección
            currentWaypoint += direction * 2; // moverse al siguiente válido
        }
    }

    private void CalculatePathToNextWaypoint()
    {
        PF_Nodes start = patroller.GetClosestNode();
        PF_Nodes end = patroller.route[currentWaypoint];

        path = Pathfinding.Astar(start, end); 
    }

}
