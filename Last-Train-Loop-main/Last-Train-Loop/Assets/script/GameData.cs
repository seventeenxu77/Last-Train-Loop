public static class GameData
{
    // 静态变量，场景重启或切换时不会重置
    public static int CurrentLoopIndex = 0;
    public static int ResetTimes = 0;

    // 彻底归零的方法（在通关或回到主菜单时调用）
    public static void GlobalReset()
    {
        CurrentLoopIndex = 0;
        ResetTimes = 0;
    }
}