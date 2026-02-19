using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // [新增] 用于场景跳转

public class MouthMonster : MonoBehaviour
{
    public enum MouthState { Idle, CrazyBite, DashOpen, ShakeHead }
    private MouthState currentState = MouthState.Idle;

    [Header("模型结构引用")]
    public Transform upperJaw;
    public Transform lowerJaw;
    public Transform tongue;

    [Header("速度设置")]
    public float baseMoveSpeed = 2.0f;
    public float slowMoveSpeed = 0.8f;
    public float burstSpeedMultiplier = 4.5f;

    [Header("摇头与嘴巴设置")]
    public float shakeRange = 30f;
    public float shakeSpeed = 10f;
    public float maxOpenAngle = 45f;
    public float tongueMaxLength = 2.5f;
    public float rotateWaitTime = 2.0f;

    [Header("路径点")]
    public List<Transform> waypoints;

    [Header("杀戮设置")]
    public float killDistance = 1.8f; // [新增] 触发死亡的距离
    public string deathSceneName = "mouthdeath"; // [新增] 对应队友给你的视频跳转场景名

    private GameObject player; // [新增] 玩家引用
    private bool isActivated = false;
    private int currentPointIndex = 0;
    private Quaternion baseRotation;

    void Start()
    {
        // [新增] 自动通过标签寻找玩家
        player = GameObject.FindGameObjectWithTag("Player");
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (isActivated)
        {
            HandleVisuals();
            CheckKillPlayer(); // [新增] 每帧检查是否抓到玩家
        }
    }

    // [新增] 死亡检查逻辑
    void CheckKillPlayer()
    {
        if (player == null) return;

        // 计算水平面上的距离（防止玩家跳起导致距离变远射不死）
        Vector3 monsterPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 playerPos = new Vector3(player.transform.position.x, 0, player.transform.position.z);
        float dist = Vector3.Distance(monsterPos, playerPos);

        if (dist < killDistance)
        {
            ExecuteGameOver();
        }
    }

    // [新增] 执行游戏结束
    void ExecuteGameOver()
    {
        isActivated = false;
        StopAllCoroutines(); // 停止移动

        // 解锁鼠标，防止黑屏或视频时无法操作
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // 加载死亡视频场景
        Debug.Log("<color=red>玩家被吃掉了！跳转场景...</color>");
        if (LoopManager.Instance != null)
        {
            Destroy(LoopManager.Instance.gameObject);
        }
        SceneManager.LoadScene(deathSceneName);
    }

    void HandleVisuals()
    {
        float mouthAngle = 0;
        float targetTongueStretch = 0f;

        switch (currentState)
        {
            case MouthState.CrazyBite:
                mouthAngle = Mathf.Abs(Mathf.Sin(Time.time * 25f)) * maxOpenAngle;
                targetTongueStretch = 0f;
                break;

            case MouthState.DashOpen:
                mouthAngle = maxOpenAngle + Mathf.Sin(Time.time * 40f) * 2f;
                targetTongueStretch = 0f;
                break;

            case MouthState.ShakeHead:
                mouthAngle = maxOpenAngle;
                targetTongueStretch = tongueMaxLength;

                float yaw = Mathf.Sin(Time.time * shakeSpeed) * shakeRange;
                float pitch = (Mathf.PerlinNoise(Time.time * 15f, 0) - 0.5f) * 10f;
                float roll = (Mathf.PerlinNoise(0, Time.time * 15f) - 0.5f) * 10f;

                transform.localRotation = baseRotation * Quaternion.Euler(pitch, yaw, roll);
                break;
        }

        upperJaw.localRotation = Quaternion.Euler(-mouthAngle, 0, 0);
        lowerJaw.localRotation = Quaternion.Euler(mouthAngle, 0, 0);

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

            currentState = MouthState.CrazyBite;
            yield return new WaitForSeconds(0.7f);

            currentState = MouthState.DashOpen;
            float dashSpeed = baseMoveSpeed * burstSpeedMultiplier;
            while (Vector3.Distance(transform.position, target.position) > 1.5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, dashSpeed * Time.deltaTime);
                RotateTowards(target.position);
                yield return null;
            }

            currentState = MouthState.ShakeHead;
            baseRotation = transform.rotation;
            float timer = 0;
            while (timer < rotateWaitTime)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, slowMoveSpeed * Time.deltaTime);

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