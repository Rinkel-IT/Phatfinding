using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PF_Grid : MonoBehaviour
{
    public PF_Nodes nodePrefab;
    public int width, height;
    public PF_Nodes[] nodeMatrix;
    public float spacing;

    // Tus 4 rutas manuales
    public List<PF_Nodes> route1 = new();
    public List<PF_Nodes> route2 = new();
    public List<PF_Nodes> route3 = new();
    public List<PF_Nodes> route4 = new();

    [ContextMenu("Create Grid")]
    public void CreateMatrix()
    {
        nodeMatrix = new PF_Nodes[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var space = new Vector3(x * spacing, 0, y * spacing);
                var node = Instantiate(nodePrefab, transform.position + space, Quaternion.identity, transform);
                nodeMatrix[x + y * height] = node;
                node.Initialize(x, y);
            }
        }
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                AddNeighbors(nodeMatrix[x + y * height], x, y);
            }
        }
    }

    private void AddNeighbors(PF_Nodes node, int x, int y)
    {
        List<PF_Nodes> vecinos = new List<PF_Nodes>();
        if (x > 0) vecinos.Add(nodeMatrix[x - 1 + y * height]);
        if (y > 0) vecinos.Add(nodeMatrix[x + (y - 1) * height]);
        if (x < width - 1) vecinos.Add(nodeMatrix[x + 1 + y * height]);
        if (y < height - 1) vecinos.Add(nodeMatrix[x + (y + 1) * height]);
        node.Neighbors = vecinos;
    }

    public PF_Nodes GetNodeAt(int x, int y)
    {
        return nodeMatrix[x + y * height];
    }

    private void OnDrawGizmos()
    {
        DrawRoute(route1, Color.red);
        DrawRoute(route2, Color.green);
        DrawRoute(route3, Color.magenta);
        DrawRoute(route4, Color.yellow);
    }

    private void DrawRoute(List<PF_Nodes> route, Color color)
    {
        if (route == null || route.Count < 2) return;

        Gizmos.color = color;
        for (int i = 0; i < route.Count - 1; i++)
        {
            if (route[i] != null && route[i + 1] != null)
            {
                Gizmos.DrawLine(route[i].transform.position, route[i + 1].transform.position);
            }
        }
    }

}
