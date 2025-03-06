using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using DG.Tweening;

public class PuppetEasterEggAnimation : MonoBehaviour
{
    private Animator animator;
    private GlobalControl globalControl;
    [SerializeField] private float pirouetteLength;
    [SerializeField] private GameObject[] animationRigs;
    AnimatorStateInfo animStateInfo;
    private float NTime;
    private bool animationFinished;
    private bool isWeightOff;
    private bool isWeightOn;
    bool triggerOnce = true;

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
            case 1: PirouetteAnimation(); break;
            case 2: HandStandAnimation(); break;
            default: break;
        }
    }

    private void PirouetteAnimation()
    {
        //TODO rotation of whole thing doesn't work obviously, because the controlpoints dont rotate with - needs to be in the animation and needs to end up right where it startet.

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Pirouette") & triggerOnce)
        {
            triggerOnce = false;
            //animation rigging off
            StartCoroutine(ConstraintWeightOff());

            //start animation
            animator.SetBool("isPirouette", true);

            //turn Marionette 360degree
            gameObject.transform.DORotate(new Vector3(0, 360, 0), 3, RotateMode.FastBeyond360)
            .SetRelative()
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                //animation rigging on
                StartCoroutine(ConstraintWeightOn());
                //stop animation
                animator.SetBool("isPirouette", false);
            });
        }
    }

    private void HandStandAnimation()
    {
        //TODO can't be spammed, or breaks

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("HandStand"))
        {
            //animation rigging off
            StartCoroutine(ConstraintWeightOff());

            //trigger animation
            animator.SetTrigger("handStandTrigger");

            StartCoroutine(WaitForAnimationToEnd("HandStand reversed"));
        }
    }

    //Check ob animation läuft
    IEnumerator WaitForAnimationToEnd(string animationName)
    {
        AnimatorStateInfo animStateInfo;

        while (true)
        {
            animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (!animStateInfo.IsName(animationName) || !animStateInfo.IsName(animationName) && animStateInfo.normalizedTime < 1.0f)
            {
                yield return null; // Keep waiting
            }
            else 
            {
                StartCoroutine(ConstraintWeightOn());
                break; // Animation finished, exit loop
            }

            // Once animation finishes, turn animation rigging back on
        }
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

        isWeightOff = true;
    }

    //animationrigging on - 
    //!! Duration is set to 2f which makes the turn off smoother, but makes the animations need a cool off
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

        isWeightOff = false;
    }
}
