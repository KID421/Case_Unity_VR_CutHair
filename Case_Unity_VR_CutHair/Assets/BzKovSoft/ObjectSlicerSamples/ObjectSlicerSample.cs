using BzKovSoft.ObjectSlicer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BzKovSoft.ObjectSlicerSamples
{
	public class ObjectSlicerSample : BzSliceableObjectBase
	{
		protected override BzSliceTryData PrepareData(Plane plane)
		{
			// remember some data. Later we could use it after the slice is done.
			// here I add Stopwatch object to see how much time it takes
			// and vertex count to display.
			ResultData addData = new ResultData();

			// count vertices
			var filters = GetComponentsInChildren<MeshFilter>();
			for (int i = 0; i < filters.Length; i++)
			{
				addData.vertexCount += filters[i].sharedMesh.vertexCount;
			}

			// remember start time
			addData.stopwatch = Stopwatch.StartNew();

			// colliders that will be participating in slicing
			var colliders = gameObject.GetComponentsInChildren<Collider>();

			// return data
			return new BzSliceTryData()
			{
				// componentManager: this class will manage components on sliced objects
				componentManager = new StaticComponentManager(gameObject, plane, colliders),
				plane = plane,
				addData = addData,
			};
		}

		protected override void OnSliceFinished(BzSliceTryResult result)
		{
			if (!result.sliced)
				return;

			// on sliced, get data that we saved in 'PrepareData' method
			var addData = (ResultData)result.addData;
			addData.stopwatch.Stop();
            /*
            drawText += gameObject.name +
				". VertCount: " + addData.vertexCount.ToString() + ". ms: " +
				addData.stopwatch.ElapsedMilliseconds.ToString() + Environment.NewLine;

			if (drawText.Length > 1500) // prevent very long text
				drawText = drawText.Substring(drawText.Length - 1000, 1000);
            */
		}

		static string drawText = string.Empty;

		void OnGUI()
		{
			GUI.Label(new Rect(10, 10, 2000, 2000), drawText);
		}

		// DTO that we pass to slicer and then receive back
		class ResultData
		{
			public int vertexCount;
			public Stopwatch stopwatch;
		}
	}
}
