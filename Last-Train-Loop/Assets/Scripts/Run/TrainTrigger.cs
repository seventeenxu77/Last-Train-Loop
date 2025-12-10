using UnityEngine;

public class TrainTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 确保只有玩家触发
        if (other.CompareTag("Player"))
        {
   
            Debug.Log("进入列车，触发下一循环。");
            if(LoopManager.Instance.has_exception) LoopManager.Instance.StartNewLoop();
            else LoopManager.Instance.ResetLoop();
        }
    }
}