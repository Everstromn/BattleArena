using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    public static SoundsManager instance;
    private void Awake()
    {

        int gameManagerCount = FindObjectsOfType<SoundsManager>().Length;

        if (gameManagerCount > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

        if (instance != null)
        {
            Debug.LogError("More than one Sounds Manager");
        }
        instance = this;
    }

    public AudioSource musicSource;
    [Range(0, 1)] public float startingVolume = 0.25f;

    public AudioClip[] musicClips;
    public bool currentlyPlayingTrack = false;
    private int currentTrackNo = 0;

    public AudioSource sfxSource;
    [Range(0, 1)] public float sfxVolume = 0.5f;
    public AudioClip clickSound;
    public AudioClip victorySound;
    public AudioClip defeatSound;
    public AudioClip defaultMoveSound;
    public AudioClip defaultAttackSound;


    public void StartNextTrack()
    {
        if (currentTrackNo < musicClips.Length)
        {
            musicSource.clip = musicClips[currentTrackNo + 1];
            currentTrackNo = currentTrackNo + 1;
        }
        else
        {
            musicSource.clip = musicClips[0];
            currentTrackNo = 0;
        }

        StartCoroutine(DelayNextTrack(musicSource.clip.length));
        musicSource.Play();

    }

    public void Start()
    {
        currentTrackNo = -1;
        StartNextTrack();
        musicSource.volume = startingVolume;
    }

    private IEnumerator DelayNextTrack(float timeToDelay)
    {
        yield return new WaitForSeconds(timeToDelay);
        StartNextTrack();
    }

    public void PlayClickSound() { sfxSource.PlayOneShot(clickSound,sfxVolume); }
    public void PlayVictorySound() { sfxSource.PlayOneShot(victorySound, sfxVolume); }
    public void PlayDefeatSound() { sfxSource.PlayOneShot(defeatSound, sfxVolume); }
    public void PlayMoveSound() { sfxSource.PlayOneShot(defaultMoveSound, sfxVolume); }
    public void PlayAttackSound() { sfxSource.PlayOneShot(defaultAttackSound, sfxVolume); }
    public void PlayGivenSound(AudioClip _clip) { sfxSource.PlayOneShot(_clip, sfxVolume); }
}
