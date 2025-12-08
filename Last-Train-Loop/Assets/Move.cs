using UnityEngine;

public class FreeFlyCamera : MonoBehaviour
{
    // 公开变量，可以在Unity编辑器中调整
    public float moveSpeed = 10.0f;         // 相机移动速度
    public float fastMoveMultiplier = 2.5f; // 按住左Ctrl时的加速倍率
    public float mouseSensitivity = 2.0f;   // 鼠标灵敏度

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        // 锁定并隐藏鼠标光标，以便进行视角控制
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // --- 鼠标视角转动 ---

        // 获取鼠标移动增量
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // 计算旋转角度
        // 绕Y轴旋转 (左右看)
        rotationY += mouseX; 
        // 绕X轴旋转 (上下看)
        rotationX -= mouseY;

        // 限制上下看的角度，防止相机翻转
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        // 应用旋转
        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);


        // --- 键盘移动 ---

        // 判断是否按下了加速键
        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftControl))
        {
            currentSpeed *= fastMoveMultiplier;
        }

        // 获取键盘输入
        float moveForward = Input.GetAxis("Vertical");   // W/S键
        float moveSideways = Input.GetAxis("Horizontal"); // A/D键
        
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
        // 使用 Time.deltaTime 确保移动速度与帧率无关
        transform.position += move * currentSpeed * Time.deltaTime;


        // --- 退出控制 ---

        // 按下Esc键可以解锁鼠标
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}