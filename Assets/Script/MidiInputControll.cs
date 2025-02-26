using UnityEngine;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Core;

public class MidiInputControll : MonoBehaviour {
    [SerializeField] InputDevice input;
    CreateLine createLineScript;

    private void Start()
    {
        createLineScript = CreateLine.Instance;
    }

    void OnEnable() {
        input = InputDevice.GetByName("X-TOUCH COMPACT"); // X-TOUCH COMPACT
        //input = InputDevice.GetByName("Launch Control XL"); // Launch Control XL
        //input = InputDevice.GetByName("Arduino Leonardo"); // Arduino Leonardo
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
        //Debug.Log(e.Event);
        
        // if (e.Event is NoteOnEvent noteOn) {
        //     Debug.Log($"Note {noteOn.NoteNumber} pressed with velocity {noteOn.Velocity}");
        // }

        if (e.Event is ControlChangeEvent controlChange) {

            //Debug.Log($"ControlNumber: {controlChange.ControlNumber} ControlValue: {controlChange.ControlValue}");

            if(controlChange.ControlNumber >= 1 && controlChange.ControlNumber <= 4){
                createLineScript.MidiAmplitudeWave(controlChange.ControlNumber, controlChange.ControlValue, 127f);
            }


            // //Control the curve in CreateLineScript
            // if (controlChange.ControlNumber < 20){
            //     createLineScript.MidiControlWave(controlChange.ControlNumber, controlChange.ControlValue, 127f);
            // }
            // if(controlChange.ControlNumber > 20){
            //     createLineScript.MidiDefineWaveType(controlChange.ControlNumber, controlChange.ControlValue, 127f);
            // }
            

            
        }
        
        
    }

    void OnDisable() {
        if (input != null) {
            input.EventReceived -= OnEventReceived;
            input.Dispose();
        }
    }
}