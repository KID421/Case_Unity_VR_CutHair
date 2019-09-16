/*
 * Meshinator
 * Copyright Mike Mahoney 2013
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SubHull
{
	#region Properties & Fields
	
	// Mesh Information
	public List<Vector3> m_Vertices;
	public List<Vector3> m_Normals;
	public List<Vector4> m_Tangents;
	public List<Vector2> m_Uvs;
	public List<int> m_Triangles;
	
	// Edge Information
	public HashSet<int> m_EdgeVertexIndices;
	public Dictionary<int, List<int>> m_EdgeVertexIndexToOtherEdgeVertexIndices;
	
	#endregion Properties & Fields
	
	#region Constructors
	
	public SubHull()
	{
		m_Vertices = new List<Vector3>();
		m_Normals = new List<Vector3>();
		m_Tangents = new List<Vector4>();
		m_Uvs = new List<Vector2>();
		m_Triangles = new List<int>();
		m_EdgeVertexIndices = new HashSet<int>();
		m_EdgeVertexIndexToOtherEdgeVertexIndices = new Dictionary<int, List<int>>();
	}
	
	#endregion Constructors
	
	#region Edge Calculation
	
	public void CalculateEdges()
	{
		if (m_EdgeVertexIndices == null || m_EdgeVertexIndices.Count == 0)
			return;
		
		// Look through all out triangle vertex indices to see which edges contain 2 vertices from
		// m_EdgeVertexIndices, thus indicating an edge to the mesh that needs to be filled in
		for (int i = 0; i < m_Triangles.Count; i = i + 3)
		{
			int vertexIndex1 = m_Triangles[i];
			int vertexIndex2 = m_Triangles[i + 1];
			int vertexIndex3 = m_Triangles[i + 2];

			// If none of this triangles vertices are an edge vertex, then just continue on
			if (!m_EdgeVertexIndices.Contains(vertexIndex1) &&
				!m_EdgeVertexIndices.Contains(vertexIndex2) &&
				!m_EdgeVertexIndices.Contains(vertexIndex3))
				continue;
			
			// Make sure every edge vertex has an entry in our dictionary. Make one if it doesn't already exist
			if (m_EdgeVertexIndices.Contains(vertexIndex1))
			{
				if (!m_EdgeVertexIndexToOtherEdgeVertexIndices.ContainsKey(vertexIndex1))
					m_EdgeVertexIndexToOtherEdgeVertexIndices.Add(vertexIndex1, new List<int>());
				
				List<int> otherEdgeVertexIndices = m_EdgeVertexIndexToOtherEdgeVertexIndices[vertexIndex1];
				if (m_EdgeVertexIndices.Contains(vertexIndex2))
					otherEdgeVertexIndices.Add(vertexIndex2);
				if (m_EdgeVertexIndices.Contains(vertexIndex3))
					otherEdgeVertexIndices.Add(vertexIndex3);
			}
			if (m_EdgeVertexIndices.Contains(vertexIndex2))
			{
				if (!m_EdgeVertexIndexToOtherEdgeVertexIndices.ContainsKey(vertexIndex2))
					m_EdgeVertexIndexToOtherEdgeVertexIndices.Add(vertexIndex2, new List<int>());
				
				List<int> otherEdgeVertexIndices = m_EdgeVertexIndexToOtherEdgeVertexIndices[vertexIndex2];
				if (m_EdgeVertexIndices.Contains(vertexIndex1))
					otherEdgeVertexIndices.Add(vertexIndex1);
				if (m_EdgeVertexIndices.Contains(vertexIndex3))
					otherEdgeVertexIndices.Add(vertexIndex3);
			}
			if (m_EdgeVertexIndices.Contains(vertexIndex3))
			{
				if (!m_EdgeVertexIndexToOtherEdgeVertexIndices.ContainsKey(vertexIndex3))
					m_EdgeVertexIndexToOtherEdgeVertexIndices.Add(vertexIndex3, new List<int>());
				
				List<int> otherEdgeVertexIndices = m_EdgeVertexIndexToOtherEdgeVertexIndices[vertexIndex3];
				if (m_EdgeVertexIndices.Contains(vertexIndex1))
					otherEdgeVertexIndices.Add(vertexIndex1);
				if (m_EdgeVertexIndices.Contains(vertexIndex2))
					otherEdgeVertexIndices.Add(vertexIndex2);
			}
		}
	}
	
	#endregion Edge Calculation
	
	#region Utility Functions
	
	public bool IsEmpty()
	{
		return m_Vertices.Count < 3 || m_Triangles.Count < 3;
	}
	
	public Mesh GetMesh()
	{
		if (!IsEmpty())
		{
			Mesh mesh = new Mesh();
			
			mesh.vertices = m_Vertices.ToArray();
			mesh.triangles = m_Triangles.ToArray();

			if (m_Normals != null)
				mesh.normals = m_Normals.ToArray();

			if (m_Tangents != null)
				mesh.tangents = m_Tangents.ToArray();
			
			if (m_Uvs != null)
				mesh.uv = m_Uvs.ToArray();

			mesh.RecalculateBounds();
			return mesh;
		}
		
		return null;
	}
	
	#endregion Utility Functions
}
