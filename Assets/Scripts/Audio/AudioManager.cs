using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource ambientSource;

    [Header("Music")]
    public AudioClip mainMenuMusic;
    public AudioClip gameplayMusic;
    public AudioClip gameOverMusic;

    [Header("SFX Clips")]
    public AudioClip ballLaunch;
    public AudioClip bumperHit;
    public AudioClip flipperHit;
    public AudioClip targetHit;
    public AudioClip wallHit;
    public AudioClip spinnerHit;
    public AudioClip rampHit;
    public AudioClip ballLost;
    public AudioClip comboSound;
    public AudioClip multiBallActivate;
    public AudioClip gameOverSound;
    public AudioClip buttonClick;

    [Header("Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.8f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float ambientVolume = 0.3f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFXSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }
    }

    private void Start()
    {
        LoadVolumes();
    }

    public void PlayMusic(AudioClip clip, bool fade = true)
    {
        if (musicSource == null || clip == null) return;

        if (fade && musicSource.isPlaying)
        {
            StartCoroutine(CrossFadeMusic(clip));
        }
        else
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public void PlaySFX(AudioClip clip, float volumeScale = 1f, float pitch = 1f)
    {
        if (sfxSource == null || clip == null) return;

        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip, sfxVolume * volumeScale);
    }

    public void PlaySFXAtPosition(AudioClip clip, Vector3 position, float volumeScale = 1f)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, position, sfxVolume * volumeScale);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if (musicSource != null)
            musicSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        if (sfxSource != null)
            sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    private void LoadVolumes()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (musicSource != null) musicSource.volume = musicVolume;
        if (sfxSource != null) sfxSource.volume = sfxVolume;
    }

    private System.Collections.IEnumerator CrossFadeMusic(AudioClip newClip, float duration = 1f)
    {
        float startVolume = musicSource.volume;

        for (float t = 0; t < duration / 2; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / (duration / 2));
            yield return null;
        }

        musicSource.clip = newClip;
        musicSource.Play();

        for (float t = 0; t < duration / 2; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, musicVolume, t / (duration / 2));
            yield return null;
        }

        musicSource.volume = musicVolume;
    }
}
