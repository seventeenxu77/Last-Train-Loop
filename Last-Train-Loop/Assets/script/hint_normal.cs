using System.Collections;
using UnityEngine;
using TMPro;
using System.Xml;

public class hint_normal : MonoBehaviour
{
    // 在Inspector中拖拽UI文本组件
    [SerializeField] private GameObject textUI;
    public int index = 0;
    // 显示时长（秒）
    [SerializeField] private float displayDuration = 3f;

    // 公共属性，自动更新UI

    IEnumerator OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 激活文本
            Debug.Log("hint_normal和玩家碰撞");
            TextMeshProUGUI txt = textUI.GetComponent<TextMeshProUGUI>();
            if(LoopManager.Instance.currentLoopIndex==0)
            {
                txt.text = "还是像往常一样一成不变的车站";
                textUI.SetActive(true);
                yield return new WaitForSeconds(3.0f);
                txt.text = "我真是累坏了，赶紧进列车坐着休息一下";

                // 如果是临时显示，启动协程
                if (displayDuration > 0)
                {
                    StartCoroutine(HideTextAfterDelay());
                }
            }
            else if (LoopManager .Instance.currentLoopIndex==1)
            {
                txt.text = "我眼花了吗？这个车站有些陌生";
                textUI.SetActive(true);
                yield return new WaitForSeconds(3.0f);
                txt.text = "尤其是这个疯子，我要下楼离他远点";
                // 如果是临时显示，启动协程
                if (displayDuration > 0)
                {
                    StartCoroutine(HideTextAfterDelay());
                }
            }
        }
    }

    IEnumerator HideTextAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        textUI.SetActive(false);
    }
}
