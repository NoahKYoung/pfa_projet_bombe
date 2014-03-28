using UnityEngine;
using System.Collections;

public class PFA_TPControl : MonoBehaviour 
{
	// Public informations
	public Camera _refCam;
	public Transform playergraphic;
	public GameObject zone;
	public GameObject ground;
	
	public float _speed = 0.2f; // Vitesse
	public float _runningSpeed = 0.4f; // Vitesse en sprint
	public float _sidewaySpeedDivider = 0.75f;
	public float _reversinSpeedDivider = 0.5f;
	public float _rotSpeed = 3.5f;
	public float _sidewayRotSpeed = 4.5f;
	public float _reversinRotSpeed = 6f;
	
	// Movement vars
	Vector2 stickInput;
	float deadzone = 0.25f;
	
	// Game states
	bool _canJump = true;
	bool _sprinting = false;
	bool showzone = false;
	
	// Use this for initialization
	void Start ()
	{
		// Lol.
	}
	
	// Update is called once per frame
	void Update () 
	{
		CheckInputs();
		MoveCharacter();
	}
	
	void OnCollisionEnter(Collision col)
	{
		if(col.transform.tag == "Ground")
		{
			Debug.Log ("CAN JUMP !");
			_canJump = true;
		}
	}
	
	void CheckInputs()
	{
		// Movement Inputs		
		stickInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		modifyInputs();
		
		//Debug.Log ("H = " + stickInput.x);
		//Debug.Log ("V = " + stickInput.y);
		
		// Control inputs
		// Jump inputs
		if (Input.GetKeyDown(KeyCode.JoystickButton0) && _canJump)
		{
			Debug.Log("Jump");
			_canJump = false;
			
			// ajouter linéairement pour etre compensé avec la gravité
			this.transform.Translate(this.transform.up);
		}
		// Sprint input
		if (Input.GetKeyDown(KeyCode.JoystickButton2))
		{
			Debug.Log("Sprint");
			_sprinting = true;			
		}
		
		if (Input.GetKeyUp (KeyCode.JoystickButton2))
		{
			Debug.Log ("Stopped Sprinting");
			_sprinting = false;
		}
		
		// Shout input
		if (Input.GetKeyDown(KeyCode.JoystickButton3))
		{
			Debug.Log("Shout");	
		}	
		
		// Roll-up input
		if (Input.GetKey(KeyCode.JoystickButton1))
		{
			Debug.Log("Roll-up");
		}
		// Explosion zone input
		if (Input.GetKey(KeyCode.Joystick1Button5))
		{
			zonevisibility();
		}
	}
	
	void modifyInputs()
	{
		if(stickInput.x < deadzone && stickInput.x > -deadzone)
		{
			stickInput.x = 0;
		}
		
		if(stickInput.y < deadzone && stickInput.y > -deadzone)
		{
			stickInput.y = 0;
		}
		
		if(stickInput.x > 0)
		{
			stickInput.x = 1;
		}
		else if(stickInput.x < 0)
		{
			stickInput.x = -1;
		}
		
		if(stickInput.y > 0)
		{
			stickInput.y = 1;
		}
		else if(stickInput.y < 0)
		{
			stickInput.y = -1;
		}
	}
	
	void MoveCharacter()
	{		
		// Is sprinting ? Check speed
		float currentSpeed = 0f;
		
		if(!_sprinting)
		{
			currentSpeed = _speed;
		}
		else if (_sprinting)
		{
			currentSpeed = _runningSpeed;
		}		
		
		// Player movement
		Vector3 modifiedDirRight = _refCam.transform.right;
		modifiedDirRight.y = 0;
		
		Vector3 modifiedDirForward = _refCam.transform.forward;
		modifiedDirForward.y = 0;
		
		Vector3 xTranslate = modifiedDirRight * stickInput.x * currentSpeed;
		Vector3 yTranslate = modifiedDirForward * stickInput.y * currentSpeed;
		Vector3 composedTranslate = Vector3.Lerp(xTranslate, yTranslate, 0.5f);
		
		if(composedTranslate.magnitude > currentSpeed)
		{
			composedTranslate.x /= 2;
			composedTranslate.y /= 2;
		}
		
		this.transform.Translate(composedTranslate);
		
		//Player graphic rotation
		
		if (composedTranslate != Vector3.zero)
		{
			Quaternion newRotation = Quaternion.LookRotation(composedTranslate);
			playergraphic.transform.rotation = Quaternion.Slerp(playergraphic.transform.rotation, newRotation, Time.deltaTime * 8);
		}
		
	}
	//Explosion zone

	void zonevisibility()
	{
		showzone = !showzone;
		Debug.Log("showzone");
		if (showzone)
		{
		Object.Instantiate(zone, new Vector3(gameObject.transform.position.x, ground.transform.position.y, gameObject.transform.position.z), gameObject.transform.rotation);
		GameObject.Find("Zone(Clone)").transform.position = new Vector3(gameObject.transform.position.x, ground.transform.position.y, gameObject.transform.position.z);
		}
		else
		{
		Debug.Log("false");
		Destroy(GameObject.Find("Zone(Clone)"), 0);
		}
	}
}
