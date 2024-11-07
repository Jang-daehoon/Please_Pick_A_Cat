using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    // ������� ȿ������ AudioSource
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
        // ���� �� Ÿ��Ʋ ���� ���
        if (TitleClip != null)
        {
            PlayBackgroundMusic(TitleClip);
        }
    }

    private void InitializeAudioSources()
    {
        // ����� ����
        backgroundAudioSource.playOnAwake = false;
        backgroundAudioSource.loop = true;
        backgroundAudioSource.ignoreListenerPause = true;

        // ȿ���� ����
        effectAudioSource.playOnAwake = false;
        effectAudioSource.loop = false;
        effectAudioSource.ignoreListenerPause = true;
    }

    public void PlayBackgroundMusic(AudioClip clip)
    {
        if (clip == null) return;

        // ���� ��� ���� Ŭ���� ���� �̹� ��� ���̸� ����
        if (backgroundAudioSource.clip == clip && backgroundAudioSource.isPlaying)
            return;

        backgroundAudioSource.Stop();
        backgroundAudioSource.clip = clip;
        backgroundAudioSource.pitch = 1f; // ��ġ �ʱ�ȭ
        backgroundAudioSource.Play();
    }

    public void PlayEffectSound(AudioClip clip, float volume = 0.25f)
    {
        if (clip == null) return;

        effectAudioSource.pitch = 1f; // ��ġ �ʱ�ȭ
        effectAudioSource.PlayOneShot(clip, volume);
    }

    public void PlayEffectSound(AudioSource source, AudioClip clip, float volume = 0.25f)
    {
        if (clip == null || source == null) return;

        source.pitch = 1f; // ��ġ �ʱ�ȭ
        source.PlayOneShot(clip, volume);
    }

    // �¸�/�й� ���� ��� ���� �޼���
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
        // ��� ��� ���� ���� ����
        backgroundAudioSource.Stop();
        effectAudioSource.Stop();
    }
}
