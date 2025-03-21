using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    //References to other scripts
    private GlobalControl globalControl;

    //FMOD
    private EventInstance eventInstance; 
    public string fmodEvent;
    public string parameterName = "MyParameter";
    

    //Variables
    private float parameterValue;

    // Start is called before the first frame update
    void Start()
    {
        eventInstance = RuntimeManager.CreateInstance(fmodEvent);
        eventInstance.start();
    }

    // Update is called once per frame
    void Update()
    {
        // Update the FMOD parameter using the Unity variable.
        eventInstance.setParameterByName(parameterName, parameterValue);
    }

    private void OnDestroy()
    {
        // Stop and release the FMOD event instance.
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        eventInstance.release();
    }
}
