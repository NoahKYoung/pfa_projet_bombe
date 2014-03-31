using UnityEngine;
using System.Collections;

public class ExplosionBehaviour : MonoBehaviour {
	
	
	public float CivilianCounter = 9;

void OnTriggerEnter(Collider other)
	{
		switch(other.tag)
		{
			case "CivilExplode":
				Destroy(other.gameObject);
				Debug.Log("CivilDown");
				CivilianCounter --;	
			break;
		}
	
	}
}