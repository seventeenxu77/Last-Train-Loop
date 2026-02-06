using UnityEngine;
using UnityEngine.UI; // 如果使用 UI 息屏需要这个
using System.Collections;

public class dayunontroller : MonoBehaviour
{
    [Header("移动设置")]
    public Transform targetPosition; 
    public float waitTime = 3.0f;    
    public float acceleration = 30f; 
    public float maxSpeed = 120f;    

    [Header("灯光强化")]
    public Light trainLight;         
    public float maxIntensity = 20f; // 调高这个值，比如 20-50
    public float maxRange = 100f;    // 增加照射范围

    [Header("震动强化")]
    public Transform playerCamera;   
    public float maxShakeAmount = 0.5f; // 最大震动幅度

    [Header("息屏设置")]
    public CanvasGroup blackScreen;  // 在 UI 创建一个全黑 Image 加上 CanvasGroup

    private bool isMoving = false;
    private float currentSpeed = 0f;
    private Vector3 originalCamPos;  
    private bool isSequenceStarted = false;

    void Start()
    {
        if (trainLight != null) {
            trainLight.intensity = 0;
            trainLight.range = 0;
        }
        if (blackScreen != null) blackScreen.alpha = 0;
    }

    public void StartTrainSequence()
    {
        if (!isSequenceStarted)
        {
            isSequenceStarted = true;
            StartCoroutine(TrainSequenceRoutine());
        }
    }

    IEnumerator TrainSequenceRoutine()
    {
        float elapsed = 0f;
        if (playerCamera != null) originalCamPos = playerCamera.localPosition;

        // --- 1. 等待期：指数级增强的震动和光照 ---
        while (elapsed < waitTime)
        {
            elapsed += Time.deltaTime;
            // 使用平滑插值，让最后阶段更有爆发力
            float progress = elapsed / waitTime; 
            float powerProgress = Mathf.Pow(progress, 3); // 3次方让震动在最后猛然增强

            // 灯光：亮度和范围同时增加
            if (trainLight != null) {
                trainLight.intensity = progress * maxIntensity;
                trainLight.range = progress * maxRange;
            }

            // 震动：指数级增强
            if (playerCamera != null)
            {
                float currentShake = powerProgress * maxShakeAmount;
                playerCamera.localPosition = originalCamPos + Random.insideUnitSphere * currentShake;
            }

            yield return null;
        }

        // --- 2. 冲刺期 ---
        isMoving = true;
        
        // 冲刺过程中震动保持在最大值，直到撞击
        while (isMoving)
        {
            if (playerCamera != null)
                playerCamera.localPosition = originalCamPos + Random.insideUnitSphere * maxShakeAmount;
            yield return null;
        }
    }

    void Update()
    {
        if (isMoving)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, currentSpeed * Time.deltaTime);
            
            if (Vector3.Distance(transform.position, targetPosition.position) < 0.1f)
            {
                isMoving = false;
                if (playerCamera != null) playerCamera.localPosition = originalCamPos;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(HitAndReset());
        }
    }

    IEnumerator HitAndReset()
    {
        // 1. 瞬间息屏
        if (blackScreen != null)
        {
            float t = 0;
            while (t < 0.1f) // 0.1秒极速黑屏
            {
                t += Time.deltaTime;
                blackScreen.alpha = t / 0.1f;
                yield return null;
            }
            blackScreen.alpha = 1;
        }

        // 停止震动和移动
        isMoving = false;
        if (playerCamera != null) playerCamera.localPosition = originalCamPos;

        Debug.Log("息屏完成，开始循环逻辑");

        // 2. 执行你的循环逻辑
        if (LoopManager.Instance.has_exception) { 
            LoopManager.Instance.StartNewLoop(); 
        }
        else {
            LoopManager.Instance.ResetLoop(); 
        }

        // 3. 循环加载后黑屏淡出（可选）
        yield return new WaitForSeconds(0.5f);
        if (blackScreen != null) blackScreen.alpha = 0;
    }
}