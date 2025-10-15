using UnityEngine;

public class SceneAudioPlayer : MonoBehaviour
{
    [Header("Drag Audio Clip Here")]
    [SerializeField] private AudioClip audioClip;
    
    [Header("Settings")]
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool loop = false;
    [SerializeField] [Range(0f, 1f)] private float volume = 1f;
    
    private AudioSource audioSource;
    
    void Start()
    {
        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Setup audio source
        audioSource.clip = audioClip;
        audioSource.loop = loop;
        audioSource.volume = volume;
        audioSource.playOnAwake = false;
        
        // Play audio when scene loads
        if (playOnStart && audioClip != null)
        {
            audioSource.Play();
            Debug.Log("Audio playing: " + audioClip.name);
        }
        else if (audioClip == null)
        {
            Debug.LogWarning("No audio clip assigned!");
        }
    }
    
    // Optional: Control functions
    public void PlayAudio()
    {
        if (audioSource != null && audioClip != null)
        {
            audioSource.Play();
        }
    }
    
    public void StopAudio()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}
