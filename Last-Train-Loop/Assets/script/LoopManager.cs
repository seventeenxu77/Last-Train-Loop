using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoopManager : MonoBehaviour
{
    // 单例模式
    public static LoopManager Instance;

    private bool isTransitioning = false;

    // 【新增】用于保存画框的初始状态
    private Vector3 initialPosterPosition;
    private Quaternion initialPosterRotation;
    private Rigidbody posterRigidbody;

    [Header("UI 引用")]
    public TextMeshProUGUI stationText;
    // 【新增】场景中已存在的人物引用
    [Header("场景物件引用")]
    public GameObject strangeManInScene; // 直接拖拽场景中的人物到这里
    public GameObject poster;
    [Header("自定义模型")]
    public GameObject model_1; // 第一个模型 (通常显示的)
    public GameObject model_2; // 第二个模型 (只在 case 3 显示的)
    [Header("循环数据")]
    public int currentLoopIndex = 0;

    // 玩家出生点 (用于每次循环开始时传送玩家)
    public Transform playerSpawnPoint;

    // 用于保存每次循环动态生成的物件的父对象
    private GameObject dynamicContentParent;

    public bool has_exception =false;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        dynamicContentParent = new GameObject("Dynamic Content");

        if (stationText != null)
        {
            stationText.gameObject.SetActive(false);
        }
        // 【新增】获取 Rigidbody 并保存初始状态
        if (poster != null)
        {
            posterRigidbody = poster.GetComponent<Rigidbody>();
            initialPosterPosition = poster.transform.position;
            initialPosterRotation = poster.transform.rotation;

            // 确保在开始时重力是关闭的
            if (posterRigidbody != null)
            {
                posterRigidbody.useGravity = false;
                // 确保 Rigidbody 处于非运动学状态，便于后续控制
                posterRigidbody.isKinematic = false;
            }
        }

        GenerateLoopContent();
        has_exception = currentLoopIndex >= 1 ? true : false;

        // 【新增 1】确保玩家在第一次进入游戏时位于出生点
        TeleportPlayerToSpawn();

        // 【新增 2】第一次进入游戏时也显示站台信息
        StartCoroutine(DisplayStationTextCoroutine(3.0f));
    }

    // -----------------------------------------------------
    // 【新增】核心功能：传送玩家到出生点
    // -----------------------------------------------------


    // 【新增】将画框复原到初始状态的函数
    private void ResetPoster()
    {
        if (poster != null && posterRigidbody != null)
        {
            // 1. 停止所有运动：确保画框在复原时不带着速度或角速度
            posterRigidbody.velocity = Vector3.zero;
            posterRigidbody.angularVelocity = Vector3.zero;

            // 2. 禁用重力（防止复原后立即掉落）
            posterRigidbody.useGravity = false;

            // 3. 重置位置和旋转
            poster.transform.position = initialPosterPosition;
            poster.transform.rotation = initialPosterRotation;

            // 4. 确保画框是激活状态 (如果它被隐藏过)
            poster.SetActive(true);

            Debug.Log("画框已复原到初始位置。");
        }
        else if (poster == null)
        {
            Debug.LogError("Poster 引用丢失，无法复原画框。");
        }
    }
    void TeleportPlayerToSpawn()
    {
        GameObject player = GameObject.FindWithTag("Player"); // 确保您的玩家对象有 Tag: Player
        if (player != null && playerSpawnPoint != null)
        {
            // 注意：如果玩家使用的是 CharacterController 组件，可能需要先禁用再启用
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;

            player.transform.position = playerSpawnPoint.position;
            player.transform.rotation = playerSpawnPoint.rotation;

            if (controller != null) controller.enabled = true;
        }
    }

    // 核心功能 1：触发下一个循环 (进入列车)
    public void StartNewLoop()
    {
        // 【关键改动 1】：如果已经在转换中，则立即退出函数
        if (isTransitioning)
        {
            Debug.LogWarning("StartNewLoop 被重复调用，已忽略本次调用。");
            return;
        }
        currentLoopIndex++;
        isTransitioning = true;
        StartCoroutine(LoopTransition());
    }

    // 核心功能 2：清零重置循环 (下楼/退出)
    public void ResetLoop()
    {
        currentLoopIndex = 0;
        StartCoroutine(LoopTransition());
    }

    // -----------------------------------------------------
    // 核心功能 3：循环转换流程 (包含文字显示逻辑)
    // -----------------------------------------------------
    IEnumerator LoopTransition()
    {
        //// 1. **淡入黑屏** (假设 1.0s)
        //FadeToBlack.Instance.FadeIn(); 
        yield return null;

        // 2. **清除旧内容、传送玩家**
        CleanupPreviousContent();
        TeleportPlayerToSpawn(); // 【更新】使用新增的传送函数

        // 3. **生成新内容**
        GenerateLoopContent();

        // 5. **淡出黑屏** (假设 1.0s)
        //// FadeToBlack.Instance.FadeOut(); 
        yield return new WaitForSeconds(1.0f);
        // 4. **显示站台信息**
        StartCoroutine(DisplayStationTextCoroutine(3.0f));
        isTransitioning = false;
    }

    // -----------------------------------------------------
    // 文字显示协程 (保持不变)
    // -----------------------------------------------------
    private IEnumerator DisplayStationTextCoroutine(float duration)
    {
        if (stationText == null)
        {
            Debug.LogError("Station Text UI Component is missing!");
            yield break;
        }

        int stationNumber = currentLoopIndex + 1;
        stationText.text = $"前往第{stationNumber}站";

        stationText.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        stationText.gameObject.SetActive(false);
    }

    // -----------------------------------------------------
    // 内容生成 (保持不变)
    // -----------------------------------------------------
    void CleanupPreviousContent()
    {
        foreach (Transform child in dynamicContentParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void GenerateLoopContent()
    {
        ResetPoster();
        Animator animator = strangeManInScene.GetComponent<Animator>();
        Transform parent = dynamicContentParent.transform;
        // 默认设置：显示 Model 1，隐藏 Model 2
        bool showModel1 = true;
        bool showModel2 = false;

        switch (currentLoopIndex)
        {
            case 0:
                Debug.Log("Loop 0: 正常场景");
                break;
            case 1:
                Debug.Log("Loop 1: 出现奇怪的人");
                if (strangeManInScene != null)
                {
                    // 1. 确保人物是激活状态
                    strangeManInScene.SetActive(true);

                    // 2. 获取人物实例上的 Animator 组件

                    if (animator != null)
                    {
                        // 3. **切换动画状态**
                        // 请使用您的 Animator Controller 中的实际参数名称
                        animator.SetTrigger("manbehave");

                        Debug.Log("奇怪的人的动画已切换到第二个循环状态。");
                    }
                }
                break;
            case 2:
                Debug.Log("Loop 2: 画框掉落");
                animator.SetTrigger("mandown");
                Rigidbody rb=poster.GetComponent<Rigidbody>();
                rb.useGravity = true;
                break;
            case 3:
                Debug.Log("Loop 3: 画面更换");
                animator.SetTrigger("mandown");
                showModel1 = false;
                showModel2 = true;
                break;
            default:
                animator.SetTrigger("mandown");
                Debug.Log($"Loop {currentLoopIndex}: 使用默认内容或随机生成。");
                break;
        }
        if (model_1 != null)model_1.SetActive(showModel1);
        if (model_2 != null)model_2.SetActive(showModel2);
    }
}