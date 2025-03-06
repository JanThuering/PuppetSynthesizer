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

    // Update is called once per frame
    void Update()
    {

    }

    private void EasterEggDances(int danceType){
        switch (danceType)
        {
            case 1: PirouetteStart(); break;
            case 2: TestDance(); break;
            default: break;
        }
    }

    private void TestDance(){
        Debug.Log("Test Dance");
    }

    private void PirouetteStart()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Pirouette"))
        {
            for (int i = 0; i < animationRigs.Length; i++)
            {
                animationRigs[i].GetComponent<TwistChainConstraint>().weight = 0;
            }
            animator.SetBool("isPirouette", true);
            gameObject.transform.DORotate(new Vector3(0, 360, 0), 4, RotateMode.FastBeyond360)
            .SetRelative()
            .SetEase(Ease.OutExpo)
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
}
