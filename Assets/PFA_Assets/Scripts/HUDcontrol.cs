using UnityEngine;
using System.Collections;

public class HUDcontrol : MonoBehaviour {
	
	
	public Texture DamageCounter1;
	public Texture DamageCounter2; 
	public Texture DamageCounter3; 
	public Texture DamageCounter4; 
	public Texture DamageCounter5; 
	public Texture DamageCounter6;
	public Texture DamageCounter7; 
	public Texture DamageCounter8; 
	public Texture DamageCounter9; 
	
	public Transform _ExplosionRef;
	

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public void Update () {
		
	if (_ExplosionRef.GetComponent<ExplosionBehaviour>().CivilianCounter == 8 || Input.GetButtonDown ("360_AButton"))
		
		{
			guiTexture.texture = DamageCounter1;
		}
	if (_ExplosionRef.GetComponent<ExplosionBehaviour>().CivilianCounter == 7)
	
		{
			guiTexture.texture = DamageCounter2;
		}
	if (_ExplosionRef.GetComponent<ExplosionBehaviour>().CivilianCounter == 6)
		
		{
			guiTexture.texture = DamageCounter3;
		}
	if (_ExplosionRef.GetComponent<ExplosionBehaviour>().CivilianCounter == 5)
		
		{
			guiTexture.texture = DamageCounter4;
		}
	if (_ExplosionRef.GetComponent<ExplosionBehaviour>().CivilianCounter == 4)
		
		{
			guiTexture.texture = DamageCounter5;
		}
		
	if (_ExplosionRef.GetComponent<ExplosionBehaviour>().CivilianCounter == 3)
		
		{
			guiTexture.texture = DamageCounter6;
		}
	if (_ExplosionRef.GetComponent<ExplosionBehaviour>().CivilianCounter == 2)
		
		{
			guiTexture.texture = DamageCounter7;
		}
	if (_ExplosionRef.GetComponent<ExplosionBehaviour>().CivilianCounter == 1)
		
		{
			guiTexture.texture = DamageCounter8;
		}
	if (_ExplosionRef.GetComponent<ExplosionBehaviour>().CivilianCounter == 0)
		
		{
			guiTexture.texture = DamageCounter9;
		}
	
	}
}
