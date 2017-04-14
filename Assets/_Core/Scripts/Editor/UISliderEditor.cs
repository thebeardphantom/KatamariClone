using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(UISlider))]
public class UISliderEditor : SliderEditor
{
    private SerializedProperty uiSliderVisuals;
    private SerializedProperty valueLabel;
    private SerializedProperty labelNormalizedValue;
    private SerializedProperty valueLabelFormat;

    protected override void OnEnable()
    {
        base.OnEnable();
        uiSliderVisuals = serializedObject.FindProperty("uiSliderVisuals");
        valueLabel = serializedObject.FindProperty("valueLabel");
        labelNormalizedValue = serializedObject.FindProperty("labelNormalizedValue");
        valueLabelFormat = serializedObject.FindProperty("valueLabelFormat");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(uiSliderVisuals, new GUIContent("UI Slider Visuals"));
        EditorGUILayout.PropertyField(labelNormalizedValue);
        EditorGUILayout.PropertyField(valueLabel);
        EditorGUILayout.PropertyField(valueLabelFormat);
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }
}
