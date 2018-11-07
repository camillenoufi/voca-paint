using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChuckSoundController : MonoBehaviour {

    //public static float objSoundtype = 1;
	
	private ChuckSubInstance myChuck;
    private float midiPos;
	
	// Use this for initialization
	void Start () {

        midiPos = ConvertYPosToMidiNote(gameObject.transform.position.y);
		
		myChuck = GetComponent<ChuckSubInstance>();
        myChuck.SetFloat("soundType", paintGM.soundTags[gameObject.tag]);
        myChuck.SetFloat("bpm", PlayLineController.currentTempo);
        myChuck.SetFloat("midiNote", midiPos);
        runChuckInstrument();
		Debug.Log(PlayLineController.currentTempo);
        StartCoroutine(DestroyChuckSound(0.5f));
	}
	

    float ConvertYPosToMidiNote(float y)
    {
        float midi = Mathf.Round((2.0f * paintGM.modOperator) - paintGM.modOperatorOffset + y);
        return midi;
    }

    void runChuckInstrument()
    {
        myChuck.RunCode(@"
			global float bpm;
			global float midiNote;
			global float soundType;
			0.99*(60.0/bpm)/4.0 => float timeStep;
			timeStep::second => dur T;

			if(soundType == 1) //pink
				playSax(midiNote, .6);
			else if(soundType == 3) //yellow
				playSin(midiNote);
			else if(soundType == 5) //blue
				playSinLPF(midiNote);
			
			//SOUND FUNCTIONS
			fun void playSax(float note, float velocity)
			{
				// patch
				Saxofony sax => JCRev r => dac;
				.2 => r.gain;
				.2 => r.mix;
				// set specs
				.5 => sax.stiffness;
				.5 => sax.aperture;
				.5 => sax.noiseGain;
				.5 => sax.blowPosition;
				6 => sax.vibratoFreq;
				.5 => sax.vibratoGain;
				.5 => sax.pressure;
				
				// start the note
				Std.mtof(note) => sax.freq;
				sax.gain(setGain(note));
				velocity => sax.noteOn;
				2*T => now;
			}

			fun void playSin(float note)
			{
				SinOsc s1 => HPF hpf => ADSR e => NRev re => dac;
				50 => hpf.freq;
				.1 => re.gain;
				.1 => re.mix;
				e.set( 10::ms, 5::ms, .5, 20::ms );  //(a,d,s height % of freq,r)
				
				// start the note
				Std.mtof(note) => s1.freq;
				s1.gain(setGain(note));
				e.keyOn();// press the key
				2*T - e.releaseTime() => now; // play/wait until beginning of release
				e.keyOff(); //release the key
				e.releaseTime() => now; // wait until release is done
			}

			fun void playSinLPF(float note)
			{
				SinOsc s1 => LPF lpf => ADSR e => NRev re => dac;
				500 => lpf.freq;
				.1 => re.gain;
				.3 => re.mix;
				e.set( 30::ms, 5::ms, .5, 20::ms );  //(a,d,s height % of freq,r)
				
				// start the note
				Std.mtof(note) => s1.freq;
				s1.gain(setGain(note));
				e.keyOn();// press the key
				2*T - e.releaseTime() => now; // play/wait until beginning of release
				e.keyOff(); //release the key
				e.releaseTime() => now; // wait until release is done
			}

			fun float setGain(float note)
			{
				return ( 0.05 + (note/127.0)/2 );
			}
			

		");

    }

    private IEnumerator DestroyChuckSound(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
