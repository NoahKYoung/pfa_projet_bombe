using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PFA_TPControl : MonoBehaviour 
{
	// Public informations
	public Camera _refCam;
	public Transform playergraphic;
	public GameObject zone;
	public Transform _refspawn;
	
	public float _speed = 6f; // Vitesse
	public float _runningSpeed = 12f; // Vitesse en sprint
	
	public float _rangeExpAngle = 40f;
	public float _rangeRunExpAngle = 65f;
	
	public float _maxStunCount = 1.5f;
	
	public float _kickbackSpeed = 0.2f;
	public float _kickbackDistance = 2f;
	
	public float _screamRadius = 20f;
	
	// Explosion management
	public bool explosionON = false;
	public bool controlenabled = true;
	public float explosionRadius = 5f;
	public GameObject Explosion;
	
	//SprintTimer vars
	public float SprintTime;
	
	public float RecoverSprintTime;
	bool _enableSprint = true;
	
	// Movement vars
	private Vector2 stickInput;
	private float deadzone = 0.25f;
	
	// Game states
	public bool _canJump = true;
	private bool _sprinting = false;
	private bool showzone = false;
	private bool _leftTriggerPressed = false;
	
	private bool _isSpherical = false;
	
	private bool _stunned = false;
	private float _stunCount = 0;
	
	private bool _kickback = false;
	private Vector3 _kickbackDirection;
	private Vector3 _originalPos;
	
	// Use this for initialization
	void Start ()
	{
		Transform Bomb = transform.Find("BOMB");
		Bomb.rigidbody.isKinematic = true;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(!_isSpherical)
		{
			CheckStates();
			
			if(!_stunned && controlenabled)
			{
				CheckInputs();
				MoveCharacter();
				RecoverSprintControl();
			}
			
			ManageExpZone();
		}
		else
		{
			SphericalUpdate();
		}		
	}
	
	void ManageExpZone()
	{
		if(showzone)
		{
			Vector3 pos = new Vector3(gameObject.transform.position.x, (gameObject.transform.position.y - collider.bounds.extents.y * 0.99f), gameObject.transform.position.z);
			GameObject.Find("Zone(Clone)").transform.position = pos;
		}
	}
	
	void SphericalUpdate()
	{
		ResetCharPos();
		CheckSphericalInputs();
	}
	
	void RecoverSprintControl()
	{
		if(_enableSprint == false)
		{
			RecoverSprintTime -=Time.deltaTime;
			
			if(RecoverSprintTime <=0)
			{
				RecoverSprintTime = 0;
				SprintTime = 5;
				_enableSprint = true;

			}
		}
	}
	
	void ResetCharPos()
	{
		transform.position = transform.Find("BOMB").position;
		transform.Find("BOMB").position = transform.position;
	}
	
	void CheckSphericalInputs()
	{
		if(Input.GetKeyDown(KeyCode.JoystickButton1))
		{
			RollUp(false);
		}
	}
	
	void OnCollisionEnter(Collision col)
	{
		if(col.transform.tag == "Ground")
		{
			_canJump = true;
		}
		
		if(col.transform.tag == "Wall")
		{
			float collisionAngle = Vector3.Angle(col.contacts[0].normal, -this.playergraphic.forward);
			
			if(_sprinting)
			{
				if(collisionAngle < _rangeRunExpAngle)
				{
					Explode();
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
					Explode();
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
				rigidbody.MovePosition(transform.position + (_kickbackDirection * _kickbackSpeed * Time.deltaTime));
				//this.transform.Translate(_kickbackDirection * _kickbackSpeed * Time.deltaTime);
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
		if(controlenabled == true)
		{
			stickInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
			modifyInputs();
		}
		else
		{
			stickInput = Vector2.zero;
		}
		
		// Control inputs
		// Jump inputs
		if (Input.GetButtonDown("360_AButton") && _canJump && _sprinting == false)
		{
			Debug.Log("Jump");
			_canJump = false;
			
			rigidbody.AddForce(new Vector3(0, 6, 0), ForceMode.Impulse);
		}
		
		//Sprint/ Jump
		if(_enableSprint == true && _canJump == true && _sprinting == true && Input.GetButtonDown("360_AButton"))
		{
			Debug.Log("JumpSprint");
			_canJump = false;
			
			rigidbody.AddForce(new Vector3(0, 7, 0), ForceMode.Impulse);
		}
		
		// Sprint input
		if (Input.GetButton("360_XButton") && _enableSprint == true)
		{
			Debug.Log("Sprint");
			_sprinting = true;
			RecoverSprintTime = 5;
			
			SprintTime -= Time.deltaTime;
		
			if (SprintTime <= 0)
			{
				SprintTime = 0;
				_sprinting = false;
				_enableSprint = false;
				Debug.Log("NoSprint");
			}
		}
		
		if (Input.GetButtonUp("360_XButton"))
		{
			Debug.Log ("Stopped Sprinting");
			_sprinting = false;
		}
		
		// Shout input
		if (Input.GetButtonDown("360_YButton"))
		{
			Shout();
			//transform.Find();
		}
		
		// Roll-up input
		if (Input.GetButtonDown("360_BButton"))
		{
			RollUp(true);
		}
		
		// Explosion
		if(Input.GetAxis("360_Triggers") > 0)
		{
			Explode();
		}
		
		// Explosion zone
		if (Input.GetAxis("360_Triggers") < 0)
		{
			if(!_leftTriggerPressed)
			{
				_leftTriggerPressed = true;
				zonevisibility();
			}
		}
		
		if (Input.GetAxis("360_Triggers") == 0)
		{
			_leftTriggerPressed = false;
		}
	}
	
	public void Explode()
	{
		if(explosionON == false)
		{
			//Destroy(gameObject);
			Instantiate(Explosion, transform.position, transform.rotation);
			GameObject.Find("Explosion(Clone)").transform.localScale = new Vector3(explosionRadius, explosionRadius, explosionRadius);
			GameObject.Find("Explosion(Clone)").transform.position = transform.position;
			controlenabled = false;
		}
		
		explosionON = true;
	}
	
	void RollUp(bool spherical)
	{
		_isSpherical = spherical;
		
		if(_isSpherical && showzone)
		{
			showzone = false;
			Destroy(GameObject.Find("Zone(Clone)"), 0);
		}
		
		rigidbody.isKinematic = spherical;
		collider.enabled = !spherical;
		transform.Find("GraphPlayer").Find("perso_mesh").Find("MESH").Find("arttoy:TOY1").renderer.enabled = !spherical;
		
		Transform Bomb = transform.Find("BOMB");
		Bomb.rigidbody.isKinematic = !spherical;
		Bomb.collider.enabled = spherical;
		Bomb.renderer.enabled = spherical;
		
		Bomb.position = transform.position;
		
		if(_isSpherical)
		{
			float _currentSpeed = _speed;
			
			if(_sprinting) 
			{ 
				_currentSpeed = _runningSpeed; 
			}
			
			float isMoving = 0f;
			
			if(stickInput != Vector2.zero)
			{
				isMoving = 1f;
			}
			
			Bomb.rigidbody.velocity = rigidbody.velocity + (playergraphic.forward * _currentSpeed * isMoving);
			
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
		if(stickInput != Vector2.zero)
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
			composedTranslate.y = 0;
			
			rigidbody.MovePosition(transform.position + (composedTranslate * Time.deltaTime * currentSpeed));
			//rigidbody.AddForce(composedTranslate, ForceMode.Force);
			//this.transform.Translate(composedTranslate * Time.deltaTime * currentSpeed);
			
			//Player graphic rotation
			if (composedTranslate != Vector3.zero)
			{
				Quaternion newRotation = Quaternion.LookRotation(composedTranslate);
				playergraphic.rotation = Quaternion.Slerp(playergraphic.rotation, newRotation, Time.deltaTime * 8);
			}
		}
	}
	
	//Explosion zone
	void zonevisibility()
	{
		showzone = !showzone;
		
		if (showzone)
		{
			
			Vector3 pos = new Vector3(gameObject.transform.position.x, (gameObject.transform.position.y - collider.bounds.extents.y * 0.99f), gameObject.transform.position.z);
			
			Object.Instantiate(zone, pos, gameObject.transform.rotation);
			GameObject.Find("Zone(Clone)").transform.localScale = new Vector3(explosionRadius, 0.01f, explosionRadius);
			GameObject.Find("Zone(Clone)").transform.position = pos;
		}
		else
		{
			Destroy(GameObject.Find("Zone(Clone)"), 0);
		}
	}
}
