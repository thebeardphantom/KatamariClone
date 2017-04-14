using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "CUSTOM/UISliderVisuals")]
public class UISliderVisuals : ScriptableObject
{
    public bool rotateHandle;
    public Sprite[] handleImages;
    public Vector3 endScale;
}