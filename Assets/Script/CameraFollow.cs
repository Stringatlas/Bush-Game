using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraFollow : MonoBehaviour
{
	public Transform target;
	public float normalFov;
	public float lerpSpeed = 1.0f;

	private Vector3 offset;

	private Vector3 targetPos;
	Camera cam;

	private void Awake()
	{
		cam = GetComponent<Camera>();
		cam.orthographicSize = normalFov;
	}
	private void Start()
	{
		offset = transform.position - target.position;
	}

	private void Update()
	{
		targetPos = target.position + offset;
		transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
	}

	public void SetFov(float fov)
	{
		cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, fov, lerpSpeed * Time.deltaTime);
	}
}

