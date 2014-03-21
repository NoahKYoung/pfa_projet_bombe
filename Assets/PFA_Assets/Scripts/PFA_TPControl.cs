using UnityEngine;
using System.Collections;

public class PFA_TPControl : MonoBehaviour 
{
	// Public informations
	public float _speed = 0.2f; // Vitesse
	public float _runningSpeed = 0.4f; // Vitesse en sprint
	public float _sidewaySpeedDivider = 0.75f;
	public float _reversinSpeedDivider = 0.5f;
	public float _rotSpeed = 3.5f;
	public float _sidewayRotSpeed = 4.5f;
	public float _reversinRotSpeed = 6f;
	
	// Movement vars
	Vector2 stickInput;
	float deadzone = 0.15f;
	
	// Game states
	bool _canJump = true;
	bool _sprinting = false;
	
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
	}
	
	void modifyInputs()
	{
		if(stickInput.magnitude < deadzone)
		{
			stickInput = Vector2.zero;
		}
		
		if(stickInput.y > deadzone)
		{
			stickInput.y = 1;
		}
		else if (stickInput.y < -deadzone)
		{
			stickInput.y = -1; // gérer ca
		}
		else
		{
			stickInput.y = 0;
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
		
		
		//Rotate
		if(stickInput.y == -1)
		{
			if(stickInput.x != 0)
			{
				this.transform.Rotate(stickInput.y * -(stickInput.x/(Mathf.Abs(stickInput.x))) * this.transform.up * _reversinRotSpeed);
			}
			else
			{
				this.transform.Rotate(stickInput.y * this.transform.up * _reversinRotSpeed);
			}
		}
		else if(stickInput.y == 0)
		{
			this.transform.Rotate(stickInput.x * this.transform.up * _rotSpeed);
		}
		else
		{
			this.transform.Rotate(stickInput.x * this.transform.up * _rotSpeed);
		}
	
		// Movement
		if(stickInput.y == -1)
		{
			this.transform.Translate(0, 0, Mathf.Abs(stickInput.y) * currentSpeed/_reversinSpeedDivider);
		}
		else if(stickInput.y == 0)
		{
			this.transform.Translate(0, 0, Mathf.Abs(stickInput.x) * currentSpeed/_sidewaySpeedDivider);
		}
		else
		{
			this.transform.Translate(0, 0, stickInput.y * currentSpeed);
		}
	}
}
