using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Core;

public enum EventType { NoteDown, NoteUp, ControlChange }

public class MidiActions : MonoBehaviour {
    [System.Serializable]
    public class MidiAction {
        [HideInInspector] public string name;
        public EventType eventType;
        public byte noteOrCC;
        public UnityEvent<int> action;
    }

    public MidiAction[] actions;

    readonly List<InputDevice> inputDevices = new();
    readonly Queue<(UnityEvent<int> action, int value)> mainThreadActions = new();

    void OnValidate() {
        // Update action names
        if (actions == null) return;
        foreach (var a in actions) {
            a.name = $"{a.eventType} {a.noteOrCC}";
        }
    }

    void OnEnable() {
        // Just subscribe to all devices
        foreach (var device in InputDevice.GetAll()) {
            inputDevices.Add(device);

            device.EventReceived += OnEventReceived;
            device.StartEventsListening();
        }
    }

    void OnEventReceived(object sender, MidiEventReceivedEventArgs e) {
        foreach (var a in actions) {
            switch (e.Event) {
                case NoteOnEvent noteOn when a.eventType == EventType.NoteDown
                              && noteOn.NoteNumber == a.noteOrCC:
                    lock (mainThreadActions) {
                        mainThreadActions.Enqueue((a.action, noteOn.Velocity));
                    }
                    break;
                case NoteOffEvent noteOff when a.eventType == EventType.NoteUp
                               && noteOff.NoteNumber == a.noteOrCC:
                    lock (mainThreadActions) {
                        mainThreadActions.Enqueue((a.action, 0));
                    }
                    break;
                case ControlChangeEvent controlChange when a.eventType == EventType.ControlChange
                               && controlChange.ControlNumber == a.noteOrCC:
                    lock (mainThreadActions) {
                        mainThreadActions.Enqueue((a.action, controlChange.ControlValue));
                    }
                    break;
            }
        }
    }

    void Update() {
        lock (mainThreadActions) {
            while (mainThreadActions.Count > 0) {
                var (action, value) = mainThreadActions.Dequeue();
                action.Invoke(value);
            }
        }
    }

    void OnDisable() {
        foreach (var device in inputDevices) {
            device.StopEventsListening();
            device.EventReceived -= OnEventReceived;
        }
    }
}
