using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class paintGM : MonoBehaviour {

	// PUBLIC EDITABLE VARIABLES (in unity editor)
	public Transform baseDot;
	public KeyCode mouseLeft;
	public KeyCode spaceBar;
	public float canvasWidthIn;
	public float canvasHeightIn;
	

    // PUBLIC VARIABLES FOR OTHER CLASSES TO ACCESS
    public static float canvasWidth;
    public static float canvasHeight;
	public static string toolType; //what tool is being utilized
	public static Color currentColor;
	public static int currentOrder;
	public static float currentScale = 2.0f;
	public static string currentTag;
	public static float modOperator = 30.0f;
	public static float modOperatorOffset = -15.0f;

    public static Dictionary<string, int> soundTags = new Dictionary<string, int>();

    //PRIVATE VARIABLES
	private float midiNote;
    private float yPosPrev = -1;
    private float yPos; //syncer variable
	private string[] colorTags = new string[] {"adc","pink","green","yellow","orange","blue"};

    // Chuck stuff
    ChuckSubInstance myChuckPitchTrack;
    ChuckFloatSyncer myPitchSyncer;
    ChuckFloatSyncer myTimeSyncer;



    // Use this for initialization
    void Start () 
	{
        canvasWidth = canvasWidthIn;
		canvasHeight = canvasHeightIn;
		midiNote = 60;
        yPosPrev = yPos;

		//link colors to sounds, using ColorTags array
		LinkColorTagToSound();

        // set up chuck
        myChuckPitchTrack = GetComponent<ChuckSubInstance>();
        StartChuckPitchTrack(myChuckPitchTrack);
        myPitchSyncer = gameObject.AddComponent<ChuckFloatSyncer>();
        myPitchSyncer.SyncFloat(myChuckPitchTrack, "midiPos"); //current instance of chuck is determining pos value
	}
	
	// Update is called once per frame
	void Update () {
		
		//get mouse info no matter what
		Vector3 mousePosition = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10.0f);
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);

		//if mouse is over the canvas
		if(Mathf.Abs(objPosition.x) < canvasWidth/2.0f && Mathf.Abs(objPosition.y) < canvasHeight/2.0f) 
		{
            // use mouse position as drawing point
			if (Input.GetKey(mouseLeft))
            {
				Instantiate(baseDot, objPosition, baseDot.rotation);
            }
            // get voice input and use as drawing point
            else if (toolType == "adc" && Input.GetKey(spaceBar))
            {
                Vector3 voicePosition = new Vector3(PlayLineController.xpos, SetPitch2YPosition(), objPosition.z);
                currentColor = new Color32(100, 100, 180, 255);
				Instantiate(baseDot, voicePosition, baseDot.rotation);

                
            }
		}
		
	}

	void LinkColorTagToSound()
	{
        // link color tags to instruments in sound dictionary
        for(int i = 0; i<colorTags.Length; i++) 
		{
            soundTags.Add(colorTags[i], i);
		}
	}

	float SetPitch2YPosition()
	{
        midiNote = myPitchSyncer.GetCurrentValue();
        yPos = MapPitchToYPosition(midiNote);
        if ((yPos < yPosPrev - 13.0f) | (yPos > yPosPrev + 13.0f)) //try to correct for pitch
        {
            yPos = yPosPrev;
        }
        else
        {
            yPosPrev = yPos;
        }
		Debug.Log(yPos);
		return yPos;
	}

	float MapPitchToYPosition(float midiNote) 
	{
		return (midiNote % modOperator + modOperatorOffset);
	}

    // ChucK pitch tracking script contained here
    void StartChuckPitchTrack(ChuckSubInstance myChuckPitchTrack)
    {
        // instantiate Chuck Pitch Tracking code
        myChuckPitchTrack.RunCode(@"

			60.0 => global float midiPos;
			
			// analysis
			adc => PoleZero dcblock => FFT fft => blackhole;

			// set to block DC
			.99 => dcblock.blockZero;
			// set FFT params
			1024 => fft.size;
			// window
			Windowing.hamming( fft.size() ) => fft.window;

			// to hold result
			UAnaBlob blob;
			// find sample rate
			second / samp => float srate;

			// interpolate
			float target_freq, curr_freq, target_gain, curr_gain;
			spork ~ ramp_stuff();

			// go for it
			while( true )
			{
				// take fft
				fft.upchuck() @=> blob;
				// find peak
				0 => float max; int where;
				for( int i; i < blob.fvals().cap(); i++ )
				{
					// compare
					if( blob.fvals()[i] > max )
					{
						// save
						blob.fvals()[i] => max;
						i => where;      
					}    
				}  
				// set freq
				(where $ float) / fft.size() * srate => target_freq;
				
				// set gain
				(max / .8) => target_gain;
				
				// hop
				(fft.size()/2)::samp => now;
			}

				// interpolation
			fun void ramp_stuff()
			{
				// mysterious 'slew'
				0.025 => float slew;
				float m_cf;
				float m_pf;

				// infinite time loop
				0 => int count;
				-1.0 => float prev_freq;
				while (true)
				{
					(target_freq - curr_freq) * 5 * slew + curr_freq => curr_freq;
					(target_gain - curr_gain) * slew + curr_gain => curr_gain;
					if (prev_freq > -1.0 && curr_freq >= 110.0 && curr_freq <= 1760)  //plausible sung pitch
					{
						Math.round(Std.ftom(curr_freq)) => m_cf;
						Math.round(Std.ftom(prev_freq)) => m_pf;
						if (m_cf == m_pf)
						{
							count++;
							if (count >= 10)
							{
								//curr_freq => s.freq;
								//0 => s.gain;
								m_cf => midiPos;
							//<<< ""Note:"", midiPos >>>; //test to check input       
							}
						}
						else
						{
							0 => count;
							//0 => s.freq;
							//0 => s.gain;
						}
					}
					curr_freq => prev_freq;
					0.0025::second => now;
				}
			}
			
		");
    }
}
