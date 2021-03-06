﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayLineController : MonoBehaviour {

    // Public variables
	public KeyCode playLeft, playRight, stopPlay, spaceBar;
    public static bool playRightFlag = false;
    public static bool playLeftFlag = false;
    public static float xpos;
    public static float currentTempo = 80.0f; //bpm
    public static float beatCount = 0.0f;
	

    // chuck sync stuff
    private ChuckSubInstance myChuck;
    private ChuckEventListener myNextBeatListener;
    private ChuckFloatSyncer myTempoSyncer;
	private bool beatFlag = false;
	private float startPos;
	
	// Use this for initialization
	void Start () {
        startPos = -1*paintGM.canvasWidth/2.0f;
		xpos = startPos;
        RunChuckClock();
	}
	
	// Update is called once per frame
	void Update () 
	{
        //update tempo
		myTempoSyncer.SetNewValue(currentTempo);
        
		//check if play keys have been hit
		if (Input.GetKeyDown(playRight)) 
		{
			playRightFlag = true;
			playLeftFlag = false;
		}
		else if (Input.GetKeyDown(playLeft))
		{
            playLeftFlag = true;
			playRightFlag = false;
		}
		// if not, reset
		else if (Input.GetKeyDown(stopPlay) || Input.GetKeyUp(spaceBar)) 
		{
            playRightFlag = false;
            playLeftFlag = false;
			beatCount = 0;
			xpos = startPos;
            transform.position = new Vector3(xpos, 0, 0); //set to start pos
        }
            

        // if in play mode or record mode, update xposition
		if ( playRightFlag || playLeftFlag || Input.GetKey(spaceBar) 
				&& beatFlag ) 
		{
            beatFlag = false;
			xpos = beatCount - paintGM.canvasWidth/2.0f; //shift to center around 0

			if (playRightFlag || playLeftFlag)
				transform.position = new Vector3(xpos, 0, 0); //update player line
		}
		
	}

    void ProcessBeat()
    {
		beatFlag = true;
        if (playLeftFlag) //move playline to the left 1/16th beat
            beatCount = (beatCount - 1.0f) % paintGM.canvasWidth;
        else if (playRightFlag || Input.GetKey(spaceBar)) //move current beat to the right 1/16th beat
            beatCount = (beatCount + 1.0f) % paintGM.canvasWidth;
    }


    void RunChuckClock()
    {
        myChuck = GetComponent<ChuckSubInstance>();

		myChuck.RunCode(@"
			80.0 => global float bpm;
			//global float beatPos;
			global Event beatNotifier;
			
			while( true )
			{
				(15.0/bpm)::second => now; //increment by 16th note interval
				beatNotifier.broadcast();
			}
		");

        myTempoSyncer = gameObject.AddComponent<ChuckFloatSyncer>();
		myTempoSyncer.SyncFloat(myChuck, "bpm");
		myNextBeatListener = gameObject.AddComponent<ChuckEventListener>();
        myNextBeatListener.ListenForEvent(myChuck, "beatNotifier", ProcessBeat);
    }
}
