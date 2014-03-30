using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PFA_TPControl : MonoBehaviour 
{
	// Public informations
	public Camera _refCam;
	public Transform playergraphic;
	
	public float _speed = 6f; // Vitesse
	public float _runningSpeed = 12f; // Vitesse en sprint
	
	public float _rangeExpAngle = 40f;
	public float _rangeRunExpAngle = 65f;
	
	public float _maxStunCount = 1.5f;
	
	public float _kickbackSpeed = 0.2f;
	public float _kickbackDistance = 2f;
	
	public float _screamRadius = 20f;
	
	// Movement vars
	private Vector2 stickInput;
	private float deadzone = 0.25f;
	
	// Game states
	private bool _canJump = true;
	
	private bool _sprinting = false;
	
	private bool _stunned = false;
	private float _stunCount = 0;
	
	private bool _kickback = false;
	private Vector3 _kickbackDirection;
	private Vector3 _originalPos;
	
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
					_originalPos = transform.position;
					stunPlayer();
					projectBack();
				}
			}
		}
		
		if(col.transform.tag == "Civil")
		{
			_originalPos = transform.position;
			stunPlayer();
			projectBack();
		}
	}
	
	// Verify any states actual state
	void CheckStates()
	{
		if(_stunned)
		{
			if(_stunCount >= _maxStunCount)
			{
				_stunned = false;
				_stunCount = 0;
			}
			
			_stunCount += Time.deltaTime;
		}
		
		if(_kickback)
		{
			if(Vector3.Distance(_originalPos, transform.position) < _kickbackDistance)
			{
				this.transform.Translate(_kickbackDirection * _kickbackSpeed * Time.deltaTime);
			}
			else
			{
				Debug.Log(Vector3.Distance(_originalPos, transform.position));
				_kickback = false;
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
		_kickbackDirection = Vector3.Normalize(_kickbackDirection);
	}
	
	// Verify inputs
	void CheckInputs()
	{
		// Movement Inputs		
		stickInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		modifyInputs();
		
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
			Shout();
		}	
		
		// Roll-up input
		if (Input.GetKey(KeyCode.JoystickButton1))
		{
			Debug.Log("Roll-up");
		}
	}
	
	void Shout()
	{
		GameObject[] civilList =  GameObject.FindGameObjectsWithTag("Civil");
		int i = 0;
		
		for(i = 0; i < civilList.Length; i++)
		{
			(civilList[i].GetComponent("PFA_Civil") as PFA_Civil).ScreamedAt();
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
		
		Vector3 xTranslate = modifiedDirRight * stickInput.x;
		Vector3 yTranslate = modifiedDirForward * stickInput.y;
		Vector3 composedTranslate = Vector3.Lerp(xTranslate, yTranslate, 0.5f);
		
		composedTranslate = Vector3.Normalize(composedTranslate);
		this.transform.Translate(composedTranslate * Time.deltaTime * currentSpeed);
		
		
		//Player graphic rotation
		if (composedTranslate != Vector3.zero)
		{
			Quaternion newRotation = Quaternion.LookRotation(composedTranslate);
			playergraphic.rotation = Quaternion.Slerp(playergraphic.rotation, newRotation, Time.deltaTime * 8);
		}
	}
}
