using UnityEngine;

// 确保该脚本所挂载的对象上一定有 AudioSource 组件
[RequireComponent(typeof(AudioSource))]
public class FreeFlyCamera1 : MonoBehaviour // 注意：如果你的文件名是 Move.cs，这里最好也改成 public class Move : MonoBehaviour
{
  

    [Header("音效设置")]
    public AudioClip footstepSound;         // 脚步声音频文件
    private AudioSource audioSource;        // 引用AudioSource组件



    void Start()
    {
        // 锁定并隐藏鼠标光标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 获取并设置AudioSource
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = footstepSound;
        audioSource.loop = true;
    }

    void Update()
    {
        float moveForward = Input.GetAxis("Vertical");   // W/S键
        float moveSideways = Input.GetAxis("Horizontal"); // A/D键

        
        // --- 脚步声音效逻辑 ---
        // 检查是否有来自W,A,S,D的移动输入
        if (moveForward != 0 || moveSideways != 0)
        {
            // 如果正在移动，并且音效还没开始播放
            if (!audioSource.isPlaying)
            {
                // 就开始播放音效
                audioSource.Play();
            }
        }
        else
        {
            // 如果没有移动输入，并且音效正在播放
            if (audioSource.isPlaying)
            {
                // 就停止播放音效
                audioSource.Stop();
            }
        } // <- 这一对 `if-else` 的括号很容易出错

        

        // --- 退出控制 ---
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    } // <- 这是 Update() 的右括号

} // <- 这是整个 Class 的右括号