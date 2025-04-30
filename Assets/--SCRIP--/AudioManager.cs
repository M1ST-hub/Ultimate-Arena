using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{

    private static AudioManager instance;

    public static AudioManager Instance { get { return instance; } }

    [Header("References")]
    public AudioMixer masterMixer;
    public AudioSource audioSource;

    [Header("Track List")]
    public AudioClip[] trackList;
    

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        masterMixer.SetFloat("MasterVol", PlayerPrefs.GetFloat("MasterVolume", 1));
        masterMixer.SetFloat("MusicVol", PlayerPrefs.GetFloat("MusicVolume", 1));
        audioSource.clip = trackList[0];
        audioSource.Play();
    }

    private void Update()
    {
        if (audioSource != null)
        {
            if (audioSource.clip.length == 0)
            {
                audioSource.Stop();
                audioSource.clip = trackList[UnityEngine.Random.Range(0, trackList.Length)];
                audioSource.Play();
            }

        }
    }

    public void ChangeSoundVolume(float soundLevel)
    {
        masterMixer.SetFloat("MasterVol", soundLevel);
        PlayerPrefs.SetFloat("MasterVolume", soundLevel);
    }

    public void ChangeMusicVolume(float soundLevel)
    {
        masterMixer.SetFloat("MusicVol", soundLevel);
        PlayerPrefs.SetFloat("MusicVolume", soundLevel);
    }
}
