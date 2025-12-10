using UnityEngine;

// 使用 RequireComponent 可以确保该脚本所挂载的对象上一定有 AudioSource 组件
[RequireComponent(typeof(AudioSource))]
public class FreeFlyCamera : MonoBehaviour
{
    // 公开变量，可以在Unity编辑器中调整
    [Header("移动设置")]
    public float moveSpeed = 10.0f;         // 相机移动速度
    public float fastMoveMultiplier = 2.5f; // 按住左Ctrl时的加速倍率
    
    [Header("视角设置")]
    public float mouseSensitivity = 2.0f;   // 鼠标灵敏度

    // ============== 新增的音效相关变量 ==============
    [Header("音效设置")]
    public AudioClip footstepSound;           // 用于存放脚步声音频文件
    public float walkStepInterval = 0.6f;     // 正常行走时，脚步声的播放间隔
    public float sprintStepInterval = 0.35f;  // 加速跑时，脚步声的播放间隔
    
    private AudioSource audioSource;        // 用来引用AudioSource组件
    private float footstepTimer = 0f;       // 脚步声计时器
    // ===============================================

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        // 锁定并隐藏鼠标光标，以便进行视角控制
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // ============== 在开始时获取AudioSource组件 ==============
        audioSource = GetComponent<AudioSource>();
        // ========================================================
    }

    void Update()
    {
        // --- 鼠标视角转动 ---
        // (这部分代码保持不变)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotationY += mouseX; 
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);


        // --- 键盘移动 ---
        bool isSprinting = Input.GetKey(KeyCode.LeftControl);
        float currentSpeed = isSprinting ? moveSpeed * fastMoveMultiplier : moveSpeed;

        float moveForward = Input.GetAxis("Vertical");   // W/S键
        float moveSideways = Input.GetAxis("Horizontal"); // A/D键
        
        // ============== 脚步声音效处理 ==============
        // 检查是否有水平方向的移动 (前后左右)，并且相机在地面上（这里简化为不考虑上下移动）
        if (moveForward != 0 || moveSideways != 0)
        {
            // 如果正在移动，就更新计时器
            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0f)
            {
                // 计时器到点，播放脚步声
                if (footstepSound != null)
                {
                    // PlayOneShot适合播放短促、可重叠的音效
                    audioSource.PlayOneShot(footstepSound);
                }

                // 根据当前是行走还是冲刺，重置计时器
                footstepTimer = isSprinting ? sprintStepInterval : walkStepInterval;
            }
        }
        else
        {
            // 如果没有移动，则重置计时器，以便下次移动时能立即播放第一声
            footstepTimer = 0f;
        }
        // ===============================================


        // 计算基于相机朝向的移动向量
        Vector3 move = transform.forward * moveForward + transform.right * moveSideways;

        // 上升和下降 (Y轴)
        if (Input.GetKey(KeyCode.Space))
        {
            move.y += 1; // 向上移动
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            move.y -= 1; // 向下移动
        }

        // 应用移动
        transform.position += move.normalized * currentSpeed * Time.deltaTime;


        // --- 退出控制 ---
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}