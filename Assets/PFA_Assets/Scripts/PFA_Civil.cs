using UnityEngine;
using System.Collections;

public class PFA_Civil : MonoBehaviour 
{
	public float _speed = 4f;
	
	public int _behaviourPattern = 0;
	private int _currentBehaviour = 0;
	
	enum Behaviours
	{
		LinearMov,
		RandMov,
		Idle,
		Fleeing
	};
	
	// Use this for initialization
	void Start () 
	{
		InitBehaviour();
	}
	
	void InitBehaviour()
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		CivilMovement();
	}
	
	void CivilMovement()
	{
		this.transform.Rotate(this.transform.up, Random.Range(-5f, 15f));
		this.transform.Translate(Vector3.Normalize(this.transform.forward) * Time.deltaTime * _speed);
		
		this.transform.rotation = Quaternion.LookRotation(Vector3.Normalize(this.transform.forward) * Time.deltaTime * _speed);
	}
}
