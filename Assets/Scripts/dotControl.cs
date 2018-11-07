using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dotControl : MonoBehaviour {

    private ChuckSubInstance myChuck;
    private IEnumerator coroutine;
	private float midiPos;
    
	
	// Use this for initialization
	void Start () 
	{
		GetComponent<SpriteRenderer>().color = paintGM.currentColor;
        GetComponent<SpriteRenderer>().tag = paintGM.currentTag;
        GetComponent<Transform>().localScale = new Vector2(paintGM.currentScale,paintGM.currentScale);
        midiPos = ConvertYPosToMidiNote(gameObject.transform.position.y);

        myChuck = GetComponent<ChuckSubInstance>();
        myChuck.SetFloat("soundType", paintGM.soundTags[GetComponent<SpriteRenderer>().tag]);
        myChuck.SetFloat("midiNote", Mathf.Round(midiPos));

        SetHaloRender(false);
	}


	float ConvertYPosToMidiNote(float y)
	{
		float midi = Mathf.Round((2.0f * paintGM.modOperator) - paintGM.modOperatorOffset + y);
		return midi;
	}

	void OnMouseOver()
	{
		if(paintGM.toolType == "eraser")
			Destroy(gameObject);
	}

    void OnTriggerEnter(Collider other) //turn on halo
    {	
		if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(runChuckSubInstance());
            StartCoroutine(HaloOnOff(2.0f));
        }
    }

    private IEnumerator HaloOnOff(float waitTime)
    {
        SetHaloRender(true);
		yield return new WaitForSeconds(waitTime);
        SetHaloRender(false);
    }

    void SetHaloRender(bool state)
    {
        Component halo = gameObject.GetComponent("Halo");
        halo.GetType().GetProperty("enabled").SetValue(halo, state, null);
    }

    private IEnumerator runChuckSubInstance()
    {
        runChuckInstrument();
        yield return new WaitForSeconds(0.0f);
    }

	void runChuckInstrument()
	{
        myChuck.SetFloat("bpm", PlayLineController.currentTempo);

        myChuck.RunCode(@"
			global float bpm;
			global float midiNote;
			global float soundType;
			0.99*(60.0/bpm)/4.0 => float timeStep;

			Saxofony sax;
			SinOsc s1;
			TriOsc s2;
			SinOsc s3;
			SinOsc s4;

			<<<soundType>>>;
			
			// SET SPECS based on sound type (check paintGM.colorTags[] for change in length/list of options)
			if(soundType == 0) //voice
			{
				s1 => NRev re => dac;
				.5 => re.gain;
				.8 => re.mix;

				play2to5(midiNote);
			}
			else if(soundType == 1)
			{
				// patch
				sax => JCRev r => dac;
				.1 => r.gain;
				.2 => r.mix;
				// set specs
				.5 => sax.stiffness;
				.5 => sax.aperture;
				.5 => sax.noiseGain;
				.5 => sax.blowPosition;
				6 => sax.vibratoFreq;
				.5 => sax.vibratoGain;
				.5 => sax.pressure;
				
				play1(midiNote, .6);
			}
			else
			{
				s1 => NRev re => dac;
				.1 => re.gain;
				.1 => re.mix;

				play2to5(midiNote);
			}
			
			//SOUND FUNCTIONS
			fun void play1(float note, float velocity)
			{
				// start the note
				Std.mtof(note) => sax.freq;
				velocity => sax.noteOn;
				(4*timeStep)::second => now;
			}

			fun void play2to5(float note)
			{
				// start the note
				Std.mtof(note) => s1.freq;
				(4*timeStep)::second => now;
			}

			fun float setGain(float note)
			{
				return ( 0.05 + (note/127.0)/4 ); //max gain at 0.3
			}
			

		");

		//Debug.Log("ran chuck code");

	}
}
