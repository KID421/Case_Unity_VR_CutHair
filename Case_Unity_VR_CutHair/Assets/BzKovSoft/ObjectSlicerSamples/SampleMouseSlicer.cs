using UnityEngine;
using BzKovSoft.ObjectSlicer;
using System.Diagnostics;
using System;

namespace BzKovSoft.ObjectSlicerSamples
{
	/// <summary>
	/// Test class for demonstration purpose
	/// </summary>
	public class SampleMouseSlicer : MonoBehaviour
	{
		int _sliceId = 0;
		void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				// if left mouse clicked, try slice this object

				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit[] hits = Physics.RaycastAll(ray, 100f);

				++_sliceId;

				for (int i = 0; i < hits.Length; i++)
				{
					IBzSliceable sliceable = hits[i].transform.GetComponentInParent<IBzSliceable>();
					IBzSliceableAsync sliceableA = hits[i].transform.GetComponentInParent<IBzSliceableAsync>();

					Vector3 direction = Vector3.Cross(ray.direction, Camera.main.transform.right);
					Plane plane = new Plane(direction, ray.origin);

					if (sliceable != null)
						sliceable.Slice(plane);

					if (sliceableA != null)
						sliceableA.Slice(plane, _sliceId, null);
				}
			}
		}
	}
}