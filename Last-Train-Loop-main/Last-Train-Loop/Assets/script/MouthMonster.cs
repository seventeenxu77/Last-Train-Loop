using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MouthMonster : MonoBehaviour
{
    public enum MouthState { Idle, CrazyBite, DashOpen, ShakeHead }
    private MouthState currentState = MouthState.Idle;

    [Header("模型结构引用")]
    public Transform upperJaw; 
    public Transform lowerJaw; 
    public Transform tongue; 

    [Header("速度设置")]
    public float baseMoveSpeed = 2.0f;      // 基础速度
    public float slowMoveSpeed = 0.8f;      // 摇头时的缓慢移动速度
    public float burstSpeedMultiplier = 4.5f; // 突进倍率

    [Header("摇头与嘴巴设置")]
    public float shakeRange = 30f; 
    public float shakeSpeed = 10f; 
    public float maxOpenAngle = 45f; 
    public float tongueMaxLength = 2.5f; 
    public float rotateWaitTime = 2.0f;    // 缓慢摇头移动的持续时间

    [Header("路径点")]
    public List<Transform> waypoints;

    private bool isActivated = false;
    private int currentPointIndex = 0;
    private Quaternion baseRotation; 

    void Start() => gameObject.SetActive(false);

    void Update()
    {
        if (isActivated) HandleVisuals();
    }

    void HandleVisuals()
    {
        float mouthAngle = 0;
        float targetTongueStretch = 0f; 
        
        switch (currentState)
        {
            case MouthState.CrazyBite:
                // 1. 蓄力快咬
                mouthAngle = Mathf.Abs(Mathf.Sin(Time.time * 25f)) * maxOpenAngle;
                targetTongueStretch = 0f; 
                break;

            case MouthState.DashOpen:
                // 2. 突进大张嘴
                mouthAngle = maxOpenAngle + Mathf.Sin(Time.time * 40f) * 2f;
                targetTongueStretch = 0f; 
                break;

            case MouthState.ShakeHead:
                // 3. 缓慢移动 + 摇头吐舌头
                mouthAngle = maxOpenAngle; 
                targetTongueStretch = tongueMaxLength; 

                // 左右摇头逻辑
                float yaw = Mathf.Sin(Time.time * shakeSpeed) * shakeRange;
                float pitch = (Mathf.PerlinNoise(Time.time * 15f, 0) - 0.5f) * 10f;
                float roll = (Mathf.PerlinNoise(0, Time.time * 15f) - 0.5f) * 10f;
                
                // 基于基准朝向叠加摇头角度
                transform.localRotation = baseRotation * Quaternion.Euler(pitch, yaw, roll);
                break;
        }

        // 应用嘴巴旋转
        upperJaw.localRotation = Quaternion.Euler(-mouthAngle, 0, 0);
        lowerJaw.localRotation = Quaternion.Euler(mouthAngle, 0, 0);

        // 应用舌头缩放与乱动
        if (tongue != null)
        {
            float currentScaleZ = Mathf.Lerp(tongue.localScale.z, targetTongueStretch, Time.deltaTime * 8f);
            float vibration = (currentState == MouthState.ShakeHead) ? Mathf.Sin(Time.time * 25f) * 0.3f : 0;
            tongue.localScale = new Vector3(1f, 1f, currentScaleZ + vibration);

            if (currentState == MouthState.ShakeHead)
            {
                float tNoise = (Mathf.PerlinNoise(Time.time * 20f, 5) - 0.5f) * 45f;
                tongue.localRotation = Quaternion.Euler(tNoise, tNoise, tNoise);
            }
            else
            {
                tongue.localRotation = Quaternion.identity;
            }
        }
    }

    public void Activate()
    {
        if (!isActivated)
        {
            isActivated = true;
            gameObject.SetActive(true);
            StartCoroutine(MonsterBehavior());
        }
    }

    IEnumerator MonsterBehavior()
    {
        while (currentPointIndex < waypoints.Count)
        {
            Transform target = waypoints[currentPointIndex];

            // 第一阶段：蓄力快咬
            currentState = MouthState.CrazyBite;
            yield return new WaitForSeconds(0.7f);

            // 第二阶段：高速突进
            currentState = MouthState.DashOpen;
            float dashSpeed = baseMoveSpeed * burstSpeedMultiplier;
            while (Vector3.Distance(transform.position, target.position) > 1.5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, dashSpeed * Time.deltaTime);
                RotateTowards(target.position);
                yield return null;
            }

            // 第三阶段：缓慢摇头移动
            currentState = MouthState.ShakeHead;
            baseRotation = transform.rotation; 
            float timer = 0;
            while (timer < rotateWaitTime)
            {
                // 向目标点缓慢推进
                transform.position = Vector3.MoveTowards(transform.position, target.position, slowMoveSpeed * Time.deltaTime);
                
                // 实时更新基准朝向，确保摇头时整体方向不跑偏
                Vector3 dir = (target.position - transform.position).normalized;
                if (dir != Vector3.zero)
                {
                    baseRotation = Quaternion.Slerp(baseRotation, Quaternion.LookRotation(dir), Time.deltaTime * 2f);
                }

                timer += Time.deltaTime;
                yield return null;
            }

            currentPointIndex++;
        }
        
        gameObject.SetActive(false);
    }

    void RotateTowards(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - transform.position).normalized;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 12f * Time.deltaTime);
    }
}