﻿using UnityEngine;
using System.Collections;

public class PFA_PauseMenu : MonoBehaviour {


	public GUISkin myskin;
	
	private Rect windowRect;
	
	private bool paused = false, waited = false;
	
	private void start()
	{
		windowRect = new Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200);
	}
	
	private void waiting()
	{
		waited = true;
	}
	
	private void Update()
	{
		if (waited)
		if (Input.GetKey(KeyCode.Escape))
		{
			if(paused)
				paused = false;
			else
				paused = true;
			
			waited = false;
			Invoke ("waiting",0.3f);
		}
		if(paused)
		
			Time.timeScale = 0;
		else
				Time.timeScale = 1;
		
	}
	
	private void OnGUI()
	{
		if(paused)
			windowRect = GUI.Window (0, windowRect, windowFunc, "pause menu");
	}
	
	private void windowFunc(int id)
	{
		if(GUILayout.Button ("resume"))
		{
			paused = false;
		}
		GUILayout.BeginHorizontal();
		if(GUILayout.Button ("Options"))
		{
			
		}
		if (GUILayout.Button("Quit"))
		{
			
		}
		GUILayout.EndHorizontal();
	}
	
}
