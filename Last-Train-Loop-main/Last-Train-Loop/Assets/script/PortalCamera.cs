using UnityEngine;

public class PortalCamera : MonoBehaviour
{
    [Header("核心引用")]
    public Transform playerCamera;   
    public Transform portalEntrance; // 入口门（预制体中的门模型）
    public Transform portalExit;     // 出口参考点（目的地）

    private Camera portalCam;

    void Start()
    {
        portalCam = GetComponent<Camera>();

        // 1. 自动关联玩家相机
        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;

        // 2. 自动关联入口门（如果脚本挂在相机上且相机是门的子物体）
        if (portalEntrance == null)
            portalEntrance = transform.parent;

        // 【核心修复】：将相机从父物体中脱离。
        // 这样在编辑器里你可以带着父物体随便斜着放，
        // 但运行瞬间相机独立，不受父物体坐标系的“斜向旋转”干扰。
        transform.SetParent(null);
    }

    void LateUpdate()
    {
        if (playerCamera == null || portalEntrance == null ||!portalExit) return;
        if (Camera.main == null) return;

        // --- 1. 位置同步 ---
        // 计算玩家相对于入口门的本地位置
        Vector3 relativePos = portalEntrance.InverseTransformPoint(playerCamera.position);
        
        // X取反修复镜像效果
        Vector3 mappedPos = new Vector3(-relativePos.x, relativePos.y, relativePos.z);
        
        // 转换回世界坐标（相对于出口点）
        transform.position = portalExit.TransformPoint(mappedPos);

        // --- 2. 旋转同步 ---
        // 计算玩家相机相对于入口门的旋转偏差
        Quaternion relativeRot = Quaternion.Inverse(portalEntrance.rotation) * playerCamera.rotation;
        
        // 计算目标旋转（包含180度倒置修正）
        Quaternion targetRotation = portalExit.rotation * relativeRot * Quaternion.Euler(0, 0, 180);

        // --- 【水平锁定修正】 ---
        // 如果你斜放了门，targetRotation 可能会包含 Z 轴的翻滚。
        // 我们通过提取欧拉角，强制把 Z 轴锁死在 180（抵消倒置），确保地平线是平的。
        Vector3 euler = targetRotation.eulerAngles;
        transform.rotation = Quaternion.Euler(euler.x, euler.y, 180f);

        // --- 3. 相机参数同步 ---
        if (portalCam != null)
        {
            // 确保视野和剪裁面正确
            portalCam.fieldOfView = Camera.main.fieldOfView;
            portalCam.nearClipPlane = 0.01f; 
        }
    }
}