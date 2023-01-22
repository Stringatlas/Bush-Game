using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySpin : MonoBehaviour
{
	[SerializeField] float spinSpeed = 360;

	public float x;
	public float y;
	public float z;
	bool isVisible;

	float smoothTime = 0;

	private void OnBecameInvisible()
	{
		isVisible = false;
	}

	private void OnBecameVisible()
	{
		isVisible = true;
	}

	private void Start()
	{
		//smoothTime = 180f / spinSpeed;
	}

	private void Update()
	{
		if (isVisible)
		{
			float yRotation = Mathf.SmoothStep(transform.rotation.y, y * spinSpeed, smoothTime);
			//float xRotation = Mathf.SmoothStep(transform.rotation.x, x * spinSpeed, smoothTime);
			//float zRotation = Mathf.SmoothStep(transform.rotation.z, z * spinSpeed, smoothTime);
			transform.Rotate(new Vector3(0, yRotation, 0));
			smoothTime += Time.deltaTime;

			transform.position = transform.position + (Vector3.up * 0.002f * Mathf.Sin(Time.unscaledTime * 3f));
		}
	}
}
