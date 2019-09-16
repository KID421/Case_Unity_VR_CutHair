using System.Collections.Generic;
using UnityEngine;

// THis is the component of the moving points
[RequireComponent(typeof(MeshRenderer))]
public class GizmoScript : MonoBehaviour
{
    // If I'm Editing
    public bool pointsSelected = false;

    // Every vertex bind to this object
    public List<int> vertIndex = new List<int>();
    
    // Size of gizmo Sphere
    public float GizmoSize = 0.05f;

    // Whenn this point is not selected 
    void OnDrawGizmos()
    {
        if (pointsSelected)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, GizmoSize);
        }
    }

    // Whenn this point is selected 
    void OnDrawGizmosSelected()
    {
        if (pointsSelected)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, GizmoSize);
            AdjustVert();
        }
    }

    // Update position on vertices
    public void AdjustVert()
    {
        GameObjectDeformer parent = FindDeeformParent();
        if (parent != null)
            parent.AdjustVertices(this.gameObject);
    }

    // Finding main object
    public GameObjectDeformer FindDeeformParent()
    {
        if (transform.parent.GetComponent<GameObjectDeformer>() != null)
            return transform.parent.GetComponent<GameObjectDeformer>();
        if (transform.parent.parent.GetComponent<GameObjectDeformer>() != null)
            return transform.parent.parent.GetComponent<GameObjectDeformer>();
        return null;
    }
}
