using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameObjectDeformer))]
public class GameObjectDeformerEditor : Editor
{
    // Target
    GameObjectDeformer deformMesh;

    // We are creating new mesh, because we dont want to change the source mesh
    Mesh newMesh;


    // Initializing variables, creating copy of our mesh and saving it as DEFAULT
    private void OnEnable()
    {
        deformMesh = (GameObjectDeformer)target;
        deformMesh.gameObject.layer = LayerMask.NameToLayer("Default");
        if (deformMesh.MeshFilter == null) deformMesh.MeshFilter = deformMesh.gameObject.GetComponent<MeshFilter>();
        if (deformMesh.Initialized) return;

        CopyAndSaveMesh();

        deformMesh.objs = new List<GameObject>();
        deformMesh.vert = new List<Vector3>();

        deformMesh.Initialized = true;
    }

    // We can rename our mesh and save it
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("SaveMesh"))
        {
            ResetPoints();
            CopyAndSaveMesh();
        }
    }

    // Gui what we see in scene Window GUI
    protected virtual void OnSceneGUI()
    {
        Handles.BeginGUI();

        // we cannot be able to click and select the main object ( we need to create NotSelectable layer first )
        if (GUI.Button(new Rect(Screen.width / 2 - 60, 10, 120, 20), "Lock Main Object"))
        {
            try
            {
                deformMesh.gameObject.layer = LayerMask.NameToLayer("NotSelectable");
            }
            catch (System.Exception)
            {
                Debug.LogError("Please set up NotSelectable Layer in the Layer List, and set it Not Clickable! More information in Documentation.");
            }
        }

        if (GUI.Button(new Rect(10, Screen.height / 2 - 50, 50, 20), "Point"))
            CreatePoints();

        Handles.EndGUI();
    }

    // Copy mesh to not change source
    void CopyAndSaveMesh()
    {
        if (deformMesh != null && !string.IsNullOrEmpty(deformMesh.MeshName))
        {
            Mesh mesh = deformMesh.MeshFilter.sharedMesh;
            newMesh = new Mesh();
            newMesh.vertices = mesh.vertices;
            newMesh.triangles = mesh.triangles;
            newMesh.uv = mesh.uv;
            newMesh.normals = mesh.normals;
            newMesh.colors = mesh.colors;
            newMesh.tangents = mesh.tangents;
            AssetDatabase.CreateAsset(newMesh, "Assets/" + deformMesh.MeshName + ".asset");
            if (deformMesh.gameObject.GetComponent<MeshCollider>() != null)
                deformMesh.gameObject.GetComponent<MeshCollider>().sharedMesh = newMesh;
            deformMesh.MeshFilter.sharedMesh = newMesh;
            EditorUtility.SetDirty(deformMesh.MeshFilter);
            AssetDatabase.SaveAssets();
        }
    }

    // Create and move points 
    void CreatePoints()
    {
        ResetPoints();

        var verticies = deformMesh.MeshFilter.sharedMesh.vertices;
        int length = verticies.Length;
        for (int i = 0; i < length; i++)
        {
            GameObject go = new GameObject("Point " + i);
            go.transform.parent = deformMesh.transform;
            go.transform.position = deformMesh.transform.TransformPoint(verticies[i]);
            go.AddComponent<GizmoScript>();
            go.GetComponent<GizmoScript>().pointsSelected = true;
            deformMesh.objs.Add(go);
            deformMesh.vert.Add(verticies[i]);
        }

        for (int i = 0; i < length - 1; i++)
        {
            for (int j = i + 1; j < length; j++)
            {
                if (Vector3.Distance(deformMesh.objs[i].transform.position, deformMesh.objs[j].transform.position) < 0.001f)
                {
                    DestroyImmediate(deformMesh.objs[j]);
                    deformMesh.objs.RemoveAt(j);
                    length--;
                }
            }
        }

        for (int i = 0; i < deformMesh.objs.Count; i++)
        {
            for (int j = 0; j < deformMesh.vert.Count; j++)
            {
                if (Vector3.Distance(deformMesh.objs[i].transform.position, deformMesh.transform.TransformPoint(deformMesh.vert[j])) < 0.05f)
                {
                    deformMesh.objs[i].GetComponent<GizmoScript>().vertIndex.Add(j);
                }
            }
        }
    }

    
    void ResetPoints()
    {
        for (int i = 0; i < deformMesh.transform.childCount;)
        {
            DestroyImmediate(deformMesh.transform.GetChild(0).gameObject);
        }
        deformMesh.objs.Clear();
        deformMesh.vert.Clear();
    }
}