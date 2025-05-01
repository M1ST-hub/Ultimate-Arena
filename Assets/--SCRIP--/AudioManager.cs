using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    private static AudioManager instance;

    public static AudioManager Instance { get { return instance; } }

    [Header("References")]
    public AudioMixer masterMixer;
    public AudioSource audioSource;
    public AudioSource musicSource;

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
        musicSource.clip = trackList[0];
        musicSource.Play();
    }

    private void Update()
    {
        if (musicSource != null)
        {
            if (musicSource.clip.length == 0)
            {
                musicSource.Stop();
                musicSource.clip = trackList[UnityEngine.Random.Range(0, trackList.Length)];
                musicSource.Play();
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
