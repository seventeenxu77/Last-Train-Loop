using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoopManager : MonoBehaviour
{
    // 单例模式
    public static LoopManager Instance;

    [Header("UI 引用")]
    public TextMeshProUGUI stationText;

    [Header("可生成的内容")]
    public GameObject strangeManPrefab;
    public GameObject newBarrierPrefab;
    public GameObject tutorialSignPrefab;

    [Header("循环数据")]
    public int currentLoopIndex = 0;

    // 玩家出生点 (用于每次循环开始时传送玩家)
    public Transform playerSpawnPoint;

    // 用于保存每次循环动态生成的物件的父对象
    private GameObject dynamicContentParent;

    public bool has_exception = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy(gameObject);
        }
    }

    void Start()
    {
        dynamicContentParent = new GameObject("Dynamic Content");

        if (stationText != null)
        {
            stationText.gameObject.SetActive(false);
        }

        GenerateLoopContent();
        has_exception = currentLoopIndex >= 1 ?true:false;  //根据场景生成情况修改has_exception

        // 【新增 1】确保玩家在第一次进入游戏时位于出生点
        TeleportPlayerToSpawn();

        // 【新增 2】第一次进入游戏时也显示站台信息
        StartCoroutine(DisplayStationTextCoroutine(3.0f));
    }

    // -----------------------------------------------------
    // 【新增】核心功能：传送玩家到出生点
    // -----------------------------------------------------
    void TeleportPlayerToSpawn()
    {
        GameObject player = GameObject.FindWithTag("Player"); // 确保您的玩家对象有 Tag: Player
        if (player != null && playerSpawnPoint != null)
        {
            // 注意：如果玩家使用的是 CharacterController 组件，可能需要先禁用再启用
            // CharacterController controller = player.GetComponent<CharacterController>();
            // if (controller != null) controller.enabled = false;

            player.transform.position = playerSpawnPoint.position;

            // if (controller != null) controller.enabled = true;
        }
    }
  
    // 核心功能 1：触发下一个循环 (进入列车)
    public void StartNewLoop()
    {
        currentLoopIndex++;
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
        // 1. **淡入黑屏** (假设 1.0s)
        // FadeToBlack.Instance.FadeIn(); 
        yield return new WaitForSeconds(1.0f);

        // 2. **清除旧内容、传送玩家**
        CleanupPreviousContent();
        TeleportPlayerToSpawn(); // 【更新】使用新增的传送函数

        // 3. **生成新内容**
        GenerateLoopContent();

        // 4. **显示站台信息**
        StartCoroutine(DisplayStationTextCoroutine(10.0f));

        // 5. **淡出黑屏** (假设 1.0s)
        // FadeToBlack.Instance.FadeOut(); 
        yield return new WaitForSeconds(1.0f);
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
        Transform parent = dynamicContentParent.transform;

        switch (currentLoopIndex)
        {
            case 0:
                Debug.Log("Loop 0: 正常场景");
                break;
            case 1:
                Debug.Log("Loop 1: 出现奇怪的人");
                Instantiate(strangeManPrefab, playerSpawnPoint.position + Vector3.forward * 5f, Quaternion.identity, parent);
                break;
            case 2:
                Debug.Log("Loop 2: 增加障碍物");
                Instantiate(newBarrierPrefab, new Vector3(10, 0, 10), Quaternion.identity, parent);
                break;
            default:
                Debug.Log($"Loop {currentLoopIndex}: 使用默认内容或随机生成。");
                break;
        }
    }
}