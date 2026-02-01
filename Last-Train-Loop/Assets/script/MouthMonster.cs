using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MouthMonster : MonoBehaviour
{
    [Header("模型结构引用")]
    public Transform upperJaw;    // 拖入上颌 (Pivot)
    public Transform lowerJaw;    // 拖入下颌 (Pivot)
    public Transform tongue;      // 拖入舌头 (Pivot)
    
    [Header("咬合与舌头设置")]
    public float biteSpeed = 5f;       // 咬合与伸缩频率
    public float maxOpenAngle = 30f;   // 张开的角度
    public float tongueMaxStretch = 2.5f; // 舌头最长伸到多少倍
    public float tongueJitter = 15f;    // 舌头乱颤的剧烈程度

    [Header("路径移动设置")]
    public List<Transform> waypoints;  // 路径点列表
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;  // 过弯转向灵敏度

    private bool isActivated = false;
    private int currentPointIndex = 0;

    void Start()
    {
        // 初始隐藏怪物
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (isActivated)
        {
            HandleMouthAndTongue();
        }
    }

    // 核心逻辑：嘴巴咬合 + 舌头恐怖化
    void HandleMouthAndTongue()
    {
        // 1. 计算基础呼吸/咬合节奏 (-1 到 1)
        float wave = Mathf.Sin(Time.time * biteSpeed);
        
        // 2. 嘴巴咬合：使用 localRotation 确保不干扰父物体旋转
        float angle = wave * maxOpenAngle;
        upperJaw.localRotation = Quaternion.Euler(-angle, 0, 0);
        lowerJaw.localRotation = Quaternion.Euler(angle, 0, 0);

        // 3. 舌头逻辑
        if (tongue != null)
        {
            // A. 动态伸缩：只在张嘴时（wave 为正）大幅度伸长
            float stretchFactor = Mathf.Lerp(1f, tongueMaxStretch, Mathf.Max(0, wave));
            tongue.localScale = new Vector3(1f, 1f, stretchFactor);

            // B. 恐怖乱颤：利用柏林噪声实现肉质的非规律扭动
            float noiseX = (Mathf.PerlinNoise(Time.time * tongueJitter, 0) - 0.5f) * 20f;
            float noiseY = (Mathf.PerlinNoise(0, Time.time * tongueJitter) - 0.5f) * 20f;
            
            // 锁定 Z 轴旋转，只让舌头上下左右甩动
            tongue.localRotation = Quaternion.Euler(noiseX, noiseY, 0);
        }
    }

    public void Activate()
    {
        if (!isActivated)
        {
            isActivated = true;
            gameObject.SetActive(true);
            StartCoroutine(FollowPath());
        }
    }

    IEnumerator FollowPath()
    {
        while (currentPointIndex < waypoints.Count)
        {
            Transform target = waypoints[currentPointIndex];

            while (Vector3.Distance(transform.position, target.position) > 0.2f)
            {
                // 1. 位移
                transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

                // 2. 平滑过弯转向
                Vector3 direction = (target.position - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRot = Quaternion.LookRotation(direction);
                    // Slerp 保证过弯时嘴巴是圆滑转过去的
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
                }

                yield return null;
            }

            currentPointIndex++;
            yield return null;
        }
        
        // 完成路径后消失
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("玩家死亡！");
            // 这里可以添加重置场景或死亡特效
            // UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}