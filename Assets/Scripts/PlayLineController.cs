using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayLineController : MonoBehaviour {

    // Public variables
	public KeyCode playLeft;
    public KeyCode playRight;
    public KeyCode playUp;
    public KeyCode playDown;
	public KeyCode stopPlay;
	public KeyCode spaceBar;
    public static float currentTempo = 60.0f; //bpm
    public static float xpos;

    // chuck sync stuff
    private ChuckSubInstance myChuck;
    private ChuckEventListener myNextBeatListener;
	public static float beatCount = 0;
	private bool beatFlag = false;
	private bool playRightFlag = false;
	private bool playLeftFlag = false;
	private float startPos;
	
	// Use this for initialization
	void Start () {
        startPos = -1*paintGM.canvasWidth/2.0f;
		xpos = startPos;
        myChuck = GetComponent<ChuckSubInstance>();
        SetupChuckClock();
	}
	
	// Update is called once per frame
	void Update () 
	{
        //update tempo
		myChuck.SetFloat("bpm", currentTempo);
        
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
		if (playRightFlag || playLeftFlag || Input.GetKey(spaceBar) && beatFlag) 
		{
			xpos = (beatCount % paintGM.canvasWidth - paintGM.canvasWidth / 2.0f);
            //Debug.Log(xpos);
            beatFlag = false;

			if (playRightFlag || playLeftFlag)
				transform.position = new Vector3(xpos, 0, 0); //update player line
		}
		
	}

    void ProcessBeat()
    {
		beatFlag = true;
        if (playLeftFlag) //move playline to the left 1/16th beat
            beatCount--;
        else if (playRightFlag || Input.GetKey(spaceBar)) //move current beat to the right 1/16th beat
            beatCount++;
		//Debug.Log(beatCount);
    }


    void SetupChuckClock()
    {
        myChuck.RunCode(@"
			80 => global float bpm;
			//global float beatPos;
			global Event beatNotifier;
			float timeStep;
			
			while( true )
			{
				(60.0/bpm)/4.0 => timeStep; //convert bpm to timestep and find 16th note interval
				timeStep::second => now;
				beatNotifier.broadcast();
			}
		");

        myNextBeatListener = gameObject.AddComponent<ChuckEventListener>();
        myNextBeatListener.ListenForEvent(myChuck, "beatNotifier", ProcessBeat);
    }
}
