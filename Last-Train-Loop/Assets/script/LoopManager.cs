using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;

public class LoopManager : MonoBehaviour
{
    // 单例模式

    public int ResetTimes = 0;
    public static LoopManager Instance;

    [Header("传送门引用")]
    public GameObject portal;
    private bool isTransitioning = false;
    [Header("黑板引用")]
    [SerializeField] private Blackboard GlobalBlackBoard;
    //存储循环id
    public int currentEventID = 0;
    [Header("循环随机化设置")]
    // 存储所有可能的事件索引（1到10）
    private List<int> availableEventIndices = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 ,10};
    private List<int> normalEventIndices = new List<int> { 1, 2, 3, 4,10 }; // 正常事件的索引
    private List<int> exceptionEventIndices = new List<int> { 5, 6, 7, 8, 9 }; // 异常事件的索引
    // 【新增】用于保存画框的初始状态
    private Vector3 initialPosterPosition;
    private Quaternion initialPosterRotation;
    private Rigidbody posterRigidbody;

    [Header("UI 引用")]
    public TextMeshProUGUI stationText;
    [SerializeField] private GameObject hintnorma;
    // 【新增】场景中已存在的人物引用
    [Header("列车")]
    public GameObject car; // car
    [Header("站台")]
    public GameObject loud; // loud
    [Header("场景物件引用")]
    public GameObject strangeManInScene; // 直接拖拽场景中的人物到这里
    public GameObject poster;
    [Header("自定义模型")]
    public GameObject model_1; // 第一个模型 (通常显示的)
    public GameObject model_2; // 第二个模型
    [Header("海报")]
    public GameObject poster1; // 第一个海报 (通常显示的)
    public GameObject poster2; // 第二个海报
    [Header("广告")]
    public GameObject adv1; // 第一个广告 (通常显示的)
    public GameObject adv2; // 第二个广告
    [Header("循环数据")]
    public int currentLoopIndex = 0;
    [Header("通关设置")]
    public TextMeshProUGUI gameEndText; // 用于显示 "游戏结束"
    public GameObject trainMovementController; // 控制列车移动的脚本所在的对象

    // 玩家出生点 (用于每次循环开始时传送玩家)
    public Transform playerSpawnPoint;
    // 游戏结束点 (结束游戏传送到列车上)
    public Transform playerSpawnPointend;

    // 用于保存每次循环动态生成的物件的父对象
    private GameObject dynamicContentParent;

    public bool has_exception =false;
    public bool isDarkLoop = false;
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
        AudioTrigger dd = loud.GetComponent<AudioTrigger>();
        gameEndText.gameObject.SetActive(false);
        hintnorma.SetActive(false);
        strangeManInScene.SetActive(true);
        // ------------------------------------------------------------------
        // 【关键新增代码】：在游戏启动时，确保所有备用模型都是隐藏的
        // ------------------------------------------------------------------
        if (model_2 != null) model_2.SetActive(false);
        if (poster2 != null) poster2.SetActive(false);
        if (adv2 != null) adv2.SetActive(false);

        // 确保默认模型是显示的 (如果您的默认模型在编辑器中被隐藏了)
        if (model_1 != null) model_1.SetActive(true);
        if (poster1 != null) poster1.SetActive(true);
        if (adv1 != null) adv1.SetActive(true);
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
        if (portal != null) portal.SetActive(false);

        GenerateLoopContent();

        // 【新增 1】确保玩家在第一次进入游戏时位于出生点
        TeleportPlayerToSpawn();
    }
    // -----------------------------------------------------
    // 【新增】核心功能：传送玩家到出生点
    // -----------------------------------------------------

    //锁定玩家移动
    private void LockPlayerControl()
    {

        // 2. 禁用 CharacterController (推荐：防止物理碰撞导致的微小移动)
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            // 假设 CharacterController 也在同一个或父对象上
            CharacterController controller = player.GetComponent<CharacterController>();

            if (controller != null)
            {
                controller.enabled = false;
                Debug.Log("CharacterController 已禁用。");
            }
        }
    }

    public void UpdateBlackBoard()
    { 
        if (GlobalBlackBoard != null)
        {
            GlobalBlackBoard.SetVariableValue("currentIndex", currentLoopIndex);
            GlobalBlackBoard.SetVariableValue("currentEventID", currentEventID);
            GlobalBlackBoard.SetVariableValue("ResetTimes", ResetTimes);
            GlobalBlackBoard.SetVariableValue("hasException", has_exception);
            GlobalBlackBoard.SetVariableValue("isDarkLoop", isDarkLoop);
        }
    }

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
    void TeleportPlayerToSpawnend()
    {
        GameObject player = GameObject.FindWithTag("Player"); // 确保您的玩家对象有 Tag: Player
        if (player != null && playerSpawnPointend != null)
        {
            // 注意：如果玩家使用的是 CharacterController 组件，可能需要先禁用再启用
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;

            player.transform.position = playerSpawnPointend.position;
            player.transform.rotation = playerSpawnPointend.rotation;

            if (controller != null) controller.enabled = true;
        }
    }

    // 核心功能 1：触发下一个循环 (进入列车)
    public void StartNewLoop()
    {
        //门判断
        if (isTransitioning)
        {
            Debug.LogWarning("StartNewLoop 被重复调用，已忽略本次调用。");
            return;
        }
        currentLoopIndex++;
        if (currentLoopIndex == 6)
        {
            isTransitioning = true; // 关门
            TeleportPlayerToSpawnend();
            StartCoroutine(GameEndSequence());
            return; // 阻止执行转换函数，因为已经关门
        }
        isTransitioning = true;
        StartCoroutine(LoopTransition());
    }

    public void ResetLoop()
    {
        currentLoopIndex = 0;
        StartCoroutine(LoopTransition());
        ResetTimes++;
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
        isTransitioning = false;
    }

    //游戏结束
    // 【新增】通关序列协程
    IEnumerator GameEndSequence()
    {
        // 1. 锁定玩家控制
        LockPlayerControl();

        // 2. 触发列车和人物移动（动画）
        //if (trainMovementController != null)
        //{
        //    // 假设您的列车移动脚本上有 StartMovement() 方法
        //    // trainMovementController.GetComponent<TrainScript>().StartMovement(); 
        //    // 假设您的人物在 Loop 5 也要有特殊动画
        //    // strangeManInScene.GetComponent<Animator>().SetTrigger("RunAway"); 
        //    Debug.Log("环境脚本（列车/人物）已触发。");
        //}

        // 3. 等待动画或移动效果启动 (例如 2 秒)
        yield return new WaitForSeconds(1.0f);
        Subwayrun ss = car.GetComponent<Subwayrun>();
        if (ss != null)ss.left();
        // 4. 显示游戏结束文本
        if (gameEndText != null)
        {
            gameEndText.text = "我真的离开了这个鬼地方了吗？";
            gameEndText.gameObject.SetActive(true);
            yield return new WaitForSeconds(3.0f);
            gameEndText.text = "游戏结束\n感谢您的游玩！";
        }

        Debug.Log("游戏结束文本已显示。");

        // 5. 永久等待（或加载主菜单）
        // yield return new WaitForSeconds(10.0f); // 保持文本显示一段时间
        // SceneManager.LoadScene("MainMenu");
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
    void checkIsDarkLoop()
    {
        SetDarkLoop setDarkLoop = GameObject.Find("DarkLoop").GetComponent<SetDarkLoop>();
        if (setDarkLoop == null) Debug.LogWarning("未找到setDarkLoop");
        setDarkLoop.gameObject.SetActive(true);
        if (isDarkLoop) setDarkLoop.ActiveAll();
        else setDarkLoop.InActiveAll();
    }
    void GenerateLoopContent()
    {
        //管理黑夜关卡
        isDarkLoop = false;
        //SetDarkLoop setDarkLoop = GameObject.Find("DarkLoop").GetComponent<SetDarkLoop>();
        //setDarkLoop.InActiveAll();
        ResetPoster();
        Animator animator = strangeManInScene.GetComponent<Animator>();
        Transform parent = dynamicContentParent.transform;
        if (portal != null) portal.SetActive(false);
        // 默认显示1隐藏2
        bool showModel1 = true;
        bool showModel2 = false;
        bool showposter1 = true;
        bool showposter2 = false;
        bool showadv1 = true;
        bool showadv2 = false;
        AudioTrigger dd = loud.GetComponent<AudioTrigger>();
        dd.PlaySound();
        if (animator != null)
        {
            // 清除所有可能干扰的 Trigger，否则人物动画无法切换
            animator.ResetTrigger("mandown");
            animator.ResetTrigger("manbehave");//animator还没有完成对mandown的处理会进入等待过渡，紧接着就调用manbehave会被吞没指令
        }
        if (true)
        {
            int totalEvents = normalEventIndices.Count + exceptionEventIndices.Count;
            int randomIndex = Random.Range(1, totalEvents + 1); // 随机抽取 1 到 10 之间的一个数

            // 所有事件索引合并，随机抽取一个
            List<int> combinedEvents = new List<int>(normalEventIndices);
            combinedEvents.AddRange(exceptionEventIndices);

            int eventIndex = Random.Range(0, combinedEvents.Count);
            currentEventID = combinedEvents[eventIndex];
        }
        if (currentLoopIndex == 0)
        {
            currentEventID = 0;
        }
        if (currentLoopIndex == 1)
        {
            currentEventID = 12;
        }
        if (currentLoopIndex == 2)
        {
            currentEventID = 11;
        }
        if (currentLoopIndex == 5)
        {
            currentEventID = 8;
        }
        Debug.Log($"循环次数 {currentLoopIndex}: 触发事件 ID {currentEventID}");
        switch (currentEventID)
        {
            case 0 or 1 or 2 or 3: 
                Debug.Log($"case{currentEventID}: 正常场景");
                animator.SetTrigger("mandown");
                has_exception = false;
                break;
            case 4:
                Debug.Log("case 4: 出现奇怪的人");
                if (strangeManInScene != null)
                {
                    if (animator != null)
                    {
                        animator.SetTrigger("manbehave");
                        Debug.Log("奇怪的人的动画已切换到第二个循环状态。");
                    }
                }
                has_exception = true;
                break;
            case 5:
                Debug.Log("case 5: 画框掉落");
                animator.SetTrigger("mandown");
                Rigidbody rb=poster.GetComponent<Rigidbody>();
                rb.useGravity = true;
                has_exception =true;
                break;
            case 6:
                Debug.Log("case 6：58画面更换");
                animator.SetTrigger("mandown");
                showModel1 = false;
                showModel2 = true;
                has_exception = true;
                break;
            case 7:
                Debug.Log("case 7: 消失的她画面更换");
                animator.SetTrigger("mandown");
                showposter1 = false;
                showposter2 = true;
                has_exception = true;
                break;
            case 8:
                Debug.Log("case 8: 满头大汉");
                animator.SetTrigger("mandown");
                has_exception = true;
                break;
            case 9:
                Debug.Log("case 9: 广告异常");
                animator.SetTrigger("mandown");
                showadv1 = false;
                showadv2 = true;
                has_exception = true;
                break;
            case 10:
                Debug.Log("case 10: 正常场景");
                animator.SetTrigger("mandown");
                has_exception = false;
                break;  
            case 11:
                Debug.Log("case 11: 开启传送门");
                animator.SetTrigger("mandown");
                has_exception = true;
        
                // 【核心操作】在这里激活传送门
                if (portal != null) portal.SetActive(true); 
                break;
            case 12:
                Debug.Log("case 12: 黑夜关卡");
                animator.SetTrigger("mandown");
                isDarkLoop = true;
                has_exception = true;
                break;
            default:
                animator.SetTrigger("mandown");
                Debug.Log($"Loop {currentLoopIndex}: 使用默认内容或随机生成。");
                break;
        }
        if (model_1 != null)model_1.SetActive(showModel1);
        if (model_2 != null)model_2.SetActive(showModel2);
        if (poster1 != null) poster1.SetActive(showposter1);
        if (poster2!= null) poster2.SetActive(showposter2);
        if (adv1 != null) adv1.SetActive(showadv1);
        if (adv2 != null) adv2.SetActive(showadv2);
        UpdateBlackBoard();
        checkIsDarkLoop();
    }
}