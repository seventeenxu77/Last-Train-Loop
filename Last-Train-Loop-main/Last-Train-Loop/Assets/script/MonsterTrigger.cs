using UnityEngine;

public class MonsterTrigger : MonoBehaviour
{
    public MouthMonster monster; 
    private bool hasTriggered = false; // 用变量代替 Destroy

    private void OnTriggerEnter(Collider other)
    {
        // 只有玩家进入 且 还没触发过时 才执行
        if (other.CompareTag("Player") && !hasTriggered)
        {
            if (monster != null)
            {
                monster.Activate();
                hasTriggered = true; // 标记为已触发
                // 不要 Destroy(gameObject); 
                Debug.Log("大嘴怪触发器已激活");
            }
        }
    }

    // [新增] 给 LoopManager 调用的重置接口
    public void ResetTrigger()
    {
        hasTriggered = false;
        Debug.Log("触发器已复位，可以再次触发");
    }
}