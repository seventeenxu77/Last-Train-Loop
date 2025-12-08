using UnityEngine;

public class playerMove : MonoBehaviour
{
    [Header("指定player的起始位置")]
    public Vector3 StartPosition;
    [Header("指定player起始在target")]
    public Transform Target;
    [Tooltip("指定玩家移动速度")]
    public float PlayerSpeed = 3f;
    [Tooltip("指定视角转动灵敏度")]
    public float mouseSensitivity = 20.0f;   // 鼠标灵敏度

    private float rotationX = 0f;
    private float rotationY = 0f;
    void Start()
    {
        transform.position = StartPosition;
        if (Target != null ) transform.position = Target.position + new Vector3(1,1,1);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void FixedUpdate()
    {
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
        float xValue = Input.GetAxis("x");
        float zValue = Input.GetAxis("z");
        Vector3 move = xValue * transform.right + zValue * transform.forward;
        if (Input.GetKey(KeyCode.Space))
        {
            move.y += 1; // 向上移动
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            move.y -= 1; // 向下移动
        }
        transform.position += move * PlayerSpeed * Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
