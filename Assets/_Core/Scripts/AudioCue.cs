using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "CUSTOM/AudioCue")]
public class AudioCue : ScriptableObject
{
    private const float MAX_PITCH = 3f;
    private static readonly Dictionary<AudioCue, AudioSource> sources = new Dictionary<AudioCue, AudioSource>();

    [SerializeField]
    private AudioSource sourcePrefab;
    [SerializeField]
    [Range(-MAX_PITCH, MAX_PITCH)]
    private float rndPitchMod = 0.05f;
    [SerializeField]
    [Range(0f, 1f)]
    private float rndVolumeMod = 0.05f;
    [SerializeField]
    private AudioClip[] clips;
    private int lastSelectedIndex = -1;
    private AudioSource sourceInstance;

    public AudioCue PlayOneShot(Vector3? position = null)
    {
        if (clips.Length == 0)
        {
            throw new System.IndexOutOfRangeException("No clips assigned to AudioCue");
        }
        var sourceInstance = GetSourceInstance();
        if (position.HasValue)
        {
            sourceInstance.transform.position = position.Value;
        }
        sourceInstance.pitch = rndPitchMod > 0f ? Mathf.Clamp(Random.Range(1f - rndPitchMod, 1f + rndPitchMod), -MAX_PITCH, MAX_PITCH) : 1f;
        sourceInstance.volume = rndVolumeMod > 0f ? Mathf.Clamp01(Random.Range(1f - rndVolumeMod, 1f + rndVolumeMod)) : 1f;
        sourceInstance.PlayOneShot(GetRandomClip());
        return this;
    }
    private AudioSource GetSourceInstance()
    {
        if (sourceInstance == null)
        {
            sourceInstance = Instantiate(sourcePrefab);
            sourceInstance.name = name;
        }
        return sourceInstance;
    }
    private AudioClip GetRandomClip()
    {

        int index = 0;
        if (clips.Length > 1)
        {
            do
            {
                index = Random.Range(0, clips.Length);
            }
            while (index == lastSelectedIndex);
        }
        lastSelectedIndex = index;
        return clips[index];
    }
}