using UnityEngine;
using System.Collections;

public class PFA_TPCamera : MonoBehaviour 
{
	// Camera
	public Transform cameraTransform;
	public Transform cameraTarget;
	public float rotSpeed = 5f;
	public float camDistance = 2.435931f;
	public float camZDistance = 2.068446f;
	public float camXRot = 26.06088f;
	
	// Controls
	Vector2 camStickInput;
	float deadzone = 0.15f;	
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		CheckCamInputs();
		CameraPositionUpdate();
		CameraMovement();
	}
	
	void CameraPositionUpdate()
	{
		float playerRot = transform.rotation.y;
		float newX = transform.position.x - camZDistance * Mathf.Cos (playerRot);
		float newY = cameraTransform.position.y;
		float newZ = camZDistance * Mathf.Sin(playerRot);
		
		cameraTransform.position = new Vector3(newX, newY, newZ);
		
		//cameraTransform.position = new Vector3(transform.position.x + offsetX, cameraTransform.position.y, transform.position.z + offsetZ);
		cameraTransform.LookAt(cameraTarget.position);
	}
	
	void CheckCamInputs()
	{
		// Movement Inputs
		camStickInput = new Vector2(Input.GetAxis("Horizontal2"), Input.GetAxis("Vertical2"));
		
		if(camStickInput.magnitude < deadzone)
		{
			camStickInput = Vector2.zero;
		}
		
		Debug.Log ("Hcam = " + camStickInput.x);
		Debug.Log ("Vcam = " + camStickInput.y);
	}
	
	void CameraMovement()
	{
		this.transform.Rotate(camStickInput.x * this.transform.up * rotSpeed);
		//cameraTransform.RotateAround(this.transform.position, this.transform.up, rotSpeed * camStickInput.x);
	}
}
