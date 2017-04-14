using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UISlider : Slider
{
    private const float HANDLE_ROTATIONS = 2f;

    [SerializeField]
    private UISliderVisuals uiSliderVisuals;
    [SerializeField]
    private Text valueLabel;
    [SerializeField]
    private bool labelNormalizedValue;
    [SerializeField]
    private string valueLabelFormat = "{0}";

    protected override void Set(float input, bool sendCallback)
    {
        base.Set(input, sendCallback);
        if (uiSliderVisuals != null)
        {
            if (uiSliderVisuals.rotateHandle)
            {
                handleRect.rotation = Quaternion.Euler(0f, 0f, -359f * normalizedValue * HANDLE_ROTATIONS);
            }
            // Update visual representation
            var index = Mathf.RoundToInt((uiSliderVisuals.handleImages.Length - 1) * normalizedValue);
            (targetGraphic as Image).sprite = uiSliderVisuals.handleImages[index];
            handleRect.localScale = Vector3.Lerp(Vector3.one, uiSliderVisuals.endScale, normalizedValue);
        }
        if (valueLabel != null)
        {
            valueLabel.text = string.Format(valueLabelFormat, labelNormalizedValue ? normalizedValue : value);
        }
    }
}