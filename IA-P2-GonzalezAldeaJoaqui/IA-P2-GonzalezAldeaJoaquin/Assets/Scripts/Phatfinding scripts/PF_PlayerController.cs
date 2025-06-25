using UnityEngine;
using System.Linq;

public class PF_PlayerController : MonoBehaviour
{
    public PF_Grid grid;
    public float moveSpeed = 5f;

    private PF_Nodes currentNode;
    private PF_Nodes targetNode;

    private bool isMoving = false;
    private Vector3 startPos;
    private Vector3 endPos;
    private float moveTime = 0f;
    private float totalMoveTime;

    private Quaternion startRot;
    private Quaternion endRot;
    public float rotationSpeed = 520f; // gradosps

    void Start()
    {
        currentNode = grid.nodeMatrix
            .OrderBy(n => Vector3.Distance(n.transform.position, transform.position))
            .FirstOrDefault();

        if (currentNode != null)
        {
            transform.position = currentNode.transform.position;
        }
       
    }

    void Update()
    {
        if (isMoving)
        {
            moveTime += Time.deltaTime;
            float t = Mathf.Clamp01(moveTime / totalMoveTime);

            // Movimiento con Lerp
            transform.position = Vector3.Lerp(startPos, endPos, t);

            // Rotación más rápida que el movimiento
            Vector3 dirToTarget = (endPos - transform.position).normalized;
            if (dirToTarget != Vector3.zero)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(dirToTarget, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
            }

            if (t >= 1f)
            {
                isMoving = false;
                currentNode = targetNode;
                targetNode = null;
            }

            return;
        }

        if (Input.GetKeyDown(KeyCode.W)) TryMove(Vector3.forward);
        if (Input.GetKeyDown(KeyCode.S)) TryMove(Vector3.back);
        if (Input.GetKeyDown(KeyCode.A)) TryMove(Vector3.left);
        if (Input.GetKeyDown(KeyCode.D)) TryMove(Vector3.right);
    }

    void TryMove(Vector3 dir)
    {
        float angleThreshold = 100f; // angulo de giro permitido

        PF_Nodes neighbor = currentNode.Neighbors
            .Where(n => !n.IsBloqued)
            .OrderBy(n => Vector3.Angle(dir, (n.transform.position - currentNode.transform.position).normalized))
            .FirstOrDefault(n =>
                Vector3.Angle(dir, (n.transform.position - currentNode.transform.position).normalized) < angleThreshold);

        if (neighbor != null)
        {
            targetNode = neighbor;
            startPos = transform.position;
            endPos = targetNode.transform.position;
            moveTime = 0f;
            totalMoveTime = Vector3.Distance(startPos, endPos) / moveSpeed;
            isMoving = true;

            // Preparar rotación hacia destino
            Vector3 dirToTarget = (endPos - startPos).normalized;
            if (dirToTarget != Vector3.zero)
            {
                startRot = transform.rotation;
                endRot = Quaternion.LookRotation(dirToTarget, Vector3.up);
            }

            Debug.Log($"[TryMove] Moviendo a nodo ({targetNode.X}, {targetNode.Y})");
        }
        else
        {
            Debug.Log("[TryMove] No hay nodo vecino en esa dirección");
        }
    }
}
