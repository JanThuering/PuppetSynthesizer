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

            if (e.Event.EventType == MidiEventType.ControlChange){

            }
            Debug.Log($"Control {controlChange.ControlNumber} changed to {controlChange.ControlValue}");

            
            _csCreateLine.InputToValues(controlChange.ControlNumber, controlChange.ControlValue, 127f);

            
        }
        
        
    }

    void OnDisable() {
        if (input != null) {
            input.EventReceived -= OnEventReceived;
            input.Dispose();
        }
    }
}