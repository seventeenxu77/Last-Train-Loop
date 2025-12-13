using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class SubwayRun : MonoBehaviour
{
    public Vector3 startPosition = new Vector3(7.4f, 2.7f, 35f);
    public Vector3 endPosition = new Vector3(7.4f, 2.7f, 20f);
    public Vector3 leftPosition = new Vector3(7.4f, 2.7f, -10f);
    public float moveDuration = 5f; // 移动持续时间（秒）
    public float delayBeforeMove = 10f; // 延迟开始移动的时间（秒）

    private float elapsedTime = 0f;
    public float leftMoveTime = 10f;
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
        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition; // 确保最终位置准确
    }
    public void left()
    {
        StartCoroutine(MoveTo(leftPosition, leftMoveTime));
    }
    private IEnumerator MoveTo(Vector3 target, float duration)
    {
        Vector3 start = transform.position;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, elapsed / duration);
            yield return null;
        }

        transform.position = target; // 确保落点精确
    }
}
