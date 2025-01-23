using UnityEngine;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Core;

public class MidiInputControll : MonoBehaviour {
    [SerializeField] InputDevice input;
    [SerializeField] CreateLine _csCreateLine;


    void OnEnable() {
        input = InputDevice.GetByName("X-TOUCH COMPACT");
        if(input == null){
            Debug.LogError("MIDI device not found");
        }
        if (input != null) {
            input.EventReceived += OnEventReceived;
            input.StartEventsListening();
        }
    }
    

    void OnEventReceived(object sender, MidiEventReceivedEventArgs e) {
        var midiDevice = (MidiDevice)sender;
        //Debug.Log($"Received MIDI event {e.Event} from {midiDevice.Name}");
        Debug.Log(e.Event);
        
        /*
        if (e.Event is NoteOnEvent noteOn) {
            Debug.Log($"Note {noteOn.NoteNumber} pressed with velocity {noteOn.Velocity}");
        }
        */
    }



    /*
    void OnEventReceived(object sender, MidiEventReceivedEventArgs e) {
    var midiDevice = (MidiDevice)sender;

    if (e.Event is NoteOnEvent noteOn) {
        // Map MIDI velocity (0-127) to desired range and update CreateLine
        float mappedValue = noteOn.Velocity / 127f;  // Normalize to 0-1
        _csCreateLine._debugMidiValue = mappedValue;
        Debug.Log($"Updated MIDI value to {mappedValue}");
    }
    }
    */

    void OnDisable() {
        if (input != null) {
            input.EventReceived -= OnEventReceived;
            input.Dispose();
        }
    }
}