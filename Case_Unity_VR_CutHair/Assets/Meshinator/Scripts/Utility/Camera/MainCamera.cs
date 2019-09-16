using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour
{
	public Transform target;
	public double distance = 50;
	float scrollSpeed = 3;
	float panSpeed = 0.5f;
	
	double xSpeed = 250.0;
	double ySpeed = 120.0;
	
	double yMinLimit = -89;
	double yMaxLimit = 89;
	
	double minZoom = 1;
	double maxZoom = 100;
	
	private double x = 0.0;
	private double y = 0.0;

	// Use this for initialization
	void Start ()
	{
		Vector3 angles = transform.eulerAngles;
	    x = angles.y;
	    y = angles.x;
	
		// Make the rigid body not change rotation
	   	if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
		
		// Set clipping distances for 3D objects to save rendering time
		float[] distances = new float[32];
		gameObject.GetComponent<Camera>().layerCullDistances = distances;
	}
	
	void FixedUpdate()
	{
		// Figure out what direction the camera is looking, so we can using arrowkeys or
		// WASD to move it around correctly
		Vector3 direction = target.position - transform.position;
		direction.y = 0;
		direction.Normalize();
		Vector3 rightAngleDirection = new Vector3(
			direction.x * Mathf.Cos(Mathf.PI/2) - direction.z * Mathf.Sin(Mathf.PI/2),
			0,
			direction.x * Mathf.Sin(Mathf.PI/2) - direction.z * Mathf.Cos(Mathf.PI/2));
		
		// Move the camera around with arrow keys or WASD
		int panFactor = (int)Mathf.Max(1.0f, (panSpeed * Mathf.Max(1.0f, (float)(distance / 10))));
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
		{
			target.position += direction * panFactor;
		}
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
		{
			target.position += rightAngleDirection * panFactor;
		}
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
		{
			target.position -= direction * panFactor;
		}
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
		{
			target.position -= rightAngleDirection * panFactor;
		}
	}
	
	void LateUpdate ()
	{
		// Rotate the camera when the right mouse button is down
	    if (target && Input.GetMouseButton(1))
		{
	        x += Input.GetAxis("Mouse X") * xSpeed * 0.02;
	        y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02;
	 		y = ClampAngle(y, yMinLimit, yMaxLimit);
		}

		// Zoom the camera in and out
		distance -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
		if (distance < minZoom)
			distance = minZoom;
		else if (distance > maxZoom)
			distance = maxZoom;
		
//		// Don't let the camera fall below the terrain
//		OffsetCameraFromTerrain();

		// Position the camera appropriately all the time
		Quaternion rotation = Quaternion.Euler((float)y, (float)x, 0.0f);
	    Vector3 position = rotation * new Vector3(0.0f, 0.0f, (float)-distance) + target.position;
	        
	    transform.rotation = rotation;
	    transform.position = position;
    }
	
	static double ClampAngle(double angle, double min, double max)
	{
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp ((float)angle, (float)min, (float)max);
	}
	
//	void OffsetCameraFromTerrain()
//	{
//		float distance = 200;
//		Vector3 position = new Vector3(this.transform.position.x, 100, this.transform.position.z);
//		
//		Ray ray = new Ray(position, Vector3.down);
//		RaycastHit[] hits = Physics.RaycastAll(ray, distance);
//		if (hits.Length > 0)
//		{
//			RaycastHit terrainHit = hits[hits.Length - 1];
//			target.transform.position = new Vector3(
//												target.transform.position.x,
//												100 - terrainHit.distance,
//												target.transform.position.z);
//		}
//	}
	
	public void FocusCameraOnTransform(Transform transform, float panDuration)
	{
		// Lerp the camera focal point to the passed-in transform
		StartCoroutine(CameraLerpCoroutine(transform.position, Time.timeSinceLevelLoad, panDuration));
	}
	
	private IEnumerator CameraLerpCoroutine(Vector3 endPosition, float startTime, float panDuration)
	{
		Vector3 startPosition = new Vector3(target.position.x, target.position.y, target.position.z);
		
		float percentComplete = 0f;
		while ((percentComplete = (Time.timeSinceLevelLoad - startTime) / panDuration) < 1)
		{
			// Lerp the camera focal point to the passed-in transform
			target.position = Vector3.Slerp(startPosition, endPosition, percentComplete);
			yield return null;
		}
		
		// Set the final position
		target.position = endPosition;
	}
}
