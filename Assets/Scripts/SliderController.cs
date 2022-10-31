using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    [SerializeField] private Gradient colorGradient;
    private Slider slider;
    [SerializeField] Image fillImage;
    void Start()
    {
        slider = GetComponent<Slider>();
    }

    public void AddValueToSlider(float value)
    {
        float finalValue = slider.value + value;
        if (finalValue < 0) finalValue = 0;
        else if (finalValue > slider.maxValue) finalValue = slider.maxValue;


        LeanTween.value(slider.value, finalValue, 0.25f).setOnUpdate((float tweenedValue) => {
            
            slider.value = tweenedValue;
            Color newColor = colorGradient.Evaluate(slider.normalizedValue);

            if(fillImage != null)fillImage.color = newColor;
        });

    }
    
    public float GetSliderValue()
    {
        return slider.value;
    }
}
