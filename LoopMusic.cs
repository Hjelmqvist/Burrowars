using System.Collections.Generic;
using UnityEngine;

public class LoopMusic : MonoBehaviour
{
    [SerializeField] AudioSource source = null;
    [SerializeField] AudioClip[] clips = null;

    List<AudioClip> clipsLeft = new List<AudioClip>();

    void Start()
    {
        FillClips();
        source.clip = GetRandomClip();
        source.Play();
    }

    void Update()
    {
        if (!source.isPlaying)
        {
            source.clip = GetRandomClip();
            source.Play();
        }
    }

    void FillClips()
    {
        foreach (AudioClip clip in clips)
            clipsLeft.Add(clip);
    }

    AudioClip GetRandomClip()
    {
        if (clipsLeft.Count == 0)
            FillClips();

        int i = Random.Range(0, clipsLeft.Count);
        AudioClip clip = clipsLeft[i];
        clipsLeft.RemoveAt(i);

        return clip;
    }
}
