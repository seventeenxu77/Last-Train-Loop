using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    [Header("相机旋转设置")]
    public Transform cameraTransform; // 拖入子物体 Main Camera
    public float mouseSensitivity = 200f;
    private float xRotation = 0f;

    [Header("必要组件")]
    public CharacterController controller; // 拖入自身的 CharacterController

    private Vector3 velocity; // 用于处理重力和跳跃的速度
    private bool isGrounded;  // 是否在地面上

    void Start()
    {
        // 游戏开始时隐藏并锁定鼠标，防止鼠标滑出窗口
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 1. 检测是否在地面上
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            // 在地面时重置下落速度（设置一个小的负值确保角色贴地）
            velocity.y = -2f;
        }

        // 2. 处理视角旋转 (鼠标)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 左右旋转：旋转整个玩家身体
        transform.Rotate(Vector3.up * mouseX);

        // 上下旋转：只旋转相机，并限制视角在 -90 到 90 度之间
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 3. 处理水平移动 (WASD)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // 根据玩家当前面向的方向来计算移动向量
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // 4. 处理跳跃
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // 跳跃物理公式：v = sqrt(h * -2 * g)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 5. 应用重力
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}