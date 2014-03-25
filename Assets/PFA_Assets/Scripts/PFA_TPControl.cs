using UnityEngine;
using System.Collections;

public class PFA_TPControl : MonoBehaviour 
{
	// Public informations
	public Camera _refCam;
	public Transform playergraphic;
	
	public float _speed = 0.2f; // Vitesse
	public float _runningSpeed = 0.4f; // Vitesse en sprint
	
	public float _rangeExpAngle = 40f;
	public float _rangeRunExpAngle = 65f;
	
	public float _maxStunCount = 2.5f;
	
	public float _maxKickCount = 0.25f;
	
	// Movement vars
	private Vector2 stickInput;
	private float deadzone = 0.25f;
	
	// Game states
	private bool _canJump = true;
	
	private bool _sprinting = false;
	
	private bool _stunned = false;
	private float _stunCount = 0;
	
	private bool _kickback = false;
	private float _kickCount = 0;
	private Vector3 _kickbackDirection;
	
	// Use this for initialization
	void Start ()
	{
		// Lol.
	}
	
	// Update is called once per frame
	void Update () 
	{
		CheckStates();
		
		if(!_stunned)
		{
			CheckInputs();
			MoveCharacter();
		}
	}
	
	void OnCollisionEnter(Collision col)
	{
		if(col.transform.tag == "Ground")
		{
			Debug.Log ("CAN JUMP !");
			_canJump = true;
		}
		
		if(col.transform.tag == "Wall")
		{
			float collisionAngle = Vector3.Angle(col.contacts[0].normal, -this.playergraphic.forward);
			
			if(_sprinting)
			{
				if(collisionAngle < _rangeRunExpAngle)
				{
					Debug.Log ("BOOOOM");
				}
				else
				{
					stunPlayer();
					projectBack();
				}
			}
			else
			{
				if(collisionAngle < _rangeExpAngle)
				{
					Debug.Log ("BOOOOM");
				}
				else
				{
					stunPlayer();
					projectBack();
				}
			}
		}
	}
	
	// Verify any states actual state
	void CheckStates()
	{
		if(_stunned)
		{
			_stunCount += Time.deltaTime;
			
			if(_stunCount >= _maxStunCount)
			{
				_stunned = false;
				_stunCount = 0;
			}
		}
		
		if(_kickback)
		{
			_kickCount += Time.deltaTime;
			
			if(_kickCount >= _maxKickCount)
			{
				_kickback = false;
				_kickCount = 0;
			}
			else
			{
				this.transform.Translate(_kickbackDirection * 0.25f);
			}
		}
	}
	
	// Set stun variables
	void stunPlayer()
	{
		_stunned = true;
		_stunCount = 0;
	}
	
	// Projects player backwards
	void projectBack()
	{
		_kickback = true;
		_kickbackDirection = -this.playergraphic.forward;
		_kickCount = 0;
	}
	
	// Verify inputs
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
	}
	
	// Modify inputs for unique speed
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
	
	// Move and rotate character accordingly
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
}
