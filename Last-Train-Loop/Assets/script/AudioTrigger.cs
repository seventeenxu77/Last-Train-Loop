using UnityEngine;
using UnityEngine.UIElements;

public class AudioTrigger : MonoBehaviour
{
    [Header("音频设置")]
    public AudioClip soundClip;     
    public float volume = 1.0f;       // 音量（0-1）

    private AudioSource audioSource;

    void Start()
    {
        // 自动获取或添加AudioSource组件
        audioSource = GetComponent<AudioSource>();
     
    }

    public void PlaySound()
    {

        if (soundClip != null)
        {
            audioSource.Stop();
            audioSource.Play();
        }
    }
}
