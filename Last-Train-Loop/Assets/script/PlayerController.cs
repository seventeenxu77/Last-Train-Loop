using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    [Header("相机旋转设置")]
    public Transform cameraTransform;
    public float mouseSensitivity = 200f;
    private float xRotation = 0f;

    [Header("视角晃动设置")]
    public bool enableHeadBob = true;
    public float bobAmplitude = 0.05f;
    public float bobFrequency = 10.0f;
    private float defaultCameraY;
    private float bobTimer = 0f;

    // [新增] --- 音频设置 ---
    [Header("音频设置")]
    public AudioSource footstepSource; // 拖入挂载了 AudioSource 的物体
    // ----------------------

    [Header("必要组件")]
    public CharacterController controller;

    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (cameraTransform != null)
        {
            defaultCameraY = cameraTransform.localPosition.y;
        }
    }

    void Update()
    {
        // 1. 地面检测
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 2. 视角旋转
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 3. 移动输入
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // 视角晃动
        HandleHeadBob(x, z);

        // [新增] --- 处理脚步声 ---
        HandleFootstepAudio(x, z);
        // -----------------------

        // 4. 跳跃
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 5. 重力
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleHeadBob(float inputX, float inputZ)
    {
        if (!enableHeadBob || cameraTransform == null) return;

        if (isGrounded && (Mathf.Abs(inputX) > 0.1f || Mathf.Abs(inputZ) > 0.1f))
        {
            bobTimer += Time.deltaTime * bobFrequency;
            float newY = defaultCameraY + Mathf.Sin(bobTimer) * bobAmplitude;
            cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, newY, cameraTransform.localPosition.z);
        }
        else
        {
            bobTimer = 0;
            float newY = Mathf.Lerp(cameraTransform.localPosition.y, defaultCameraY, Time.deltaTime * 10f);
            cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, newY, cameraTransform.localPosition.z);
        }
    }

    // [新增] 脚步声控制逻辑
    void HandleFootstepAudio(float inputX, float inputZ)
    {
        // 如果没有赋值 AudioSource，直接返回，防止报错
        if (footstepSource == null) return;

        // 判断玩家是否在移动 (输入不为0) 且 在地面上
        bool isMoving = Mathf.Abs(inputX) > 0.1f || Mathf.Abs(inputZ) > 0.1f;

        // 逻辑：如果在地面上 且 在移动
        if (isGrounded && isMoving)
        {
            // 如果当前没有在播放声音，才开始播放（防止每帧重复调用 Play 导致声音鬼畜）
            if (!footstepSource.isPlaying)
            {
                footstepSource.Play();
            }
        }
        else
        {
            // 如果停下来了 或者 跳在空中，且声音正在播放，则停止
            if (footstepSource.isPlaying)
            {
                footstepSource.Stop(); // 或者用 footstepSource.Pause();
            }
        }
    }
}