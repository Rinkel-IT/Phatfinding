using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PF_Nodes : MonoBehaviour
{
    [SerializeField] private int x, y;
    [SerializeField] List<PF_Nodes> neighbors;
    [SerializeField] int cost;
    [SerializeField] bool isBlocked;
    public bool IsBloqued => isBlocked;
    public int Cost => cost;
    public int X => x;
    public int Y => y;

    public List<PF_Nodes> Neighbors
    {
        get { return neighbors; }
        set { neighbors = value; }
    }

    public void Initialize(int x, int y)
    {
        this.x = x;
        this.y = y;
        gameObject.name = gameObject.name + $"({x}, {y})";
    }

    private void OnMouseDown()
    {
        PF_Manager.Instance.DoPath(this);
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < neighbors.Count; i++)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, neighbors[i].transform.position);
        }
    }
}
