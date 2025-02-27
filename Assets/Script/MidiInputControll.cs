using UnityEngine;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Core;

public class MidiInputControll : MonoBehaviour {
    [SerializeField] InputDevice input;
    CreateLine createLineScript;
    GlobalControl globalControlScript;

    private void Start()
    {
        FillAllParameters();
    }

    private void FillAllParameters(){
        createLineScript = CreateLine.Instance;
        globalControlScript = GlobalControl.Instance;
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

        if (e.Event is ControlChangeEvent controlChange) {
            //GLOBAL - Control the wave in GlobalControl
            if(controlChange.ControlNumber >= 7 && controlChange.ControlNumber <= 9){
                globalControlScript.MidiGlobalWave(controlChange.ControlNumber, controlChange.ControlValue, 127f);
            }

            //AMPLITUDE - Control amplitudes of the individual Waves 
            if(controlChange.ControlNumber >= 1 && controlChange.ControlNumber <= 4){
                globalControlScript.MidiAmplitudeWave(controlChange.ControlNumber, controlChange.ControlValue, 127f);
            }

            //SPEED - Control speed of the individual Waves
            if(controlChange.ControlNumber >= 10 && controlChange.ControlNumber <= 13){
                globalControlScript.MidiSpeedWave(controlChange.ControlNumber, controlChange.ControlValue, 127f);
            }

            //TYPE - Control speed of the individual Waves
            if(controlChange.ControlNumber >= 32 && controlChange.ControlNumber <= 35 || controlChange.ControlNumber >= 15 && controlChange.ControlNumber <= 18){
                globalControlScript.MidiWaveType(controlChange.ControlNumber, controlChange.ControlValue, 127f, true);
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