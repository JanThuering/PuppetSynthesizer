using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using System;
using UnityEngine.Animations.Rigging;
using DG.Tweening;

public class PuppetEasterEggAnimation : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private GameObject[] animationRigs;

    // Start is called before the first frame update

    void OnEnable()
    {
        // GlobalControl.OnPirouette += Pirouette;
    }

    void OnDisable()
    {
        // GlobalControl.OnPirouette -= Pirouette;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

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
            gameObject.transform.DORotate(Vector3.one * 360, 4).
            OnComplete(() => PirouetteStop());

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
