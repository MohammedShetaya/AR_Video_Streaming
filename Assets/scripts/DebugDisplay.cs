using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugDisplay : MonoBehaviour
{
	Dictionary<string, string> debugLogs = new Dictionary<string, string>();

	public Text display;

	private void Update()
	{
	}
	private void OnEnable()
	{
		Application.logMessageReceived += handleLogMessage;
	}

	private void OnDisable()
	{
		Application.logMessageReceived += handleLogMessage;

	}


	void handleLogMessage(string logString , string stackTrace , LogType type ) {

		display.text += "\n"+ logString;
		display.SetAllDirty();
	}
}
