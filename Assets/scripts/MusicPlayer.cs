using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer instance;

    [SerializeField]
    private AudioClip[] Music;
    private AudioSource AudioPlayer;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else 
            Destroy(gameObject);
        DontDestroyOnLoad(this);
        AudioPlayer = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!AudioPlayer.isPlaying)
            AudioPlayer.PlayOneShot(Music[Random.Range(0, Music.Length)]);
    }
}
