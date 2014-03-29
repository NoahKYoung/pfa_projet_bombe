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
	
	void InitLine()
	{
		_lineDir = _patrolReference.transform.position - transform.position;
		_lineDir = Vector3.Normalize(_lineDir);
		_lineDir.y = 0;
	}
	
	void InitWander()
	{
		float angleW = Random.Range(-_anglesLimit, _anglesLimit);
		Quaternion rotationW = Quaternion.AngleAxis(angleW, Vector3.up);
		
		_wanderDir = rotationW * transform.forward;
		_wanderDir = Vector3.Normalize(_wanderDir);
		_wanderDir.y = 0;
	}
	
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
		}
	}
	
	void Update () 
	{
		CivilMovement();
	}
	
	void BehaveFlee()
	{
		Debug.Log("FLEEING");
	}
	
	void BehaveRand()
	{
		Vector3 moveVec;
		
		if(Vector3.Distance(transform.position, _originalPos) >= _wanderRadius)
		{
			Debug.Log ("GOING BACK");
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
	
	void CivilMovement()
	{
		switch(_currentBehaviour)
		{
			case (int)Behaviours.Fleeing:
				if(_maxFleeTime <= 0)
				{
					BehaveFlee();
				}
				else
				{
					BehaviourChecks(BehaveFlee, _maxFleeTime, _minFleeTime, _randFlee);
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
	
	void RotateGraphTowards(Vector3 moveVec)
	{
		if (moveVec != Vector3.zero)
		{
			Quaternion newRotation = Quaternion.LookRotation(moveVec);
			civilGraph.rotation = Quaternion.Slerp(civilGraph.rotation, newRotation, Time.deltaTime * 8);
		}
	}
}
