using UnityEngine;
using System.Collections;

public class Subwayrun : MonoBehaviour
{
    [Header("位置设置")]
    public Vector3 startPosition = new Vector3(7.4f, 2.7f, 35f);
    public Vector3 endPosition = new Vector3(7.4f, 2.7f, 8.15f);
    public Vector3 leftPosition = new Vector3(7.4f, 2.7f, -3000f);

    [Header("时间设置")]
    public float moveDuration = 5f; // 进站移动持续时间（秒）
    public float delayBeforeMove = 20f; // 延迟开始移动的时间（秒）
    public float leftMoveTime = 5f; // 离站移动时间

    [Header("关键引用")]
    public GameObject man; // 直接拖拽场景中的人物到这里

    // [新增] 动画曲线：用于控制进站时的速度变化
    [Header("进站减速曲线")]
    [Tooltip("请在Inspector里把这条线调成：前面是斜直线(匀速)，最后变平(减速)")]
    public AnimationCurve decelerationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

    void Start()
    {
        transform.position = startPosition;
        Invoke(nameof(StartMoving), delayBeforeMove);
    }

    public void StartMoving()
    {
        StartCoroutine(MoveTrain());
    }

    // [核心修改] 进站逻辑
    private IEnumerator MoveTrain()
    {
        float timer = 0f; // 使用局部变量计时更安全

        while (timer < moveDuration)
        {
            timer += Time.deltaTime;

            // 1. 计算线性的时间进度 (0 到 1)
            float linearT = timer / moveDuration;

            // 2. [关键] 使用曲线重新计算进度
            // 如果曲线是直的，速度就不变；如果曲线变平，速度就变慢
            float curvedT = decelerationCurve.Evaluate(linearT);

            // 3. 使用经过曲线处理的 curvedT 进行插值
            transform.position = Vector3.Lerp(startPosition, endPosition, curvedT);

            yield return null;
        }

        transform.position = endPosition; // 确保最终位置准确
    }

    // --- 以下代码保持不变，处理离站逻辑 ---

    public void left() // 结束调用
    {
        StartCoroutine(MoveTo(leftMoveTime));
    }

    private IEnumerator MoveTo(float duration)
    {
        Vector3 start = transform.position;
        float elapsed = 0;
        Vector3 pos = man.transform.position;

        // 离站通常是加速或者匀速，这里暂时保持原来的线性逻辑
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.position = Vector3.Lerp(start, leftPosition, t);

            // 人物跟随移动
            man.transform.position = Vector3.Lerp(pos, pos + leftPosition - endPosition, t);

            yield return null;
        }
    }
}