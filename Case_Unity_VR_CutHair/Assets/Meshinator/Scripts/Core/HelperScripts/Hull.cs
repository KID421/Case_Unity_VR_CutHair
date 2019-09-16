/*
 * Meshinator
 * Copyright Mike Mahoney 2013
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hull
{
	#region Properties & Fields
	
	private const int c_MinTrianglesPerImpact = 48;
	
	private const float c_CompressionResistance = 0.95f; // MUST be < 1 and > 0
	
	// Mesh Information
	private List<Vector3> m_Vertices;
	private List<Vector3> m_Normals;
	private List<Vector4> m_Tangents;
	private List<Vector2> m_Uvs;
	private List<int> m_Triangles;
	
	// New SubHulls for storing fracture information
	private SubHull m_FirstSubHull;
	private SubHull m_SecondSubHull;

	#endregion Properties & Fields
	
	#region Constructors
	
	public Hull(Mesh mesh)
	{
		// Get all the mesh information
		m_Vertices = new List<Vector3>(mesh.vertices);
		m_Triangles = new List<int>(mesh.triangles);
		
		if (mesh.normals.Length > 0)
			m_Normals = new List<Vector3>(mesh.normals);
		
		if (mesh.tangents.Length > 0)
			m_Tangents = new List<Vector4>(mesh.tangents);
		
		if (mesh.uv.Length > 0)
			m_Uvs = new List<Vector2>(mesh.uv);
}
	
	#endregion Constructors
	
	#region Deformation Functions

	public void Impact(Vector3 impactPoint, Vector3 impactForce, Meshinator.ImpactShapes impactShape, Meshinator.ImpactTypes impactType)
	{
		// Look through all triangles to see which ones are within the impactForce from the impactPoint,
		// and measure the area of every triangle in the list
		Dictionary<int, float> triangleIndexToTriangleArea = new Dictionary<int, float>();
		foreach (int triangleIndex in GetIntersectedTriangleIndices(impactPoint, impactForce.magnitude))
		{
			float areaOfTriangle = GetAreaOfTriangle(triangleIndex);
			triangleIndexToTriangleArea.Add(triangleIndex, areaOfTriangle);
		}

		// Keep breaking down the largest triangle until there are more than c_MinTrianglesPerImpact
		// triangles in the list
		while (triangleIndexToTriangleArea.Keys.Count < c_MinTrianglesPerImpact)
		{
			// If we have 64988 vertices or more, we can't add any more or we risk going over the
			// 65000 limit, which causes problems for unity.
			if (m_Vertices.Count > 64988)
				break;
			
			// Get the index of the biggest triangle in our dictionary
			int indexOfLargestTriangle = GetIndexOfLargestTriangle(triangleIndexToTriangleArea);

			// Break that triangle down and remove it from the dictionary
			List<int> newTriangleIndices = BreakDownTriangle(indexOfLargestTriangle);
			triangleIndexToTriangleArea.Remove(indexOfLargestTriangle);
			
			// Measure the areas of the resulting triangles, and add them to the dictionary
			foreach (int triangleIndex in newTriangleIndices)
			{
				// Make sure each triangle is still intersected by our force before we add it back to the list
				if (IsTriangleIndexIntersected(triangleIndex, impactPoint, impactForce.magnitude))
				{
					float areaOfTriangle = GetAreaOfTriangle(triangleIndex);
					triangleIndexToTriangleArea.Add(triangleIndex, areaOfTriangle);
				}
			}
		}

		// Now that we have the proper vertices and triangles, actually go about deforming the mesh
		// by moving the vertices around.
		AdjustVerticesForImpact(impactPoint, impactForce, impactShape, impactType);
	}
	
	private void AdjustVerticesForImpact(Vector3 impactPoint, Vector3 impactForce,
		Meshinator.ImpactShapes impactShape, Meshinator.ImpactTypes impactType)
	{
		// Get the radius from the impactPoint that we'll be looking into
		float impactRadius = impactForce.magnitude;
		
		// Figure out how many vertices we will move
		Dictionary<int, float> movedVertexToForceMagnitudeMap = new Dictionary<int, float>();
		for (int i = 0; i < m_Vertices.Count; i++)
		{
			Vector3 vertex = m_Vertices[i];

			// Figure out the distance from the impact to this vector in different ways to
			// determine the shape of the impact
			Vector3 distanceVector = Vector3.zero;
			switch (impactShape)
			{
				case Meshinator.ImpactShapes.FlatImpact:
					distanceVector = Vector3.Project(vertex - impactPoint, impactForce.normalized);
					break;
					
				case Meshinator.ImpactShapes.SphericalImpact:
					distanceVector = vertex - impactPoint;
					break;
			}
			
			// If we're using a FlatImpact and the angle between the impact vector, and this vertex to the point
			// if impact is greater than 90 degrees, then make the distance the negative of itself. This fixes some
			// weird issues that pop up by the physics system determining that the point of collision is inside the
			// mesh instead of on the surface
			float distance = distanceVector.magnitude;
			if (impactShape == Meshinator.ImpactShapes.FlatImpact &&
				Vector3.Angle(vertex - impactPoint, impactForce) > 90f)
				distance = -distance;
			
			// If this vertex is within the impact radius, then we'll be moving this vertex.
			// Store the magnitude of the force by which this vertex will be moved
			float vertexForceMagnitude = Mathf.Max(0, (impactRadius - distance)) * c_CompressionResistance;
			if (distance < impactRadius && vertexForceMagnitude > 0)
				movedVertexToForceMagnitudeMap.Add(i, vertexForceMagnitude);
		}
		
		// Depending on our ImpactType, deform the mesh appropriately
		switch (impactType)
		{
			case Meshinator.ImpactTypes.Compression:
				CompressMeshVertices(movedVertexToForceMagnitudeMap, impactForce);
				break;
			
			case Meshinator.ImpactTypes.Fracture:
				FractureMeshVertices(movedVertexToForceMagnitudeMap, impactForce);
				break;
		}
	}
	
	private void CompressMeshVertices(Dictionary<int, float> movedVertexToForceMagnitudeMap, Vector3 impactForce)
	{
		// Move vertices away by the force value
		foreach (int vertexIndex in movedVertexToForceMagnitudeMap.Keys)
		{
			Vector3 vertex = m_Vertices[vertexIndex];

			// Get the vector by which we will move this vertex
			float vertexForceMagnitude = movedVertexToForceMagnitudeMap[vertexIndex];
			Vector3 vertexForce = impactForce.normalized * vertexForceMagnitude;

			// Set the new vertex
			m_Vertices[vertexIndex] = vertex + vertexForce;
		}
	}
	
	private void FractureMeshVertices(Dictionary<int, float> movedVertexToForceMagnitudeMap, Vector3 impactForce)
	{
		// If we're not moving ALL of our vertices, then we need to store vertex information about the
		// vertices that we'll be changing for our new SubHull meshes.
		if (movedVertexToForceMagnitudeMap.Keys.Count >= m_Vertices.Count)
			return;

		// Set up our initial SubHulls. This divides our vertices between the two SubHulls, along with
		// all vertex information, and assigns triangles each SubHull
		SetupSubHulls(movedVertexToForceMagnitudeMap);
		
		// This goes through the SubHulls, finds all the edge vertices, and adds new vertices and triangles
		// to close up the open end of each SubHull mesh
		FixUpSubHulls(impactForce);
		
		// Set the first SubHull's mesh information into all our fields
		m_Vertices = m_FirstSubHull.m_Vertices;
		m_Normals = m_FirstSubHull.m_Normals;
		m_Tangents = m_FirstSubHull.m_Tangents;
		m_Uvs = m_FirstSubHull.m_Uvs;
		m_Triangles = m_FirstSubHull.m_Triangles;
		m_FirstSubHull = null;
	}
	
	#endregion Deformation Functions
	
	#region Helper Functions
	
	public void SetupSubHulls(Dictionary<int, float> movedVertexToForceMagnitudeMap)
	{
		// Create new SubHulls for us to work with
		m_FirstSubHull = new SubHull();
		m_SecondSubHull = new SubHull();
		
		// Add all the information about each vertex to the proper SubHull
		Dictionary<SubHull, Dictionary<int, int>> subHullToIndexMap = new Dictionary<SubHull, Dictionary<int, int>>();
		for (int i = 0; i < m_Vertices.Count; i++)
		{
			// Firgure out which SubHull we're adding this vertex to
			SubHull subHull = m_FirstSubHull;
			if (movedVertexToForceMagnitudeMap.ContainsKey(i))
				subHull = m_SecondSubHull;

			// Add this vertex information to the correct SubHull
			subHull.m_Vertices.Add(m_Vertices[i]);
			if (m_Normals.Count > i)
				subHull.m_Normals.Add(m_Normals[i]);
			if (m_Tangents.Count > i)
				subHull.m_Tangents.Add(m_Tangents[i]);
			if (m_Uvs.Count > i)
				subHull.m_Uvs.Add(m_Uvs[i]);
			
			// Make sure out maps are set up appropriately
			if (!subHullToIndexMap.ContainsKey(subHull))
				subHullToIndexMap.Add(subHull, new Dictionary<int, int>());
			
			subHullToIndexMap[subHull].Add(i, subHull.m_Vertices.Count - 1);
		}
		
		// TODO (micmah): I might eventually want to see how many triangles are split by the fracture, and
		//  if it's not enough, I could break those triangles down until I have a nicely triangulated edge...
		
		// Build vertex equivalence map. Sometimes multiple vertices occupy the exact same location, and we
		// need to be able to track that when we figure out which vertex positions share what triangles.
		Dictionary<Vector3, List<int>> vertexToIndicesMap = new Dictionary<Vector3, List<int>>();
		for (int i = 0; i < m_Vertices.Count; i++)
		{
			Vector3 vertex = m_Vertices[i];
			if (!vertexToIndicesMap.ContainsKey(vertex))
				vertexToIndicesMap.Add(vertex, new List<int>());
			
			List<int> vertexIndices = vertexToIndicesMap[vertex];
			vertexIndices.Add(i);
		}

		// Add each triangle to the proper SubHull
		for (int i = 0; i < m_Triangles.Count; i = i + 3)
		{
			// Count the number of vertices in this triangle that are being moved. This is used
			// to figure out what SubHull this triangle goes with
			List<int> movedVertexIndices = new List<int>();
			for (int triangleIndex = i; triangleIndex < i + 3; triangleIndex++)
			{
				if (movedVertexToForceMagnitudeMap.ContainsKey(m_Triangles[triangleIndex]))
					movedVertexIndices.Add(m_Triangles[triangleIndex]);
			}

			// This triangle has vertices in both SubHulls. Add these vertex indices
			// to m_EdgeVertexIndices in the proper SubHulls and continue on.
			if (movedVertexIndices.Count != 3 && movedVertexIndices.Count != 0)
			{
				for (int triangleIndex = i; triangleIndex < i + 3; triangleIndex++)
				{
					int vertexIndex = m_Triangles[triangleIndex];

					// Figure out which SubHull this vertex belongs to
					SubHull vertexSubHull = m_FirstSubHull;
					if (movedVertexIndices.Contains(vertexIndex))
						vertexSubHull = m_SecondSubHull;
					
					// Figure out the index of this vertex within its SubHull
					Dictionary<int, int> oldIndexToNewIndexMap = subHullToIndexMap[vertexSubHull];
					
					// Add this vertex as an edge vertex in our SubHull
					foreach (int index in vertexToIndicesMap[m_Vertices[vertexIndex]])
						vertexSubHull.m_EdgeVertexIndices.Add(oldIndexToNewIndexMap[index]);
				}

				continue;
			}
			
			// Figure out which SubHull this triangle is being added to
			SubHull subHull = m_FirstSubHull;
			if (movedVertexIndices.Count == 3)
				subHull = m_SecondSubHull;
			
			// Add this triangle to the right SubHull
			for (int triangleIndex = i; triangleIndex < i + 3; triangleIndex++)
			{
				int vertexIndex = m_Triangles[triangleIndex];
				Dictionary<int, int> oldIndexToNewIndexMap = subHullToIndexMap[subHull];
				if (oldIndexToNewIndexMap.ContainsKey(vertexIndex))
					subHull.m_Triangles.Add(oldIndexToNewIndexMap[vertexIndex]);
			}
		}
		
		// Have each SubHull setup edge information for use later on
		m_FirstSubHull.CalculateEdges();
		m_SecondSubHull.CalculateEdges();
	}
	
	public void FixUpSubHulls(Vector3 impactForce)
	{
		for (int i = 0; i < 2; i++)
		{
			// Figure out which SubHull we're fixing right now
			SubHull subHull;
			if (i == 0)
				subHull = m_FirstSubHull;
			else
				subHull = m_SecondSubHull;

			// Create central point by averaging all edge vertex information
			Vector3 vertex = Vector3.zero;
			Vector4 tangent = Vector4.zero;
			Vector2 uv = Vector2.zero;
			foreach (int edgeVertexIndex in subHull.m_EdgeVertexIndexToOtherEdgeVertexIndices.Keys)
			{
				vertex += subHull.m_Vertices[edgeVertexIndex];
				tangent += subHull.m_Tangents[edgeVertexIndex];
				uv += subHull.m_Uvs[edgeVertexIndex];
			}
			
			int edgeVertexCount = subHull.m_EdgeVertexIndexToOtherEdgeVertexIndices.Count;
			vertex /= edgeVertexCount;
			tangent /= edgeVertexCount;
			uv /= edgeVertexCount;
			
			// Add the new central point vertex information
			subHull.m_Vertices.Add(vertex);
			subHull.m_Tangents.Add(tangent);
			subHull.m_Uvs.Add(uv);
			
			// Create a normal based on the direction of the impactForce
			if (i == 0)
				subHull.m_Normals.Add(-impactForce.normalized);
			else
				subHull.m_Normals.Add(impactForce.normalized);
		
			// Make triangles with all edge vertices and a central point
			foreach (int edgeVertexIndex in subHull.m_EdgeVertexIndexToOtherEdgeVertexIndices.Keys)
			{
				List<int> edgeVertexIndices = subHull.m_EdgeVertexIndexToOtherEdgeVertexIndices[edgeVertexIndex];
				foreach (int vertexIndex in edgeVertexIndices)
				{
					subHull.m_Triangles.Add(subHull.m_Vertices.Count - 1);
					subHull.m_Triangles.Add(vertexIndex);
					subHull.m_Triangles.Add(edgeVertexIndex);
				}
				
				// TODO (micmah): The solution here duplicates lots of triangles... See if we can't fix that
			}
		}
	}
	
	#endregion Helper Functions
	
	#region Utility Functions
	
	private List<int> GetIntersectedTriangleIndices(Vector3 impactPoint, float impactRadius)
	{
		List<int> intersectedTriangles = new List<int>();
		for (int i = 0; i < m_Triangles.Count; i = i + 3)
		{
			if (IsTriangleIndexIntersected(i, impactPoint, impactRadius))
				intersectedTriangles.Add(i);	
		}
		
		return intersectedTriangles;
	}
	
	private bool IsTriangleIndexIntersected(int triangleIndex, Vector3 impactPoint, float impactRadius)
	{
		// Make sure we've got a good triangle index
		if (triangleIndex % 3 != 0)
		{
			Debug.LogError("Invalid Triangle index: " + triangleIndex + "  Must be a multiple of 3!");
			return false;
		}
		
		// Get the vectors for our triangle
		Vector3 A = m_Vertices[m_Triangles[triangleIndex]] - impactPoint;
		Vector3 B = m_Vertices[m_Triangles[triangleIndex + 1]] - impactPoint;
		Vector3 C = m_Vertices[m_Triangles[triangleIndex + 2]] - impactPoint;
		
		// Is the impact sphere outside the triangle plane?
		float rr = impactRadius * impactRadius;
		Vector3 V = Vector3.Cross(B - A, C - A);
		float d = Vector3.Dot(A, V);
		float e = Vector3.Dot(V, V);
		bool sep1 = d * d > rr * e;
		if (sep1)
			return false;
		
		// Is the impact sphere outside a triangle vertex?
		float aa = Vector3.Dot(A, A);
		float ab = Vector3.Dot(A, B);
		float ac = Vector3.Dot(A, C);
		float bb = Vector3.Dot(B, B);
		float bc = Vector3.Dot(B, C);
		float cc = Vector3.Dot(C, C);
		bool sep2 = (aa > rr) && (ab > aa) && (ac > aa);
		bool sep3 = (bb > rr) && (ab > bb) && (bc > bb);
		bool sep4 = (cc > rr) && (ac > cc) && (bc > cc);
		if (sep2 || sep3 || sep4)
			return false;
		
		// Is the impact sphere outside a triangle edge?
		Vector3 AB = B - A;
		Vector3 BC = C - B;
		Vector3 CA = A - C;
		float d1 = ab - aa;
		float d2 = bc - bb;
		float d3 = ac - cc;
		float e1 = Vector3.Dot(AB, AB);
		float e2 = Vector3.Dot(BC, BC);
		float e3 = Vector3.Dot(CA, CA);
		Vector3 Q1 = AB * e1 - d1 * AB;
		Vector3 Q2 = BC * e2 - d2 * BC;
		Vector3 Q3 = CA * e3 - d3 * CA;
		Vector3 QC = C * e1 - Q1;
		Vector3 QA = A * e2 - Q2;
		Vector3 QB = B * e3 - Q3;
		bool sep5 = (Vector3.Dot(Q1, Q1) > rr * e1 * e1) && (Vector3.Dot(Q1, QC) > 0);
		bool sep6 = (Vector3.Dot(Q2, Q2) > rr * e2 * e2) && (Vector3.Dot(Q2, QA) > 0);
		bool sep7 = (Vector3.Dot(Q3, Q3) > rr * e3 * e3) && (Vector3.Dot(Q3, QB) > 0);
		if (sep5 || sep6 || sep7)
			return false;
		
		// If we've gotten here, then this impact force DOES intersect this triangle.
		return true;
	}
	
	private List<int> BreakDownTriangle(int triangleIndex)
	{
		List<int> newTriangleIndices = new List<int>();
		newTriangleIndices.Add(triangleIndex);
		
		// If we have 64988 vertices or more, we can't add any more or we risk going over the
		// 65000 limit, which causes problems for unity.
		if (m_Vertices.Count > 64988)
			return newTriangleIndices;
		
		// Get the vertex indices and store them here
		int indexA = m_Triangles[triangleIndex];
		int indexB = m_Triangles[triangleIndex + 1];
		int indexC = m_Triangles[triangleIndex + 2];
		
		// Get the 3 vertices for this triangle
		Vector3 vertexA = m_Vertices[indexA];
		Vector3 vertexB = m_Vertices[indexB];
		Vector3 vertexC = m_Vertices[indexC];

		// Find the center points of this triangle sides. We'll be adding these as a new vertices.
		Vector3 centerAB = (vertexA + vertexB) / 2f;
		Vector3 centerAC = (vertexA + vertexC) / 2f;
		Vector3 centerBC = (vertexB + vertexC) / 2f;

		// Adjust the old triangle to use one of the new vertices
		m_Vertices.Add(centerAB);
		m_Vertices.Add(centerAC);
		m_Triangles[triangleIndex + 1] = m_Vertices.Count - 2;
		m_Triangles[triangleIndex + 2] = m_Vertices.Count - 1;
		
		// Add 3 new vertices for the other triangles
		m_Vertices.Add(centerAC);
		m_Vertices.Add(centerBC);
		m_Vertices.Add(vertexC);
		m_Triangles.Add(m_Vertices.Count - 2);
		m_Triangles.Add(m_Vertices.Count - 1);
		m_Triangles.Add(m_Vertices.Count - 3);
		newTriangleIndices.Add(m_Triangles.Count - 3);
		
		// Add 3 new vertices for the other triangles
		m_Vertices.Add(centerAB);
		m_Vertices.Add(centerBC);
		m_Vertices.Add(vertexB);
		m_Triangles.Add(m_Vertices.Count - 1);
		m_Triangles.Add(m_Vertices.Count - 2);
		m_Triangles.Add(m_Vertices.Count - 3);
		newTriangleIndices.Add(m_Triangles.Count - 3);
	
		// Add 3 new vertices for the other triangles
		m_Vertices.Add(centerAB);
		m_Vertices.Add(centerBC);
		m_Vertices.Add(centerAC);
		m_Triangles.Add(m_Vertices.Count - 2);
		m_Triangles.Add(m_Vertices.Count - 1);
		m_Triangles.Add(m_Vertices.Count - 3);
		newTriangleIndices.Add(m_Triangles.Count - 3);
		
		// Add new normals. These MUST be added in the same order as the vertices above!
		if (m_Normals.Count > 0)
		{
			Vector3 normalA = m_Normals[indexA];
			Vector3 normalB = m_Normals[indexB];
			Vector3 normalC = m_Normals[indexC];
			Vector3 normalAB = (normalA + normalB) / 2;
			Vector3 normalAC = (normalA + normalC) / 2;
			Vector3 normalBC = (normalB + normalC) / 2;
			
			m_Normals.Add(normalAB);
			m_Normals.Add(normalAC);
			m_Normals.Add(normalAC);
			m_Normals.Add(normalBC);
			m_Normals.Add(normalC);
			m_Normals.Add(normalAB);
			m_Normals.Add(normalBC);
			m_Normals.Add(normalB);
			m_Normals.Add(normalAB);
			m_Normals.Add(normalBC);
			m_Normals.Add(normalAC);
		}
		
		// Add new tangents. These MUST be added in the same order as the vertices above!
		if (m_Tangents.Count > 0)
		{
			Vector4 tangentA = m_Tangents[indexA];
			Vector4 tangentB = m_Tangents[indexB];
			Vector4 tangentC = m_Tangents[indexC];
			Vector4 tangentAB = (tangentA + tangentB) / 2;
			Vector4 tangentAC = (tangentA + tangentC) / 2;
			Vector4 tangentBC = (tangentB + tangentC) / 2;
			
			m_Tangents.Add(tangentAB);
			m_Tangents.Add(tangentAC);
			m_Tangents.Add(tangentAC);
			m_Tangents.Add(tangentBC);
			m_Tangents.Add(tangentC);
			m_Tangents.Add(tangentAB);
			m_Tangents.Add(tangentBC);
			m_Tangents.Add(tangentB);
			m_Tangents.Add(tangentAB);
			m_Tangents.Add(tangentBC);
			m_Tangents.Add(tangentAC);
		}
		
		// Add new UVs. These MUST be added in the same order as the vertices above!
		if (m_Uvs.Count > 0)
		{
			Vector2 uvA = m_Uvs[indexA];
			Vector2 uvB = m_Uvs[indexB];
			Vector2 uvC = m_Uvs[indexC];
			Vector2 uvAB = (uvA + uvB) / 2;
			Vector2 uvAC = (uvA + uvC) / 2;
			Vector2 uvBC = (uvB + uvC) / 2;
			
			m_Uvs.Add(uvAB);
			m_Uvs.Add(uvAC);
			m_Uvs.Add(uvAC);
			m_Uvs.Add(uvBC);
			m_Uvs.Add(uvC);
			m_Uvs.Add(uvAB);
			m_Uvs.Add(uvBC);
			m_Uvs.Add(uvB);
			m_Uvs.Add(uvAB);
			m_Uvs.Add(uvBC);
			m_Uvs.Add(uvAC);
		}
		
		return newTriangleIndices;
	}
	
	private float GetAreaOfTriangle(int triangleIndex)
	{
		// Get the vertices of the triangle
		Vector3 vertexA = m_Vertices[m_Triangles[triangleIndex]];
		Vector3 vertexB = m_Vertices[m_Triangles[triangleIndex + 1]];
		Vector3 vertexC = m_Vertices[m_Triangles[triangleIndex + 2]];
		
		// Figure out the area of the triangle
		Vector3 v = Vector3.Cross(vertexA - vertexB, vertexA - vertexC);
		return v.magnitude * 0.5f;
	}
	
	private int GetIndexOfLargestTriangle(Dictionary<int, float> triangleIndexToTriangleArea)
	{
		int indexOfLargestTriangle = 0;
		float areaOfLargestTriangle = 0f;
		foreach (int triangleIndex in triangleIndexToTriangleArea.Keys)
		{
			float areaOfCurrentTriangle = triangleIndexToTriangleArea[triangleIndex];
			if (areaOfCurrentTriangle > areaOfLargestTriangle)
			{
				indexOfLargestTriangle = triangleIndex;
				areaOfLargestTriangle = areaOfCurrentTriangle;
			}
		}
		
		return indexOfLargestTriangle;
	}
	
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
	
	public Mesh GetSubHullMesh()
	{
		if (m_SecondSubHull != null && !m_SecondSubHull.IsEmpty())
			return m_SecondSubHull.GetMesh();
		
		return null;
	}
	
	#endregion Utility Functions
}
