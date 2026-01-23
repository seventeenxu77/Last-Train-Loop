using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class Subwayrun : MonoBehaviour
{
    public Vector3 startPosition = new Vector3(7.4f, 2.7f, 35f);
    public Vector3 endPosition = new Vector3(7.4f, 2.7f, 8.15f);
    public Vector3 leftPosition = new Vector3(7.4f, 2.7f, -3000f);
    public float moveDuration = 5f; // 移动持续时间（秒）
    public float delayBeforeMove = 20f; // 延迟开始移动的时间（秒）

    private float elapsedTime = 0f;
    public float leftMoveTime = 5f;
    public GameObject man; // 直接拖拽场景中的人物到这里
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
    public void left() //结束调用
    {
        StartCoroutine(MoveTo(leftMoveTime));
    }
    private IEnumerator MoveTo( float duration)
    {

        Vector3 start = transform.position;
        float elapsed = 0;
        Vector3 pos = man.transform.position;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, leftPosition, elapsed / duration);
            man.transform.position= Vector3.Lerp(pos, pos+ leftPosition- endPosition, elapsed / duration);
            yield return null;
        }

    }
}
