using System.Collections.Generic;
using UnityEngine;

public class SimpleIK : MonoBehaviour
{
    public Transform target;

    //how many times should simulate every frame
    public int simulation = 1;
    private Vector3 rootPosition;
    private class Node
    {
        public Transform mTransform;
        //distance to the parent
        public float length;
    }

    private List<Node> nodes;
    
    void Start()
    {
        Init();
    }

    private void Init()
    {
        nodes = new List<Node>();
        Transform current = transform;
        while (current != null)
        {
            var node = new Node();
            node.mTransform = current;
            if (current.parent != null)
            {
                node.length = (current.position - current.parent.position).magnitude;
            }
            nodes.Add(node);

            if (current.childCount > 0)
            {
                current = current.GetChild(0);
            }
            else
            {
                break;
            }
        }
    }

    void LateUpdate()
    {
        rootPosition = transform.position;
        for (int i = 0; i < simulation; i++)
        {
            InverseSimulation();
            ForwardSimulation();
        }
    }
    
    /// <summary>
    /// From tail to head, move every node to its target position
    /// </summary>
    private void InverseSimulation()
    {
        Vector3 targetPosition;
        if (target != null)
            targetPosition = target.position;
        else
            targetPosition = nodes[nodes.Count - 1].mTransform.position;
        
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            nodes[i].mTransform.position = targetPosition;
            if (i == 0)
                break;
            Vector3 dir = targetPosition - nodes[i - 1].mTransform.position;
            targetPosition = targetPosition - dir.normalized * nodes[i].length;
        }
    }
    
    /// <summary>
    /// From head to tail, move every node to its target position
    /// </summary>
    private void ForwardSimulation()
    {
        Vector3 targetPosition = rootPosition;
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].mTransform.position = targetPosition;
            if (i == nodes.Count - 1)
                break;
            Vector3 dir = targetPosition - nodes[i + 1].mTransform.position;
            targetPosition = targetPosition - dir.normalized * nodes[i + 1].length;
        }
    }

    private void OnDrawGizmos()
    {
        if(Application.isEditor && !Application.isPlaying)
            Init();
        
        
        Gizmos.color = Color.yellow;
        
        for (int i = 1; i < nodes.Count; i++)
        {
            Gizmos.DrawLine(nodes[i - 1].mTransform.position,nodes[i].mTransform.position);
        }
        Gizmos.color = Color.red;
        if(target != null)
            Gizmos.DrawLine(nodes[nodes.Count - 1].mTransform.position,target.position);
    }
}
