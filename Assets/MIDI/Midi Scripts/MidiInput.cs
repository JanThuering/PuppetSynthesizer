using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Multimedia;

public static class MidiInput {
    struct NoteData {
        public byte velocity;
        // The following two flags are used to ensure that notes persist for at least one frame.
        public bool wasSetThisFrame; // The note was set this frame and should not be removed.
        public bool receivedNoteOff; // The note was released this frame but was blocked by wasSetThisFrame.
                                     // It will be removed next frame.
    }

    static readonly Dictionary<(byte channel, byte note), NoteData> activeNotes = new();
    static readonly Dictionary<(byte channel, byte controlNumber), byte> ccValues = new();
    static readonly List<InputDevice> inputDevices = new();
    static readonly List<OutputDevice> outputDevices = new();

    static MidiInput() {
        // Clean Up
        foreach (var device in inputDevices) {
            device.StopEventsListening();
            device.EventReceived -= OnMidiEventReceived;
            device.Dispose();
        }
        inputDevices.Clear();

        foreach (var device in outputDevices) {
            device.Dispose();
        }
        outputDevices.Clear();

        // Initialize
        foreach (var device in InputDevice.GetAll()) {
            inputDevices.Add(device);
            device.EventReceived += OnMidiEventReceived;
            device.StartEventsListening();
        }

        outputDevices.AddRange(OutputDevice.GetAll());
    }

    static void OnMidiEventReceived(object sender, MidiEventReceivedEventArgs e) {
        if (e.Event is ChannelEvent channelEvent) {
            var channel = channelEvent.Channel;
            switch (channelEvent) {
                case NoteOnEvent noteOn when noteOn.Velocity > 0:
                    activeNotes[(channel, noteOn.NoteNumber)] = new NoteData {
                        velocity = noteOn.Velocity,
                        wasSetThisFrame = true
                    };
                    break;
                case NoteOffEvent noteOff:
                    if (activeNotes.TryGetValue((channel, noteOff.NoteNumber), out var noteState)) {
                        if (noteState.wasSetThisFrame) {
                            // Block the note off event if the note was set this frame.
                            // But mark it for removal next frame (removed in GetNote)
                            noteState.receivedNoteOff = true;
                            activeNotes[(channel, noteOff.NoteNumber)] = noteState;
                        } else {
                            activeNotes.Remove((channel, noteOff.NoteNumber));
                        }
                    }
                    break;
                case ControlChangeEvent controlChange:
                    ccValues[(channel, controlChange.ControlNumber)] = controlChange.ControlValue;
                    break;
            }
        }
    }

    /// <summary>
    /// Returns true if the specified note is currently pressed on any input device.
    /// Optionally returns the velocity of the note.
    /// </summary>
    public static bool GetNote(byte channel, byte note) {
        return GetNote(channel, note, out _);
    }

    /// <summary>
    /// Returns true if the specified note is currently pressed on any input device.
    /// Optionally returns the velocity of the note.
    /// </summary>
    public static bool GetNote(byte channel, byte note, out byte velocity) {
        var key = (channel, note);
        if (activeNotes.TryGetValue(key, out var noteData)) {
            if (noteData.wasSetThisFrame) {
                // The note was set this frame, so it is pressed.
                activeNotes[key] = new NoteData {
                    velocity = noteData.velocity,
                    wasSetThisFrame = false
                };
                velocity = noteData.velocity;
                return true;
            }

            if (noteData.receivedNoteOff) {
                // The note was released, but was blocked by wasSetThisFrame.
                // Remove it now.
                activeNotes.Remove(key);
                velocity = 0;
                return false;
            }

            // The note is still active.
            velocity = noteData.velocity;
            return true;
        }

        // If note is not found, it is not pressed.
        velocity = 0;
        return false;
    }

    /// <summary>
    /// Sends a MIDI Note On message to all output devices.
    /// Optionally specify the velocity (default 127).
    /// </summary>
    public static void SendNoteOn(byte channel, byte note, byte velocity = 127) {
        foreach (var device in outputDevices) {
            var noteOn = new NoteOnEvent((SevenBitNumber)note, (SevenBitNumber)velocity) {
                Channel = (FourBitNumber)channel
            };
            device.SendEvent(noteOn);
        }
    }

    /// <summary>
    /// Sends a MIDI Note Off message to all output devices.
    /// </summary>
    public static void SendNoteOff(byte channel, byte note) {
        foreach (var device in outputDevices) {
            var noteOff = new NoteOffEvent((SevenBitNumber)note, SevenBitNumber.MinValue) {
                Channel = (FourBitNumber)channel
            };
            device.SendEvent(noteOff);
        }
    }

    /// <summary>
    /// Retrieves the value of a Control Change (e.g. a knob) on a given MIDI channel (any device).
    /// Returns the value, or zero if unset.
    /// </summary>
    public static byte GetCC(byte channel, byte controlNumber) {
        return ccValues.TryGetValue((channel, controlNumber), out var value) ? value : (byte)0;
    }

    /// <summary>
    /// Sends a MIDI Control Change message to all output devices.
    /// </summary>
    public static void SendCC(byte channel, byte controlNumber, byte value) {
        foreach (var device in outputDevices) {
            var controlChange = new ControlChangeEvent(
                (SevenBitNumber)controlNumber,
                (SevenBitNumber)value
            ) {
                Channel = (FourBitNumber)channel
            };
            device.SendEvent(controlChange);
        }
    }
}
