using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class paintGM : MonoBehaviour {

	// PUBLIC EDITABLE VARIABLES (in unity editor)
	public Transform baseDot;
	public KeyCode mouseLeft, spaceBar;
	public float canvasWidthIn, canvasHeightIn;
	

    // PUBLIC VARIABLES FOR OTHER CLASSES TO ACCESS
    public static float canvasWidth, canvasHeight;
	public static string toolType, currentTag; //what tool is being utilized, current dot-tool tag
	public static Color currentColor;
	public static float currentScale = 2.0f;
	public static float modOperator = 30.0f;
	public static float modOperatorOffset = -15.0f;

    public static Dictionary<string, int> soundTags = new Dictionary<string, int>();

    //PRIVATE VARIABLES
	private float yPosPrev, yPos; //prev synced variable to compare, syncer variable
    private Vector3 prevPos = new Vector3(0.0f, 0.0f, 0.0f);
	private string[] colorTags = new string[] {"adc","pink","green","yellow","orange","blue"};
	private string initColorTag = "";

    // Chuck stuff
    private ChuckSubInstance myChuckPitchTrack;
    private ChuckFloatSyncer myPitchSyncer, myAdcSyncer;



    // Use this for initialization
    void Start () 
	{
		InitializePaintPositions();
		InitializeColorInfo();
		SetUpChuck();
	}

	
	void InitializePaintPositions()
	{
        canvasWidth = canvasWidthIn;
        canvasHeight = canvasHeightIn;
        yPosPrev = yPos;
	}
	
	void InitializeColorInfo()
	{
        initColorTag = colorTags[3];
        currentColor = GameObject.Find(initColorTag).GetComponent<SpriteRenderer>().color;
        currentTag = initColorTag;
        LinkColorTagToSound(); //link colors to sounds, using ColorTags array
    }
	
	void SetUpChuck()
	{
		myChuckPitchTrack = GetComponent<ChuckSubInstance>();
        myPitchSyncer = gameObject.AddComponent<ChuckFloatSyncer>();
        myPitchSyncer.SyncFloat(myChuckPitchTrack, "midiPos"); //current instance of chuck is determining pos value
        myAdcSyncer = gameObject.AddComponent<ChuckFloatSyncer>();
        myAdcSyncer.SyncFloat(myChuckPitchTrack, "adcOnFlag");
		//StartChuckPitchTrack(myChuckPitchTrack);
	}
	
	void Update () {
		
		//get mouse info no matter what
		Vector3 mousePosition = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10.0f);
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
		//snap to xcoord grid
		objPosition.x = Mathf.Round(objPosition.x);
		if (objPosition.x == prevPos.x) //vertical line
			objPosition.y = Mathf.Round(objPosition.y);

		//if mouse is over the canvas and clicked down
		if( Input.GetKey(mouseLeft)
			&& Mathf.Abs(objPosition.x) < canvasWidth/2.0f 
			&& Mathf.Abs(objPosition.y) < canvasHeight/2.0f
            && (objPosition.x != prevPos.x && Mathf.Round(objPosition.y) != Mathf.Round(prevPos.y))
			&& toolType == "brush" )
		{
            // use mouse position as drawing point
			Instantiate(baseDot, objPosition, baseDot.rotation);
            //keep track of previous placement
            prevPos = objPosition;
		}

        else if (toolType == "adc" && Input.GetKey(spaceBar))
        {
            myAdcSyncer.SetNewValue(1.0f);
            Vector3 voicePosition = new Vector3(PlayLineController.xpos + 1, SetPitch2YPosition(), objPosition.z);
            currentColor = new Color32(100, 100, 180, 255);
            Instantiate(baseDot, voicePosition, baseDot.rotation);
        }
        else
        {
            myAdcSyncer.SetNewValue(0.0f);
        }
	}

	void LinkColorTagToSound()
	{
        // link color tags to instruments index value in dictionary
        for(int i = 0; i<colorTags.Length; i++) 
            soundTags.Add(colorTags[i], i);
	}

	float SetPitch2YPosition()
	{
        yPos = MapPitchToYPosition(myPitchSyncer.GetCurrentValue());
        if ((yPos < yPosPrev - 12.0f) | (yPos > yPosPrev + 12.0f)) //try to correct for >octave errors
            yPos = yPosPrev;
        else
            yPosPrev = yPos;
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
			1.0 => global float adcOnFlag;

			
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
			
			// run adc tracker indefinitely
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
							if (count >= 5)
							{
								m_cf => midiPos;
								//<<< ""Note:"", midiPos >>>; //test to check input       
							}
						}
						else
						{
							0 => count;
						}
					}
					curr_freq => prev_freq;
					0.0050::second => now;
				}
			}
			
		");
    }
}
