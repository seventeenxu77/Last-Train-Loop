using UnityEngine;

public class DelayAudio : MonoBehaviour
{
    public AudioSource myAudio; // 拖入你的 AudioSource
    public float delayTime = 3.0f; // 延迟几秒

    void Start()
    {
        // 如果没有手动赋值，自动获取当前物体上的组件
        if (myAudio == null) myAudio = GetComponent<AudioSource>();

        // 告诉 AudioSource：等 delayTime 秒后再播放
        myAudio.PlayDelayed(delayTime);
    }
}