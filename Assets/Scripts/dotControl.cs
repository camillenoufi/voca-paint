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
            runChuckInstrument();
            coroutine = HaloOnOff(3.0f);
            StartCoroutine(coroutine);
			/*
			if (gameObject.CompareTag("adc"))
				Debug.Log("adc collide");
			else
				Debug.Log("color collide");
			*/
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

	void runChuckInstrument()
	{
        // get midi note position of current dot
        myChuck.SetFloat("bpm", PlayLineController.currentTempo);

        myChuck.RunCode(@"
			global float bpm;
			global float midiNote;
			global float soundType;
			0.99*(60.0/bpm)/4.0 => float timeStep;
			
			// patch
			Saxofony sax => JCRev r => dac;
			.2 => r.gain;
			.1 => r.mix;

			// set
			.5 => sax.stiffness;
			.5 => sax.aperture;
			.5 => sax.noiseGain;
			.5 => sax.blowPosition;
			6 => sax.vibratoFreq;
			.5 => sax.vibratoGain;
			.5 => sax.pressure;
			// factor
			1.25 => float factor;
			
			play(midiNote, .7);
			timeStep::second => now;

			// basic play function (add more arguments as needed)
			fun void play(float note, float velocity)
			{
				// start the note
				Std.mtof(note) => sax.freq;
				velocity => sax.noteOn;
			}

		");

		Debug.Log("ran chuck code");

	}
}
