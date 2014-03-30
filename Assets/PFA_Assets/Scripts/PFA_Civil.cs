using UnityEngine;
using System.Collections;

public class PFA_Civil : MonoBehaviour 
{
	// Linear & Rand usage
	public GameObject _patrolReference;
	
	// Wander usage
	public float _wanderRadius = 15f;
	private Vector3 _wanderDir;
	private float _wanderCounter = 0f;
	private float _maxDirWanderTime = 2f;
	private float _anglesLimit = 90f;
	
	// Line usage
	private Vector3 _lineDir;
	private float _lineDirMod = 1f;
	
	// Flee usage
	public GameObject _playerRef;
	private Vector3 _fleeDir;
	private float _fleeAngleBounds = 45f;
	private float _fleeCounter = 0f;
	private float _maxDirFleeTime = 1f;
	
	//Behaviours
	public int _behaviourPattern = 0;
	private int _currentBehaviour = 0;
	
	enum  Behaviours
	{
		LinearMov = 0,
		RandMov = 1,
		Idle = 2,
		Fleeing = 3
	};
	
	// Speed
	public float _speed = 4f;
	
	// Times
	public float _maxIdleTime = 0f;
	public float _minIdleTime = 0f;
	
	public float _maxFleeTime = 0f;
	public float _minFleeTime = 0f;
	
	public float _maxLineTime = 0f;
	public float _minLineTime = 0f;
	
	public float _maxRandTime = 0f;
	public float _minRandTime = 0f;
	
	// Random weights
	public float _randIdle = 0.1f;
	public float _randFlee = 0.1f;
	public float _randLine = 0.1f;
	public float _randRand = 0.1f;
	
	// Global usages
	public Transform civilGraph;
	private float _counter = 0f;
	private Vector3 _originalPos;
	
	private delegate void RefFunc();
	
	void Start () 
	{
		InitBehaviour();
		_originalPos = this.transform.position;
	}
	
	//Public
	public void ScreamedAt()
	{
		float screamRad = (_playerRef.GetComponent("PFA_TPControl") as PFA_TPControl)._screamRadius;
		
		if(Vector3.Distance(transform.position, _playerRef.transform.position) <= screamRad)
		{
			StartFleeing();
		}
	}
	
	//Privates	
	void InitLine()
	{
		_lineDir = _patrolReference.transform.position - transform.position;
		_lineDir = Vector3.Normalize(_lineDir);
		_lineDir.y = 0;
	}
	
	void InitFlee()
	{
		float angleW = Random.Range(-_fleeAngleBounds, _fleeAngleBounds);
		Quaternion rotationW = Quaternion.AngleAxis(angleW, Vector3.up);
		
		_fleeDir = rotationW * transform.forward;
		_fleeDir = Vector3.Normalize(_fleeDir);
		_fleeDir.y = 0;
	}
	
	void InitWander()
	{
		float angleW = Random.Range(-_anglesLimit, _anglesLimit);
		Quaternion rotationW = Quaternion.AngleAxis(angleW, Vector3.up);
		
		_wanderDir = rotationW * transform.forward;
		_wanderDir = Vector3.Normalize(_wanderDir);
		_wanderDir.y = 0;
	}
	
	//Necessary sets
	void InitBehaviour()
	{
		_currentBehaviour = _behaviourPattern;
		
		switch(_behaviourPattern)
		{
			case (int)Behaviours.LinearMov:
				InitLine();
			break;
			
			case (int)Behaviours.RandMov:
				InitWander();
			break;
			
			case (int)Behaviours.Fleeing:
				InitFlee();
			break;
		}
	}
	
	void Update () 
	{
		CivilMovement();
	}
	
	// FLEE NIGGA
	void StartFleeing()
	{
		_behaviourPattern = (int)Behaviours.Fleeing;
		_currentBehaviour = _behaviourPattern;
		
		InitFlee();
	}
	
	// Flee other way of the player, stops when doesn't see him anymore
	void BehaveFlee()
	{
		Vector3 moveVec;
		
		_fleeCounter += Time.deltaTime;
			
		if(_fleeCounter >= _maxDirFleeTime)
		{
			_fleeDir = (transform.position - _playerRef.transform.position);
			float angleW = Random.Range(-_fleeAngleBounds, _fleeAngleBounds);
			Quaternion rotationW = Quaternion.AngleAxis(angleW, Vector3.up);
			
			moveVec = _fleeDir;
			moveVec = rotationW * moveVec;
			moveVec = Vector3.Normalize(moveVec);
			moveVec.y = 0;
			_fleeDir = moveVec;
			
			_fleeCounter = 0f;
		}
		
		moveVec = _fleeDir * _speed * Time.deltaTime;
		
		transform.Translate(_fleeDir * _speed * Time.deltaTime);
		RotateGraphTowards(moveVec);
	}
	
	// Wander within radius.
	void BehaveRand()
	{
		Vector3 moveVec;
		
		if(Vector3.Distance(transform.position, _originalPos) >= _wanderRadius)
		{
			moveVec = (_originalPos - transform.position);
			moveVec = Vector3.Normalize(moveVec);
			moveVec.y = 0;			
			_wanderDir = moveVec;
			
			_wanderCounter = 0f;
		}
		else
		{
			_wanderCounter += Time.deltaTime;
			
			if(_wanderCounter >= _maxDirWanderTime)
			{
				float angleW = Random.Range(-_anglesLimit, _anglesLimit);
				Quaternion rotationW = Quaternion.AngleAxis(angleW, Vector3.up);
				
				moveVec = _wanderDir;
				moveVec = rotationW * moveVec;
				moveVec = Vector3.Normalize(moveVec);
				_wanderDir = moveVec;
				
				_wanderCounter = 0f;
			}
		}
			
		moveVec = _wanderDir * _speed * Time.deltaTime;
		
		transform.Translate(moveVec);
		RotateGraphTowards(moveVec);
	}
	
	// Go towards point and go back to originpoint
	void BehaveLine()
	{
		// Movement
		Vector3 moveVec = _lineDir * _speed * Time.deltaTime * _lineDirMod;
		transform.Translate(moveVec);
		
		// Player graphic rotation
		RotateGraphTowards(moveVec);
		
		// Change direction if close to patrol point
		if(_lineDirMod == -1f)
		{
			if(Vector3.Distance(transform.position, _originalPos) <= 1f)
			{
				_lineDirMod *= -1f;
			}
		}
		else if (_lineDirMod == 1f)
		{
			if(Vector3.Distance(transform.position, _patrolReference.transform.position) <= 1f)
			{
				_lineDirMod *= -1f;
			}
		}
			
	}
	
	// Idle behaviour
	void BehaveIdle()
	{
		// If close enough, look at player !
		if(Vector3.Distance(_playerRef.transform.position, transform.position) <= 5f)
		{
			Vector3 LookPlayer = _playerRef.transform.position - transform.position;
			LookPlayer.y = 0;
			RotateGraphTowards(LookPlayer);
		}
	}
	
	bool CivilianSeesPlayer()
	{
		bool retbool = true;
		
		Ray castRay = new Ray(transform.position, (_playerRef.transform.position - transform.position));
		float castDist = Vector3.Distance(transform.position, _playerRef.transform.position);
		RaycastHit hittarget;
		
		if(Physics.Raycast(castRay, out hittarget, castDist))
		{
			if(hittarget.collider.gameObject.tag == "Wall")
			{
				retbool = false;
			}
		}
		
		Debug.DrawRay(transform.position, (_playerRef.transform.position - transform.position), Color.red);
		
		return retbool;
	}
	
	// Manage behaviours
	void CivilMovement()
	{
		switch(_currentBehaviour)
		{
			case (int)Behaviours.Fleeing:
				if(CivilianSeesPlayer())
				{
					if(_maxFleeTime <= 0)
					{
						BehaveFlee();
					}
					else
					{
						BehaviourChecks(BehaveFlee, _maxFleeTime, _minFleeTime, _randFlee);
					}
				}
				else
				{
					Debug.Log("STOPPIT");
				}
			break;
			
			case (int)Behaviours.Idle:
				if(_maxIdleTime > 0 && _behaviourPattern != (int)Behaviours.Idle)
				{
					BehaviourChecks(BehaveIdle, _maxIdleTime, _minIdleTime, _randIdle);
				}
				else
				{
					BehaveIdle();
				}
			break;
			
			case (int)Behaviours.LinearMov:
				if(_maxLineTime <= 0)
				{
					BehaveLine();
				}
				else
				{
					BehaviourChecks(BehaveLine, _maxLineTime, _minLineTime, _randLine);
				}
			break;
			
			case (int)Behaviours.RandMov:
				if(_maxRandTime <= 0)
				{
					BehaveRand();
				}
				else
				{
					BehaviourChecks(BehaveRand, _maxRandTime, _minRandTime, _randRand);
				}
			break;
		}
	}
	
	// Updates time, checks if over and applies correct behaviour
	void BehaviourChecks(RefFunc BehaveFunc, float _maxTime, float _minTime, float _randChance)
	{
		_counter += Time.deltaTime;
		
		if(_counter >= _maxTime)
		{
			_currentBehaviour = _behaviourPattern;
			_counter = 0f;
		}
		else
		{
			if(ChangeBCheck(_randChance) && _counter >= _minTime)
			{
				ToggleIdleBehaviour();
			}
			else
			{
				BehaveFunc();
			}
		}
	}
	
	// Toggles between IDLE and BEHAVIOUR
	void ToggleIdleBehaviour()
	{
		if(_behaviourPattern != (int)Behaviours.LinearMov && _behaviourPattern != (int)Behaviours.Fleeing)
		{
			if(_currentBehaviour != _behaviourPattern)
			{
				_currentBehaviour = _behaviourPattern;
			}
			else
			{
				_currentBehaviour = (int)Behaviours.Idle;
			}
			
			_counter = 0f;
		}
	}
	
	// Can change behaviour ?
	bool ChangeBCheck(float _randChance)
	{
		bool _retCheck = false;
		
		if(_behaviourPattern != (int)Behaviours.LinearMov && _behaviourPattern != (int)Behaviours.Fleeing)
		{
			float randthrow = Random.Range(0f, 1f);
			
			if(randthrow <= _randChance)
			{
				_retCheck = true;
			}
		}
		
		return _retCheck;
	}
	
	// Rotate towards 
	void RotateGraphTowards(Vector3 moveVec)
	{
		if (moveVec != Vector3.zero)
		{
			Quaternion newRotation = Quaternion.LookRotation(moveVec);
			civilGraph.rotation = Quaternion.Slerp(civilGraph.rotation, newRotation, Time.deltaTime * 8);
		}
	}
}
