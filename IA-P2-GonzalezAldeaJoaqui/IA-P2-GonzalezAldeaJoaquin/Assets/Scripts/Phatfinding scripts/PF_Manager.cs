using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PF_Manager : MonoBehaviour
{
    public static PF_Manager Instance { get; private set; }

    [SerializeField] PF_Entity entity;
    [SerializeField] PF_Grid grid;
    [SerializeField] LayerMask obstacleMask;

    private void Awake()
    {
        if (Instance)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void DoPath(PF_Nodes end)
    {
        // Colorear nodos según coste o bloqueo
        foreach (var node in grid.nodeMatrix)
        {
            node.GetComponent<Renderer>().material.color = node.IsBloqued ? Color.magenta :
                Color.Lerp(Color.yellow, Color.red, (float)node.Cost / 5);
        }

        // Obtener nodo de inicio más cercano a la entidad
        var start = grid.nodeMatrix
            .Where(node => Vector3.Distance(node.transform.position, entity.transform.position) < 5)
            .OrderBy(node => Vector3.Distance(node.transform.position, entity.transform.position))
            .FirstOrDefault();

        var path = new List<PF_Nodes>();

        // Solo se usa AstarPostSmoothing
        path = Pathfinding.AstarPS(start, end, obstacleMask);

        // Pintar la ruta generada
        for (int i = 0; i < path.Count; i++)
        {
            path[i].GetComponent<Renderer>().material.color = Color.Lerp(Color.black, Color.green, (float)i / path.Count);
        }

        entity.SetPath = path;
    }
}
