using UnityEngine;
using UnityCommonLibrary.Utility;

public class GameConfiguration
{
    public static readonly string path = PathUtility.Combine(Application.dataPath, "config.json");

    public float shadowDistance = 500f;
    public float masterVolume = 0f;
    public float musicVolume = -10f;
    public float sfxVolume = 0f;
    public float uiVolume = 0f;
    public bool anisotropicFiltering;
}