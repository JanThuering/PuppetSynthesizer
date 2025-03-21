using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PuppetEasterEggAnimation : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private GameObject[] animationRigs;

    private bool animationIsTriggered = false;
    private bool animationBool = false;

    //HandStand Animation
    private string handStandAnimation = "HandStand";
    private string handStandTrigger = "handStandTrigger";

    //Pirouette Animation
    private string pirouetteAnimation = "Pirouette";
    private string pirouetteTrigger = "pirouetteTrigger";

    //Relaxed Animation
    private string relaxAnimation = "Relax";
    private string relaxBool = "isRelaxed";

    // Start is called before the first frame update

    void OnEnable()
    {
        GlobalControl.CallEasteregg += EasterEggDances;
    }

    void OnDisable()
    {
        GlobalControl.CallEasteregg -= EasterEggDances;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    //Event picked die richtige funktion
    private void EasterEggDances(int danceType)
    {
        switch (danceType)
        {
            case 1: PlayAnimationViaTrigger(handStandAnimation, handStandTrigger); break;
            case 2: PlayAnimationViaTrigger(pirouetteAnimation, pirouetteTrigger); break;
            case 3: PlayAnimationViaBool(relaxAnimation, relaxBool); break;
            default: break;
        }
    }

    //AnimationControl
    private void PlayAnimationViaTrigger(string animationName, string animationTriggerParameterName)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName) & !animationIsTriggered)
        {
            animationIsTriggered = true; // check that whole easter egg only is triggered once until it's done
            StartCoroutine(ConstraintWeightOff()); //animation rigging off

            animator.SetTrigger(animationTriggerParameterName); //start animation
            StartCoroutine(WaitForAnimationToEnd(animationName)); //check if animation is still running, and if not -> animation rigging on
        }
    }

    private void PlayAnimationViaBool(string animationName, string animationBoolParameter)
    {
        animationBool = !animationBool; //switch bool to the opposite

        if (animationBool != animator.GetBool(animationBoolParameter))
        {
            animationIsTriggered = true; // check that whole easter egg only is triggered once until it's done
            StartCoroutine(ConstraintWeightOff()); //animation rigging off 
            
            if (animationBool == false) StartCoroutine(WaitForAnimationToEnd(animationName)); //if animation turned off -> animation rigging on
            animator.SetBool(animationBoolParameter, animationBool); //start animation
        }
    }

    IEnumerator WaitForAnimationToEnd(string animationName)
    {
        AnimatorStateInfo animStateInfo;

        // Wait for animation to start
        do
        {
            animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        } while (!animStateInfo.IsName(animationName));

        // Wait for animation to finish
        do
        {
            animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        } while (animStateInfo.normalizedTime < 1f);

        //turn constraint back on
        StartCoroutine(ConstraintWeightOn());
    }

    //animationrigging off
    IEnumerator ConstraintWeightOff()
    {
        float lerpedWeight = 1;
        float duration = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            //lerp weight value
            lerpedWeight = Mathf.Lerp(1, 0, elapsedTime / duration);

            //apply weight
            for (int i = 0; i < animationRigs.Length; i++)
            {
                animationRigs[i].GetComponent<TwistChainConstraint>().weight = lerpedWeight;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final value is correctly set
        foreach (var rig in animationRigs)
        {
            rig.GetComponent<TwistChainConstraint>().weight = 0;
        }
    }

    //animationrigging on
    IEnumerator ConstraintWeightOn()
    {
        float lerpedWeight;
        float duration = 1.5f;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            //lerp weight value
            lerpedWeight = Mathf.Lerp(0, 1, elapsedTime / duration);

            //apply weight
            for (int i = 0; i < animationRigs.Length; i++)
            {
                animationRigs[i].GetComponent<TwistChainConstraint>().weight = lerpedWeight;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final value is correctly set
        foreach (var rig in animationRigs)
        {
            rig.GetComponent<TwistChainConstraint>().weight = 1;
        }

        animationIsTriggered = false; //make animation triggerable again
    }
}