using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    // 배경음과 효과음용 AudioSource
    public AudioSource backgroundAudioSource;
    public AudioSource effectAudioSource;

    [Header("BackgroundAudioClip")]
    public AudioClip bgmClip;
    public AudioClip TitleClip;

    [Header("AudioClip")]
    public AudioClip spawnUnitClip;
    public AudioClip levelUpClip;
    public AudioClip playerAtkClip;
    public AudioClip enemyAtkClip;
    public AudioClip unitDeadClip;
    public AudioClip victoryClip;
    public AudioClip defeatClip;
    public AudioClip FireLaserClip;

    private void Awake()
    {
        Instance = this;
        InitializeAudioSources();
    }

    private void Start()
    {
        // 시작 시 타이틀 음악 재생
        if (TitleClip != null)
        {
            PlayBackgroundMusic(TitleClip);
        }
    }

    private void InitializeAudioSources()
    {
        // 배경음 설정
        backgroundAudioSource.playOnAwake = false;
        backgroundAudioSource.loop = true;
        backgroundAudioSource.ignoreListenerPause = true;

        // 효과음 설정
        effectAudioSource.playOnAwake = false;
        effectAudioSource.loop = false;
        effectAudioSource.ignoreListenerPause = true;
    }

    public void PlayBackgroundMusic(AudioClip clip)
    {
        if (clip == null) return;

        // 현재 재생 중인 클립과 같고 이미 재생 중이면 리턴
        if (backgroundAudioSource.clip == clip && backgroundAudioSource.isPlaying)
            return;

        backgroundAudioSource.Stop();
        backgroundAudioSource.clip = clip;
        backgroundAudioSource.pitch = 1f; // 피치 초기화
        backgroundAudioSource.Play();
    }

    public void PlayEffectSound(AudioClip clip, float volume = 0.25f)
    {
        if (clip == null) return;

        effectAudioSource.pitch = 1f; // 피치 초기화
        effectAudioSource.PlayOneShot(clip, volume);
    }

    public void PlayEffectSound(AudioSource source, AudioClip clip, float volume = 0.25f)
    {
        if (clip == null || source == null) return;

        source.pitch = 1f; // 피치 초기화
        source.PlayOneShot(clip, volume);
    }

    // 승리/패배 사운드 재생 전용 메서드
    public void PlayGameEndSound(bool isVictory)
    {
        StopAllSounds();

        effectAudioSource.pitch = 1f;
        AudioClip clipToPlay = isVictory ? victoryClip : defeatClip;
        effectAudioSource.clip = clipToPlay;
        effectAudioSource.PlayOneShot(clipToPlay, 1f);
    }

    private void StopAllSounds()
    {
        // 모든 재생 중인 사운드 중지
        backgroundAudioSource.Stop();
        effectAudioSource.Stop();
    }
}
