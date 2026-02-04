using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    [Header("相机旋转设置")]
    public Transform cameraTransform, handCameraTransform;
    public float mouseSensitivity = 200f;
    private float xRotation = 0f;

    [Header("视角晃动设置")]
    public bool enableHeadBob = true;
    public float bobAmplitude = 0.05f;
    public float bobFrequency = 10.0f;
    private float defaultCameraY;
    private float bobTimer = 0f;

    [Header("音频设置")]
    public AudioSource footstepSource; 

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
        // 【核心修复】：如果控制器未激活，直接跳过本帧，不执行任何移动逻辑
        if (controller == null || !controller.enabled) 
        {
            // 如果控制器关了，声音也得关掉，防止原地踏步声
            if (footstepSource != null && footstepSource.isPlaying) footstepSource.Stop();
            return; 
        }

        // 1. 地面检测
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 2. 视角旋转 (视角旋转通常不需要依赖 CC 激活，但放在这里更安全)
        RotateCamera();

        // 3. 移动输入
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        
        // 执行水平移动
        controller.Move(move * moveSpeed * Time.deltaTime);

        // 效果类逻辑
        HandleHeadBob(x, z);
        HandleFootstepAudio(x, z);

        // 4. 跳跃
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 5. 重力与垂直移动
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        if(handCameraTransform != null)
            handCameraTransform.localRotation = cameraTransform.localRotation;
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

    void HandleFootstepAudio(float inputX, float inputZ)
    {
        if (footstepSource == null) return;
        bool isMoving = Mathf.Abs(inputX) > 0.1f || Mathf.Abs(inputZ) > 0.1f;

        if (isGrounded && isMoving)
        {
            if (!footstepSource.isPlaying) footstepSource.Play();
        }
        else
        {
            if (footstepSource.isPlaying) footstepSource.Stop();
        }
    }
}