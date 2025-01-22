using UnityEngine;

public class MidiInputExample : MonoBehaviour {
    void Update() {
        // Note
        if (MidiInput.GetNote(0, 60)) {
            Debug.Log($"Note 60 pressed");
        }

        // Note with velocity
        if (MidiInput.GetNote(0, 61, out var velocity)) {
            Debug.Log($"Note 61 pressed with velcity {velocity}");
        }

        // Control change
        var position = transform.position;
        position.y = MidiInput.GetCC(0, 1);
        transform.position = position;

        // Send MIDI messages
        if (Input.GetKeyDown(KeyCode.Space)) {
            MidiInput.SendNoteOn(0, 63);
            MidiInput.SendCC(0, 2, 127);
        }
    }
}
