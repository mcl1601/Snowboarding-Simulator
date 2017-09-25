using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Game : MonoBehaviour 
{
	public GameObject board;
	public GameObject[] checkpoints = new GameObject[6];
	private GameObject currentPoint;
	private bool gameActive;
	private int index;
	private int passedPoints;
	private float ftime;
	private double time;
	private bool endGame;
	private Vector3 freezeLoc;
	private Vector3 initLoc;
	private Quaternion initial_rotation;

	// Use this for initialization
	void Start () 
	{
		gameActive = gameObject.GetComponent<WiimoteDemo> ().game;
		currentPoint = checkpoints [0];
		index = 0;
		passedPoints = 0;
		time = 0;
		ftime = 0;
		endGame = false;
		freezeLoc = Vector3.zero;
		initLoc = board.transform.position;
		initial_rotation = board.transform.localRotation;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// check if the game is active
		if (gameActive == false && endGame == false) 
		{
			gameActive = gameObject.GetComponent<WiimoteDemo> ().game;
			return;
		}
		if (index < checkpoints.Length) {
			// set current checkpoint
			currentPoint.GetComponent<Checkpoint> ().current = true;
			ftime += Time.deltaTime;
			time = Math.Round (ftime, 2);
			// see if they went past a checkpoint
			if (board.transform.position.z > currentPoint.transform.position.z) {
				if (board.transform.position.x < currentPoint.transform.position.x + 5
				   && board.transform.position.x > currentPoint.transform.position.x - 5 && index == checkpoints.Length - 1) 
				{
					currentPoint.GetComponent<Checkpoint> ().current = false;
					index++;
					passedPoints++;
					freezeLoc = board.transform.position;
					gameObject.GetComponent<WiimoteDemo> ().game = false;
					board.GetComponent<Rigidbody> ().isKinematic = true;
				}
				else if (board.transform.position.x < currentPoint.transform.position.x + 5
				    && board.transform.position.x > currentPoint.transform.position.x - 5) {
					currentPoint.GetComponent<Checkpoint> ().current = false;
					index++;
					passedPoints++;
					currentPoint = checkpoints [index];
				} else {
					currentPoint.GetComponent<Checkpoint> ().current = false;
					index++;
					currentPoint = checkpoints [index];
				}
			}
		} else 
		{
			endGame = true;
			gameActive = false;
			board.transform.position = freezeLoc;
		}
	}

	void OnGUI()
	{
		GUIStyle gStyle = new GUIStyle ();
		gStyle.fontSize = 30;
		gStyle.alignment = TextAnchor.MiddleCenter;
		gStyle.normal.textColor = Color.white;

		GUIStyle tStyle = new GUIStyle ();
		tStyle.fontSize = 30;
		tStyle.alignment = TextAnchor.MiddleLeft;
		tStyle.normal.textColor = Color.white;

		GUIStyle fStyle = new GUIStyle ();
		fStyle.fontSize = 50;
		fStyle.alignment = TextAnchor.MiddleCenter;
		fStyle.normal.textColor = Color.white;

		GUIStyle pStyle = new GUIStyle ();
		pStyle.fontSize = 50;
		pStyle.alignment = TextAnchor.MiddleCenter;
		pStyle.normal.textColor = new Color(0.85f, 0.1f, 0, 1f);

		GUIStyle jStyle = new GUIStyle ();
		jStyle.fontSize = 50;
		jStyle.alignment = TextAnchor.MiddleCenter;
		jStyle.normal.textColor = Color.yellow;


		GUIStyle boxStyle = new GUIStyle ();
		boxStyle.normal.background = MakeTex (2,2, new Color (0f, 0.5f, 1f, 0.75f));
		GUIStyle boxStyle2 = new GUIStyle ();
		boxStyle2.normal.background = MakeTex (2,2, new Color (0f, 0f, 0, 0.2f));

		GUIStyle bStyle = new GUIStyle();
		bStyle.alignment = TextAnchor.MiddleCenter;
		bStyle.fontSize = 50;
		bStyle.normal.background = MakeTex (2,2, new Color(0,0.75f, 0, 1));
		bStyle.hover.background = MakeTex (2,2, new Color(0,0.5f, 0, 1));
		bStyle.normal.textColor = Color.white;
		bStyle.hover.textColor = Color.white;

		GUIStyle sStyle = new GUIStyle();
		sStyle.alignment = TextAnchor.MiddleLeft;
		sStyle.fontSize = 45;
		sStyle.normal.textColor = Color.white;

		if (gameActive) {
			GUI.Box (new Rect (Screen.width * 0.25f, 0, Screen.width * 0.5f, 100), "");
			GUILayout.BeginVertical (GUILayout.Width(Screen.width * 0.5f));
			GUI.Label(new Rect (Screen.width * 0.25f, 0, Screen.width * 0.5f, 50), "Checkpoints  " + passedPoints + " / " + checkpoints.Length, gStyle);
			GUI.Label(new Rect (Screen.width * 0.475f, 50, Screen.width * 0.5f, 50), "" + time, tStyle);
			GUILayout.EndVertical ();

			GUI.Box (new Rect (0, Screen.height - 75f, 200f, 75f), "");
			GUI.Label (new Rect (10, Screen.height - 70f, 75f, 75f), ""+ Mathf.Round((Vector3.SqrMagnitude (board.GetComponent<Rigidbody> ().velocity) / 10)) + " mph", sStyle);
		}

		if (endGame) 
		{
			GUI.Box (new Rect (Screen.width * 0.07f, Screen.height * 0.23f, Screen.width * 0.85f, Screen.height * 0.7f), "", boxStyle);
			GUI.Box (new Rect (Screen.width * 0.07f + 25f, Screen.height * 0.23f + 25f, Screen.width * 0.85f - 50f, Screen.height * 0.57f - 40f), "", boxStyle2);
			GUI.Label(new Rect (Screen.width * 0.25f, Screen.height * 0.28f, Screen.width * 0.5f, 50), "Cleared Checkpoints: " + passedPoints + " / " + checkpoints.Length, fStyle);
			GUI.Label(new Rect (Screen.width * 0.25f, Screen.height * 0.28f + 70, Screen.width * 0.5f, 50), "Missed Checkpoints: " + (checkpoints.Length - passedPoints), fStyle);
			GUI.Label(new Rect (Screen.width * 0.25f, Screen.height * 0.28f + 140, Screen.width * 0.5f, 50), "Time: " + time, fStyle);
			GUI.Label(new Rect (Screen.width * 0.25f, Screen.height * 0.28f + 210, Screen.width * 0.5f, 50), "Penalty: " + "+" + (checkpoints.Length - passedPoints) + " s", pStyle);
			GUI.Label(new Rect (Screen.width * 0.25f, Screen.height * 0.28f + 280, Screen.width * 0.5f, 50), "Total Time: " + (time + checkpoints.Length - passedPoints), jStyle);
			if (GUI.Button (new Rect (Screen.width * 0.5f - (Screen.width * 0.25f * 0.5f), Screen.height * 0.3f + 350, Screen.width * 0.25f, 75), "Play Again", bStyle)) 
			{
				// reset the game
				currentPoint = checkpoints [0];
				index = 0;
				passedPoints = 0;
				time = 0;
				ftime = 0;
				endGame = false;
				gameActive = false;
				board.transform.position = initLoc;
				board.GetComponent<Rigidbody> ().velocity = Vector3.zero;
				board.transform.rotation = initial_rotation;
			}
		}
	}

	private Texture2D MakeTex( int width, int height, Color col )
	{
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; ++i )
		{
			pix[ i ] = col;
		}
		Texture2D result = new Texture2D( width, height );
		result.SetPixels( pix );
		result.Apply();
		return result;
	}
}
