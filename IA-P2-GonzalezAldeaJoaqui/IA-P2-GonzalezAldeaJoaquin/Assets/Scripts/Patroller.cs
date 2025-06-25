using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Patroller : MonoBehaviour
{
    public float speed = 2f;
    public PF_Grid grid;
    public List<PF_Nodes> route = new(); // Ruta asignada
    private FSM<PatrollerState> fsm;
    public PatrollerState currentState;
    public FieldOfView fov;


    void Start()
    {
        fsm = new FSM<PatrollerState>();
        fsm.AddState(PatrollerState.Patrol, new PatrollerStatePatrol(fsm, this));
        fsm.AddState(PatrollerState.FollowTarget, new PatrollerStateFollowTarget(fsm, this));
        
        fov= GetComponent<FieldOfView>();
        fsm.Transition(PatrollerState.Patrol);
    }

    void Update()
    {
        fsm.Update();
    }

    public PF_Nodes GetClosestNode()
    {
        return grid.nodeMatrix
            .OrderBy(n => Vector3.Distance(n.transform.position, transform.position))
            .FirstOrDefault();
    }

    public enum PatrollerState
    {
        Patrol,
        FollowTarget
    }

}

public static class PatrollerAlert
{
    public static Vector3? lastKnownPlayerPosition = null;
    public static void Alert(Vector3 position)
    {
        lastKnownPlayerPosition = position;
    }

    public static void Clear()
    {
        lastKnownPlayerPosition = null;
    }

    public static bool HasAlert => lastKnownPlayerPosition != null;
}



public abstract class State<T>
{
    protected FSM<T> _fsm;
    public virtual void Enter() { }
    public virtual void Execute() { }
    public virtual void Exit() { }
}


public class FSM<T>
{
    private Dictionary<T, State<T>> states = new();
    private State<T> current;

    public void AddState(T key, State<T> state)
    {
        states[key] = state;
    }

    public void Transition(T newState)
    {
        current?.Exit();
        current = states[newState];
        current.Enter();
    }

    public void Update()
    {
        current?.Execute();
    }
}

