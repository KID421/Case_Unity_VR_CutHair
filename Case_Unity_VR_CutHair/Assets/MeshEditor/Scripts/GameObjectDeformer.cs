using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectDeformer : MonoBehaviour {

    // Objects mesh filter ... we get the vertices from here
    [HideInInspector]
    public MeshFilter MeshFilter;

    [HideInInspector]
    public bool Initialized = false;

    // we copy our mesh first and thenn we change it ... Save your mesh after you changed it 
    public string MeshName = "Default";

    // Objects are created for moving vertices
    [HideInInspector]
    public List<GameObject> objs;

    // All vertices
    [HideInInspector]
    public List<Vector3> vert;

    public void AdjustVertices(GameObject obj)
    {
        try
        {
            GizmoScript x = obj.GetComponent<GizmoScript>();
            int length = x.vertIndex.Count;
            for (int i = 0; i < length; i++)
            {
                vert[x.vertIndex[i]] = transform.InverseTransformPoint(obj.transform.position);
            }
            MeshFilter.sharedMesh.SetVertices(vert);
            MeshFilter.sharedMesh.RecalculateBounds();

            Collider col = GetComponent<Collider>();
            if (col is MeshCollider)
                ((MeshCollider)col).sharedMesh = MeshFilter.sharedMesh;
        }
        catch (System.Exception)
        {
            Debug.LogError("No GizmoScript");
        }

    }
}
