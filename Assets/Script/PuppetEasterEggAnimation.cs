using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PuppetEasterEggAnimation : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private GameObject[] animationRigs;

    private bool animationIsTriggered = false;

    //Pirouette Animation
    private string pirouetteAnimation = "Pirouette";
    private string pirouetteTrigger = "pirouetteTrigger";
    
    //HandStand Animation
    private string handStandAnimation = "HandStand";
    private string handStandTrigger = "handStandTrigger";

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
            case 1: PlayAnimation(pirouetteAnimation, pirouetteTrigger); break;
            case 2: PlayAnimation(handStandAnimation, handStandTrigger); break;
            default: break;
        }
    }

    //AnimationControl
    private void PlayAnimation(string animationName, string triggerParameterName)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName) & !animationIsTriggered)
        {
            animationIsTriggered = true; // check that whole easter egg only is triggered once until it's done
            StartCoroutine(ConstraintWeightOff()); //animation rigging off

            animator.SetTrigger(triggerParameterName); //start animation
            StartCoroutine(WaitForAnimationToEnd(animationName)); //check if animation is still running, and if not -> animation rigging on
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
        float duration = 2f;
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