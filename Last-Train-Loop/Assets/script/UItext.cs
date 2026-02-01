using System.Collections;
using UnityEngine;
using TMPro;
using System.Xml;

public class UItext : MonoBehaviour
{
    // 在Inspector中拖拽UI文本组件
    [SerializeField] private GameObject textUI;
    // 显示时长（秒）
    [SerializeField] private float displayDuration = 3f;

    // 公共属性，自动更新UI

    void OnTriggerEnter(Collider other)
    {
        int index=LoopManager.Instance.currentLoopIndex;
        if (other.CompareTag("Player"))
        {
            // 激活文本
            Debug.Log("与玩家碰撞");
            TextMeshProUGUI txt = textUI.GetComponent<TextMeshProUGUI>();
            txt.text = $"11：{(index+1)*10}";
            textUI.SetActive(true);

            // 如果是临时显示，启动协程
            if (displayDuration > 0)
            {
                StartCoroutine(HideTextAfterDelay());
            }
        }
    }

    IEnumerator HideTextAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        textUI.SetActive(false);
    }
}
