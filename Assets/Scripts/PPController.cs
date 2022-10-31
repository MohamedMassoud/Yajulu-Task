using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PPController : MonoBehaviour
{
    [SerializeField] private Volume PPNormal;
    [SerializeField] private Volume PPHit;

    private void LoadNormalPP()
    {
        LeanTween.value(0, 1, 0.5f).setOnUpdate((float value) =>
        {
            PPNormal.weight = value;
            PPHit.weight = 1 - value;
        });
    }

    private void LoadHitPP(Action onEndAction)
    {
        LeanTween.value(0, 1, 0.5f).setOnUpdate((float value) =>
        {
            PPHit.weight = value;
            PPNormal.weight = 1 - value;
            onEndAction?.Invoke();
        });
    }

    public void PlayHitEffect()
    {
        LoadHitPP(LoadNormalPP);
    }
}
