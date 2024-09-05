using UnityEngine;

public class EffectController : MonoBehaviour
{
    private float lifeTime;
    public float LifeTime
    {
        get => lifeTime;
        set
        {
            if (value < 0f)
                return;

            lifeTime = value;
            timer = 0f;
            isAutoDestroy = true;
        }
    }
    private bool isAutoDestroy = false;
    private float timer = 0f;

    public IEffectGettable target;

    [Header("사운드")]
    public AudioClip enableAudioClip;
    public AudioClip disableAudioClip;
    public bool isAutoPlay = true;
    private AudioSource audioSource;

    private void Awake()
    {
        var soundManager = GameObject.FindWithTag(Defines.Tags.SOUND_MANAGER_TAG)?.GetComponent<SoundManager>();
        audioSource = soundManager?.sfxAudioSource;
    }

    private void OnEnable()
    {
        if (isAutoPlay)
            PlayAudioClip(enableAudioClip);
    }

    private void OnDisable()
    {
        if (isAutoPlay)
            PlayAudioClip(disableAudioClip);
    }

    private void Update()
    {
        AutoDestroy();
        
        var pos = transform.position;
        pos.z = pos.y;
        transform.position = pos;
    }

    private void AutoDestroy()
    {
        if (!isAutoDestroy)
            return;

        timer += Time.deltaTime;
        if (timer > lifeTime)
        {
            ReleaseEffect();
        }
    }

    public void ReleaseEffect()
    {
        if (target != null)
            target.RemoveEffect(this);

        Destroy(gameObject);
    }

    public void PlayAudioClip(AudioClip clip)
    {
        if (audioSource && clip)
            audioSource.PlayOneShot(clip);
    }
}