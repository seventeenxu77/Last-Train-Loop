using UnityEngine;
using UnityEngine.EventSystems;

public class HoverSwitch2D : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("需要切换显示的物体")]
    public GameObject normalObject; // 通常状态下显示的物体
    public GameObject hoverObject;
    public GameObject hoverObject2; // 悬停状态下显示的物体

    void Start()
    {
        // 初始状态：显示普通物体，隐藏悬停物体
        SetObjectState(false);

        // 确保此物体有Collider2D，这是射线检测的基础[1](@ref)
        if (GetComponent<Collider2D>() == null)
        {
            Debug.LogError($"物体 {gameObject.name} 上未找到Collider2D组件，无法进行悬停检测。");
        }
    }

    // 当鼠标进入时自动调用[9](@ref)
    public void OnPointerEnter(PointerEventData eventData)
    {
        SetObjectState(true); // 切换到悬停状态
        Debug.Log($"鼠标悬停在: {gameObject.name}");
    }

    // 当鼠标离开时自动调用[9](@ref)
    public void OnPointerExit(PointerEventData eventData)
    {
        SetObjectState(false); // 切换回普通状态
        Debug.Log($"鼠标离开: {gameObject.name}");
    }

    // 控制物体显示状态的方法
    private void SetObjectState(bool isHovering)
    {
        if (normalObject != null) normalObject.SetActive(!isHovering);
        if (hoverObject != null) hoverObject.SetActive(isHovering);
        if (hoverObject2 != null) hoverObject2.SetActive(isHovering);

        // 你也可以在这里添加其他效果，比如改变颜色、播放声音等
    }
}