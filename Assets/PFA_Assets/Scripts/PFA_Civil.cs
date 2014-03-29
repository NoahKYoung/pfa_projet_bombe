using UnityEngine;
using System.Collections;

public class PFA_Civil : MonoBehaviour 
{
	public int _behaviourPattern = 0;
	public float _speed = 4f;
	
	private int _currentBehaviour = 0;
	
	public float _maxIdleTime = 0f;
	public float _minIdleTime = 0f;
	
	public float _maxFleeTime = 0f;
	public float _minFleeTime = 0f;
	
	public float _maxLineTime = 0f;
	public float _minLineTime = 0f;
	
	public float _maxRandTime = 0f;
	public float _minRandTime = 0f;
	
	public float _randIdle = 0.1f;
	public float _randFlee = 0.1f;
	public float _randLine = 0.1f;
	public float _randRand = 0.1f;
	
	private float _counter = 0f;
	private Vector3 _originalPos;
	
	private delegate void RefFunc();
	
	enum  Behaviours
	{
		LinearMov = 0,
		RandMov = 1,
		Idle = 2,
		Fleeing = 3
	};
	
	// Use this for initialization
	void Start () 
	{
		InitBehaviour();
		_originalPos = this.transform.position;
	}
	
	void InitBehaviour()
	{
		_currentBehaviour = _behaviourPattern;
	}
	
	// Update is called once per frame
	void Update () 
	{
		CivilMovement();
	}
	
	void BehaveFlee()
	{
		Debug.Log ("FLEEING");
	}
	
	void BehaveRand()
	{
		Debug.Log ("WANDERING");
	}
	
	void BehaveLine()
	{
		Debug.Log ("PATROLLING");
	}
	
	void BehaveIdle()
	{
		Debug.Log ("IDLING");
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
					Debug.Log("WADDUP");
					BehaviourChecks(BehaveRand, _maxRandTime, _minRandTime, _randRand);
				}
			break;
		}
	}
		
	void BehaviourChecks(RefFunc BehaveFunc, float _maxTime, float _minTime, float _randChance)
	{
		_counter += Time.deltaTime;
		Debug.Log ("COUNtING " + _counter);
		
		if(_counter >= _maxTime)
		{
			_currentBehaviour = _behaviourPattern;
			_counter = 0f;
		}
		else
		{
			if(ChangeBCheck(_randChance) && _counter >= _minTime)
			{
				Debug.Log("TOGGLING B...");
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
				Debug.Log("RESETTING B.");
				_currentBehaviour = _behaviourPattern;
			}
			else
			{
				Debug.Log("SWITCHING TO IDLE B.");
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
	/*
	this.transform.Rotate(this.transform.up, Random.Range(-5f, 15f));
	this.transform.Translate(Vector3.Normalize(this.transform.forward) * Time.deltaTime * _speed);
	
	this.transform.rotation = Quaternion.LookRotation(Vector3.Normalize(this.transform.forward) * Time.deltaTime * _speed);
	*/
}
