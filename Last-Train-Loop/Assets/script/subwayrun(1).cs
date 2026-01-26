using UnityEngine;
using System.Collections;

public class Subwayrun : MonoBehaviour
{
    [Header("位置设置")]
    public Vector3 startPosition = new Vector3(7.4f, 2.7f, 35f);
    public Vector3 endPosition = new Vector3(7.4f, 2.7f, 8.15f);
    public Vector3 leftPosition = new Vector3(7.4f, 2.7f, -3000f);

    [Header("时间设置")]
    public float moveDuration = 5f;        // 进站用时
    public float delayBeforeMove = 20f;    // 进站前的等待
    public float leftMoveTime = 5f;       // 离站用时

    [Header("引用")]
    public GameObject man;

    void Start()
    {
        transform.position = startPosition;
        Invoke(nameof(StartMoving), delayBeforeMove);
    }

    void StartMoving()
    {
        StartCoroutine(MoveTrain());
    }

    private IEnumerator MoveTrain()
    {
        float elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            // 计算进度 (0 到 1)
            float t = elapsedTime / moveDuration;
            
            // --- 核心修改：使用 SmoothStep 实现减速 ---
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            
            transform.position = Vector3.Lerp(startPosition, endPosition, smoothT);
            yield return null;
        }
        transform.position = endPosition;
    }

    public void left() 
    {
        StartCoroutine(MoveTo(leftMoveTime));
    }

    private IEnumerator MoveTo(float duration)
    {
        Vector3 start = transform.position;
        Vector3 playerStartPos = man.transform.position;
        // 计算玩家相对于列车的最终偏移目标
        Vector3 playerEndPos = playerStartPos + (leftPosition - endPosition);
        
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // --- 核心修改：离站也使用平滑加速和减速 ---
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(start, leftPosition, smoothT);
            man.transform.position = Vector3.Lerp(playerStartPos, playerEndPos, smoothT);
            
            yield return null;
        }
        transform.position = leftPosition;
    }
}