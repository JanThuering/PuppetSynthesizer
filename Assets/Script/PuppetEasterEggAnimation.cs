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
    public bool isPirouetting;

    // Start is called before the first frame update

    void OnEnable()
    {
        GlobalControl.CallEasteregg += PirouetteStart;
    }

    void OnDisable()
    {
        GlobalControl.CallEasteregg -= PirouetteStart;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isPirouetting) PirouetteStart();
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
            gameObject.transform.DORotate(Vector3.one * 360, 4, RotateMode.FastBeyond360)
            .SetRelative()
            .SetEase(Ease.OutExpo)
            .OnComplete(() => PirouetteStop());

        }
    }

    private void PirouetteStop()
    {
        isPirouetting = false;
        for (int i = 0; i < animationRigs.Length; i++)
        {
            animationRigs[i].GetComponent<TwistChainConstraint>().weight = 1;
        }
        animator.SetBool("isPirouette", false);
    }
}
