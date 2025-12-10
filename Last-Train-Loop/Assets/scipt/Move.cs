using UnityEngine;

// 确保该脚本所挂载的对象上一定有 AudioSource 组件
[RequireComponent(typeof(AudioSource))]
public class FreeFlyCamera : MonoBehaviour // 注意：如果你的文件名是 Move.cs，这里最好也改成 public class Move : MonoBehaviour
{
    // 公开变量，可以在Unity编辑器中调整
    [Header("移动设置")]
    public float moveSpeed = 10.0f;         // 相机移动速度
    public float fastMoveMultiplier = 2.5f; // 按住左Ctrl时的加速倍率

    [Header("视角设置")]
    public float mouseSensitivity = 2.0f;   // 鼠标灵敏度

    

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        // 锁定并隐藏鼠标光标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    void Update()
    {
        // --- 鼠标视角转动 ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);

        // --- 键盘移动 ---
        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftControl))
        {
            currentSpeed *= fastMoveMultiplier;
        }

        float moveForward = Input.GetAxis("Vertical");   // W/S键
        float moveSideways = Input.GetAxis("Horizontal"); // A/D键

        

        // --- 计算并应用移动 ---
        Vector3 move = transform.forward * moveForward + transform.right * moveSideways;
        if (Input.GetKey(KeyCode.Space)) { move.y += 1; }
        if (Input.GetKey(KeyCode.LeftShift)) { move.y -= 1; }
        transform.position += move * currentSpeed * Time.deltaTime;

        // --- 退出控制 ---
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    } // <- 这是 Update() 的右括号

} // <- 这是整个 Class 的右括号