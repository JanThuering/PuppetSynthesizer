using UnityEngine;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Core;

public class DryWetMidiMinimalExample : MonoBehaviour {
    InputDevice input;

    void OnEnable() {
        input = InputDevice.GetByName("MIDI DEVICE NAME");
        if (input != null) {
            input.EventReceived += OnEventReceived;
            input.StartEventsListening();
        }
    }

    void OnEventReceived(object sender, MidiEventReceivedEventArgs e) {
        var midiDevice = (MidiDevice)sender;
        Debug.Log($"Received MIDI event {e.Event} from {midiDevice.Name}");

        if (e.Event is NoteOnEvent noteOn) {
            Debug.Log($"Note {noteOn.NoteNumber} pressed with velocity {noteOn.Velocity}");
        }
    }

    void OnDisable() {
        if (input != null) {
            input.EventReceived -= OnEventReceived;
            input.Dispose();
        }
    }
}
