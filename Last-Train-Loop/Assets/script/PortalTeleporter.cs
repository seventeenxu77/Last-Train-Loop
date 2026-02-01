using UnityEngine;
using UnityEngine.AI;

public class PortalTeleporter : MonoBehaviour
{
    public Transform player;      // 玩家物体
    public Transform receiver;    // 目的地门（出口）

    private bool playerIsOverlapping = false;
    private float teleportCooldown = 0.5f;
    private static float lastTeleportTime;

    void Update()
    {
        if (playerIsOverlapping)
        {
            if (Time.time - lastTeleportTime < teleportCooldown) return;

            Vector3 portalToPlayer = player.position - transform.position;
            // 穿过门平面的检测逻辑
            float dotProduct = Vector3.Dot(transform.forward, portalToPlayer);

            if (dotProduct < 0f)
            {
                TeleportPlayer();
            }
        }
    }

    void TeleportPlayer()
    {
        if (player == null || receiver == null) return;

        // --- 1. 获取组件（参考 LoopManager 的防御性写法） ---
        CharacterController cc = player.GetComponent<CharacterController>();
        NavMeshAgent agent = player.GetComponent<NavMeshAgent>();

        // --- 2. 暂时禁用组件，防止瞬移报错 ---
        if (cc != null) cc.enabled = false;
        if (agent != null) agent.enabled = false;

        // --- 3. 计算位置与旋转 ---
        // 获取玩家相对于入口门的相对坐标
        Vector3 localPos = transform.InverseTransformPoint(player.position);

        // 【核心修改点 1】：位置偏移
        // 因为你手动旋转了出口门，所以不再需要 180 度反转。
        // 使用 Quaternion.identity (无偏移) 来保持坐标系同步。
        Vector3 targetPos = receiver.TransformPoint(localPos);

        // 【核心修改点 2】：旋转计算
        // 直接计算两个门物体之间的旋转差异。
        // Quaternion.Inverse(transform.rotation) * player.rotation 计算玩家相对于入口的偏差旋转
        // 然后叠加到 receiver 的旋转上，实现视角完美衔接。
        Quaternion relativeRotation = Quaternion.Inverse(transform.rotation) * player.rotation;
        player.rotation = receiver.rotation * relativeRotation;

        // --- 4. 执行传送 ---
        player.position = targetPos;
        Physics.SyncTransforms();

        // --- 5. 恢复组件（参考 LoopManager） ---
        if (cc != null) cc.enabled = true;

        // 特殊处理 NavMeshAgent：必须用 Warp 重新同步导航网格
        if (agent != null)
        {
            agent.enabled = true;
            agent.Warp(targetPos);
        }

        // --- 6. 状态重置 ---
        lastTeleportTime = Time.time;
        playerIsOverlapping = false;
        Debug.Log($"传送完成：从 {gameObject.name} 到 {receiver.name}，已匹配手动旋转朝向。");
    }

    void OnTriggerEnter(Collider other) { Debug.Log("进入触发区"); if (other.CompareTag("Player")) playerIsOverlapping = true; }
    void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) playerIsOverlapping = false; }
}