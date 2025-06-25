using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PF_Entity : MonoBehaviour
{
    public List<PF_Nodes> path = new();
    public PF_Nodes start;
    public PF_Nodes end;
    public float speed;

    public PF_Grid grid;
    public List<Transform> wayPoints = new();
    public int currentWP = 0;
    public LayerMask obstacleMask;


    public List<PF_Nodes> SetPath
    {
        set { path = value; }
    }

    // Update is called once per frame
    void Update()
    {
        if (Pathfinding.LineOfSight(transform.position, wayPoints[currentWP].transform.position, obstacleMask))
        {
            var dir = wayPoints[currentWP].transform.position - transform.position;
            transform.position += dir.normalized * speed * Time.deltaTime;
        }
        else if (path.Count > 0)
        {
            var dir = path[0].transform.position - transform.position;
            transform.position += dir.normalized * speed * Time.deltaTime;
            if (dir.magnitude < 0.1f)
                path.RemoveAt(0);
        }

        if (Vector3.Distance(wayPoints[currentWP].transform.position, transform.position) < 0.3f)
        {
            currentWP++;
            if (currentWP >= wayPoints.Count)
                currentWP = 0;

            var strt = grid.nodeMatrix.OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).FirstOrDefault();
            var end = grid.nodeMatrix.OrderBy(x => Vector3.Distance(x.transform.position, wayPoints[currentWP].transform.position)).FirstOrDefault();
            path = Pathfinding.Astar(strt, end);
        }


    }
}
