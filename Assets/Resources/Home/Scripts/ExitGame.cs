using UnityEngine;

public class ExitGame : MonoBehaviour
{
    /// <summary>
    /// 退出游戏方法
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
            // 在编辑器模式下退出播放模式
            UnityEditor.EditorApplication.isPlaying = false;
#else
        // 在打包后的应用中退出游戏
        Application.Quit();
#endif
    }

    /// <summary>
    /// 通过按键退出（如ESC键）
    /// </summary>
    private void Update()
    {
        // 检测ESC键按下
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }
}