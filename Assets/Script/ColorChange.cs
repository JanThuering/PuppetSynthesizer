using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ColorChange : MonoBehaviour
{
    [Range(0, 127)]
    public float CurrentColor = 127/2;
    private int amountOfColorEffects = 3;
    private bool isColorChange1 = false;
    private bool isColorChange2 = false;


    [Header("Materials")]
    [SerializeField] private Material[] currentMaterials;
    [SerializeField] private Material[] baseMaterials;
    [SerializeField] private Material[] colorChange1Materials;
    [SerializeField] private Material[] colorChange2Materials;

    [Header("Outline")]
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private Color[] outlineColors;
    [SerializeField] private Color currentOutlineColor;

    //Checks
    private bool isbaseColorApplied = false;
    private bool isColorChange1Applied = false;
    private bool isColorChange2Applied = false;


    // Start is called before the first frame update
    void Start()
    {
        if (isColorChange1 == false && isColorChange2 == false && isbaseColorApplied == false) BaseColor();
    }

    // Update is called once per frame
    void Update()
    {
        ColorApplier();
        if (isColorChange1 == false && isColorChange2 == false && isbaseColorApplied == false) BaseColor(); //default Color
        if (isColorChange1 && isColorChange1Applied == false) ColorEffectOne(); //Color Effect 1
        if (isColorChange2 && isColorChange2Applied == false) ColorEffectTwo(); //Color Effect 2

        currentOutlineColor = outlineMaterial.GetColor("_OutlineColor");

    }

    private void ColorApplier()
    {
        float effectOneRange = 127 / amountOfColorEffects; 
        float baseRange = effectOneRange * 2;
        float effectTwoRange = effectOneRange * 3;

        if (CurrentColor <= effectOneRange)
        {
            isColorChange1 = true;
            isColorChange2 = false;
        }
        else if (CurrentColor <= baseRange)
        {
            isColorChange1 = false;
            isColorChange2 = false;  
        }
        else if (CurrentColor <= effectTwoRange)
        {
            isColorChange1 = false;
            isColorChange2 = true;
        }
    }

    private void BaseColor()
    {
        isbaseColorApplied = true;
        isColorChange1Applied = false;
        isColorChange2Applied = false;

        if(isColorChange1 || isColorChange2) 
        {
            isColorChange1 = false;
            isColorChange2 = false;
        }
        
        ApplyColor(baseMaterials);
        ApplyOutlineColor(outlineColors[1]);
    }

    private void ColorEffectOne()
    {
        isbaseColorApplied = false;
        isColorChange1Applied = true;
        isColorChange2Applied = false;

        if(isColorChange2) isColorChange2 = false;

        ApplyColor(colorChange1Materials);
        ApplyOutlineColor(outlineColors[0]);
    }
    private void ColorEffectTwo()
    {
        isbaseColorApplied = false;
        isColorChange1Applied = false;
        isColorChange2Applied = true;

        if(isColorChange1) isColorChange1 = false;

        ApplyColor(colorChange2Materials);
        ApplyOutlineColor(outlineColors[2]);
    }

    private void ApplyColor(Material[] colorUpdateMaterial)
    {
        for (int i = 0; i < currentMaterials.Length; i++)
        {
            currentMaterials[i].color = colorUpdateMaterial[i].color;
        }
    }

    private void ApplyOutlineColor(Color outlineColor)
    {
        
        outlineMaterial.SetColor("_OutlineColor", outlineColor);
        
    }
}
