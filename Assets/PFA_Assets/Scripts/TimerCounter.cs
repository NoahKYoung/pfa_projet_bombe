using UnityEngine;
using System.Collections;

public class TimerCounter : MonoBehaviour 
{
	
	public float time = 90.0f;
	public Transform _playerRef;
	TextMesh tm;

	// Use this for initialization
	void Start () 
	{
	
		tm = GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	public void Update () 
	{
		time -= Time.deltaTime;
		tm.text = Mathf.RoundToInt(time).ToString();
	
		if(time <= 0f)
		{
			(_playerRef.GetComponent("PFA_TPControl") as PFA_TPControl).Explode();
		}
	}
}
	
	
