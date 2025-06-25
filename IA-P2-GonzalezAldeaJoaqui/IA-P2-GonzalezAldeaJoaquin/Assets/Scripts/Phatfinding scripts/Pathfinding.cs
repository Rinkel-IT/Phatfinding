using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    // Línea de visión para post-procesamiento
    public static bool LineOfSight(Vector3 a, Vector3 b, LayerMask mask)
    {
        return !Physics.Linecast(a, b, mask);
    }

    // Heurística Manhattan (necesaria para A*)
    public static int Heuristic(PF_Nodes a, PF_Nodes b)
    {
        return Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Y - b.Y);
    }

    //  MÉTODO ACTIVO
    public static List<PF_Nodes> AstarPS(PF_Nodes start, PF_Nodes end, LayerMask mask)
    {
        var path = Astar(start, end);

        int count = 0;
        while (count + 2 < path.Count)
        {
            var a = path[count].transform.position;
            var b = path[count + 2].transform.position;

            if (LineOfSight(a, b, mask))
                path.RemoveAt(count + 1);
            else
                count++;
        }

        return path;
    }

   
    public static List<PF_Nodes> Astar(PF_Nodes start, PF_Nodes end)
    {
        if (start == null || end == null) return null;

        var frontier = new PriorityQueue<PF_Nodes>();
        var cameFrom = new Dictionary<PF_Nodes, PF_Nodes>();
        var costSoFar = new Dictionary<PF_Nodes, float>();

        frontier.Enqueue(start, 0);
        cameFrom.Add(start, null);
        costSoFar.Add(start, 0);

        while (!frontier.IsEmpty)
        {
            var current = frontier.Dequeue();
            current.GetComponent<Renderer>().material.color = Color.blue;

            if (current == end)
            {
                var pathToReturn = new List<PF_Nodes>();
                while (current != start)
                {
                    pathToReturn.Add(current);
                    current = cameFrom[current];
                }
                pathToReturn.Add(start);
                pathToReturn.Reverse();
                return pathToReturn;
            }

            foreach (var next in current.Neighbors)
            {
                if (next.IsBloqued) continue;

                var newCost = costSoFar[current] + next.Cost;

                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    float priority = newCost + Heuristic(next, end);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }

        return null;
    }

    /*
    // ALGORTIMOS COMENTADOS POR LIMPIEZA

    public static List<PF_Nodes> BFS(PF_Nodes start, PF_Nodes end) { ... }
    public static List<PF_Nodes> Dijkstra(PF_Nodes start, PF_Nodes end) { ... }
    public static List<PF_Nodes> GreedyBestFirst(PF_Nodes start, PF_Nodes end) { ... }
    public static List<PF_Nodes> ThetaStar(PF_Nodes start, PF_Nodes end, LayerMask mask) { ... }
    public static List<PF_Nodes> ThetaStar(PF_Nodes start, PF_Nodes end, PF_Grid grid) { ... }
    */
}
