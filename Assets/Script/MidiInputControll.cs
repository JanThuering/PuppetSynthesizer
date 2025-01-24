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

        // if (e.Event is NoteOnEvent noteOn) {
        //     Debug.Log($"Note {noteOn.NoteNumber} pressed with velocity {noteOn.Velocity}");
        // }

        if (e.Event is ControlChangeEvent controlChange) {
            Debug.Log($"Control {controlChange.ControlNumber} changed to {controlChange.ControlValue}");
            // Map 0-127 to -10 to 10
            float mappedValue = (controlChange.ControlValue / 127f) * 20f - 10f;   
            if(controlChange.ControlNumber == 1){
                _csCreateLine._amplitudeA = mappedValue;
            }
            if(controlChange.ControlNumber == 2){
                _csCreateLine._amplitudeB = mappedValue;
            }
            if(controlChange.ControlNumber == 3){
                _csCreateLine._amplitudeC = mappedValue;
            }
            if(controlChange.ControlNumber == 4){
                _csCreateLine._amplitudeD = mappedValue;
            }
            if(controlChange.ControlNumber == 5){
                _csCreateLine._amplitudeE = mappedValue;
            }
            if(controlChange.ControlNumber == 6){
                _csCreateLine._amplitudeF = mappedValue;
            }
            if(controlChange.ControlNumber == 10){
                _csCreateLine._frequency = controlChange.ControlValue/1;
            }
            
            if(controlChange.ControlNumber == 11){
                _csCreateLine._horizontalMovement = 2*controlChange.ControlValue;
            }
            
        }
        
        
    }

    void OnDisable() {
        if (input != null) {
            input.EventReceived -= OnEventReceived;
            input.Dispose();
        }
    }
}