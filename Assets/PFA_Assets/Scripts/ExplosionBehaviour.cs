using UnityEngine;
using System.Collections;

public class ExplosionBehaviour : MonoBehaviour {
	
	
	public float CivilianCounter = 9;

void OnTriggerEnter(Collider other)
	{
		if (Input.GetAxis("360_Triggers")>0.001)
		{
		
			Destroy(other.gameObject);
			CivilianCounter -= 1;
		}
	}
	
}