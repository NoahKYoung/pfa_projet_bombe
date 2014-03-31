using UnityEngine;
using System.Collections;

public class pauseMenu : MonoBehaviour 
{
    public GUISkin myskin;

    private Rect windowRect;
    bool paused = false , waited = true;

    private void Start()
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
            if (Input.GetKey(KeyCode.Escape) || Input.GetButtonDown ("360_StartButton"))
            {
                if (paused == true)
                    paused = false;
                else
                    paused = true;
			
                waited = false;
                Invoke("waiting",0.3f);
            }
			if(paused)
		{
			Time.timeScale = 0;
			GetComponent<PFA_TPControl>().controlenabled = false;
		}
		else
		{
			Time.timeScale = 1;
			GetComponent<PFA_TPControl>().controlenabled = true;
		}
	}

    private void OnGUI()
    {
        if (paused)
            windowRect = GUI.Window(0, windowRect, windowFunc, "Pause Menu");
    }

    private void windowFunc(int id)
    {
        if (GUILayout.Button("Resume"))
        {
            paused = false;
        }
        if (GUILayout.Button("Options"))
        {

        }
        if (GUILayout.Button("Main Menu"))
        {
			Application.LoadLevel(0);
        }
    }
}
