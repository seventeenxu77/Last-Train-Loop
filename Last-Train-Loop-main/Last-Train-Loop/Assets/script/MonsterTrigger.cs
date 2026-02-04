using UnityEngine;

public class MonsterTrigger : MonoBehaviour
{
    public MouthMonster monster; // 拖入场景中的嘴巴怪

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            monster.Activate();
            Destroy(gameObject); // 触发一次后消失
        }
    }
}