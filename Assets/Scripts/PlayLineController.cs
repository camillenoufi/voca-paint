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
	public float canvasWidth;
    public static float currentTempo = 80.0f; //bpm

    // chuck sync stuff
    private ChuckSubInstance myChuck;
    private ChuckEventListener myNextBeatListener;
	private float beatCount = 0;
	private bool beatFlag = false;
	private bool playRightFlag = false;
	private bool playLeftFlag = false;
    private float xpos;
	private float startPos;
	
	// Use this for initialization
	void Start () {
        startPos = -1*canvasWidth/2.0f;
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
		else if (Input.GetKeyDown(stopPlay)) 
		{
            playRightFlag = false;
            playLeftFlag = false;
			beatCount = 0;
			xpos = startPos;
		}
            

        // if in play mode, update Player Line
		if (playRightFlag || playLeftFlag && beatFlag) 
		{
			xpos = (beatCount % canvasWidth - canvasWidth / 2.0f);
			Debug.Log(xpos);
			transform.position = new Vector3(xpos, 0, 0); //set to playline
			beatFlag = false;
		}
		
	}

    void ProcessBeat()
    {
		beatFlag = true;
        if (playLeftFlag) //move playline to the left 1/16th beat
            beatCount--;
        else if (playRightFlag) //move playline to the right 1/16th beat
            beatCount++;
		Debug.Log(beatCount);
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
