using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    private float speed = 3f;
    private bool isOpen = false;          // 当前状态
    private bool cursorOnDoor = false;    // 鼠标是否悬停
    private Quaternion closedRot;
    private Quaternion openRot;

    void Awake()
    {
        closedRot = transform.localRotation;
        openRot = closedRot * Quaternion.Euler(0, 0, -90); 
    }
    void Update()
    {
        // 平滑旋转
        Quaternion target = isOpen ? openRot : closedRot;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, target, speed * Time.deltaTime);

        // 悬停+左键
        if (cursorOnDoor && Input.GetMouseButtonDown(0))
        {
            isOpen = !isOpen;
            Debug.Log("门的状态改变");
        }
    }
    void OnMouseEnter() { cursorOnDoor = true; Debug.Log("鼠标悬停"); }
    void OnMouseExit() { cursorOnDoor = false; Debug.Log("鼠标移走"); }

}

