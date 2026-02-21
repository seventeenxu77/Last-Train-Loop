using UnityEngine;

public class dayuntrigger : MonoBehaviour
{
    public dayunontroller train; // 将场景里的火车拖进来

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // 只有玩家进入且没被触发过时才生效
        if (other.CompareTag("Player") && !triggered)
        {
            triggered = true;
            train.StartTrainSequence();
            Debug.Log("火车序列已启动...");
        }
    }
    public void ResetTrigger()
{
    triggered = false;
    Debug.Log("列车触发器已重置。");
}
}