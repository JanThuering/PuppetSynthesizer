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

    private void EasterEggDances(int danceType)
    {
        //TODO lerp
        for (int i = 0; i < animationRigs.Length; i++)
        {
            animationRigs[i].GetComponent<TwistChainConstraint>().weight = 0;
        }

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

    private void HandStandAnimation()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("HandStand"))
        {
            animator.SetBool("isHandStand", true);
        }

        animator.SetBool("isHandStand", false);
    }

    private void PirouetteAnimation()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Pirouette"))
        {
            animator.SetBool("isPirouette", true);

            //turn Marionette 360degree

            gameObject.transform.DORotate(new Vector3(0, 360, 0), 4, RotateMode.FastBeyond360)
            .SetRelative()
            .SetEase(Ease.InOutExpo)
            .OnComplete(() => PirouetteStop());
        }
    }

    private void PirouetteStop()
    {
        for (int i = 0; i < animationRigs.Length; i++)
        {
            animationRigs[i].GetComponent<TwistChainConstraint>().weight = 1;
        }
        animator.SetBool("isPirouette", false);
    }

    IEnumerator GetEndOfAnimation()
    {
        while (animationFinished == false)
        {
            animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            NTime = animStateInfo.normalizedTime;
            if (NTime > 1.0f)
            {
                animationFinished = true;
                for (int i = 0; i < animationRigs.Length; i++)
                {
                    animationRigs[i].GetComponent<TwistChainConstraint>().weight = 1;
                }
            }
            yield return null;
        }
    }
}
