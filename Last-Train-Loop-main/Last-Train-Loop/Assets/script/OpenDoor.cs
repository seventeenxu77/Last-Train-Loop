using System.Collections;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [Header("旋转设置")]
    public float openAngle = 90f;    // 旋转角度
    public float duration = 1.2f;     // 开门速度
    
    private bool isOpen = false;
    private Quaternion closedRot;
    private Quaternion openRot;
    private Coroutine currentCoroutine;

    void Start()
    {
        // 记录父物体初始旋转
        closedRot = transform.localRotation;
        
        // 【关键改动】：通常门绕 Y 轴转动。如果你确定是 Z 轴，请把 openAngle 换个位置
        openRot = closedRot * Quaternion.Euler(0, 0, openAngle); 
        
        Debug.Log($"门 {gameObject.name} 初始化成功，初始角度: {closedRot.eulerAngles}");
    }

    // 交互触发此函数
    public void openDoor()
    {
        Debug.Log("<color=cyan>收到开门指令！</color>");
        
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        
        isOpen = !isOpen; // 切换状态
        currentCoroutine = StartCoroutine(AnimateDoor());
    }

    private IEnumerator AnimateDoor()
    {
        // 起点设为当前这一刻的角度，这样中途反转会很平滑
        Quaternion startRot = transform.localRotation;
        Quaternion targetRot = isOpen ? openRot : closedRot;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            
            // 使用平滑插值曲线
            t = t * t * (3f - 2f * t);

            transform.localRotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        transform.localRotation = targetRot;
        currentCoroutine = null;
        Debug.Log("旋转动画执行完毕");
    }

    // --- 调试工具 ---
void Update()
{
    if (Input.GetKeyDown(KeyCode.E)) // 或者是 KeyCode.O
    {
        Debug.Log("检测到按键按下！"); // 看看这行出不出
        
        // 如果你加了距离判断
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player != null)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            Debug.Log("当前距离门: " + dist);
            if(dist < 2.0f) 
            {
                openDoor();
            }
            else
            {
                Debug.Log("太远了，开不了");
            }
        }
        else
        {
            Debug.LogError("没找到标签为 Player 的物体！");
        }
    }
}
}