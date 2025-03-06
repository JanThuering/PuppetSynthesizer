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
    [SerializeField] private GameObject[] animationRigs;
    AnimatorStateInfo animStateInfo;
    private float NTime;
    private bool animationFinished;
    private bool isWeightOff;
    private bool isWeightOn;

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
        //TODO lerp


        switch (danceType)
        {
            case 1: PirouetteAnimation(); break;
            case 2: HandStandAnimation(); break;
            default: break;
        }

        //Check if Animation finished
        animationFinished = false;
        StartCoroutine(GetEndOfAnimation());
    }

    private void PirouetteAnimation()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Pirouette"))
        {

            animator.SetBool("isPirouette", true);

            //animation rigging off
            StartCoroutine(ConstraintWeightOff());

            //turn Marionette 360degree
            gameObject.transform.DORotate(new Vector3(0, 360, 0), 4, RotateMode.FastBeyond360)
            .SetRelative()
            .SetEase(Ease.InOutExpo)
            .OnComplete(() => PirouetteStop());
        }
    }

    private void PirouetteStop()
    {
        //animation rigging on
        StartCoroutine(ConstraintWeightOn());
        //Stopa animation
        animator.SetBool("isPirouette", false);
    }

    private void HandStandAnimation()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("HandStand"))
        {
            //animation rigging off
            StartCoroutine(ConstraintWeightOff());

            //animations bool kurz on setzen damit animation nur einmal abgespielt wird
            animator.SetBool("isHandStand", true);
            animator.SetBool("isHandStand", false);
        }
    }

    //Check ob animation läuft
    IEnumerator GetEndOfAnimation()
    {
        while (animationFinished == false)
        {
            animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            NTime = animStateInfo.normalizedTime;
            if (NTime > 1.0f)
            {
                animationFinished = true;
                StartCoroutine(ConstraintWeightOn());
            }
            yield return null;
        }
    }

    //animationrigging on
    IEnumerator ConstraintWeightOn()
    {
        float lerpedWeight;
        float lerpT = 2;

        while (isWeightOff)
        {
            //lerp weight value
            lerpedWeight = Mathf.Lerp(0, 1, Time.deltaTime * lerpT);

            //apply weight
            for (int i = 0; i < animationRigs.Length; i++)
            {
                animationRigs[i].GetComponent<TwistChainConstraint>().weight = lerpedWeight;
            }
            
            yield return null;
        }

        isWeightOff = false;

    }

    //animationrigging off
    IEnumerator ConstraintWeightOff()
    {
        float lerpedWeight;
        float lerpT = 2;

        while (isWeightOff == false)
        {
            //lerp weight value
            lerpedWeight = Mathf.Lerp(1, 2, Time.deltaTime * lerpT);

            //apply weight
            for (int i = 0; i < animationRigs.Length; i++)
            {
                animationRigs[i].GetComponent<TwistChainConstraint>().weight = lerpedWeight;
            }
            
            yield return null;
        }

        isWeightOff = true;
    }
}
