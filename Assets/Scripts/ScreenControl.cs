using Managers;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/ScreenControl", fileName = "ScreenControl")]
public class ScreenControl : ScriptableObjectSingleton<ScreenControl>
{
    public void SetResolution(int x, int y)
    {
        Screen.SetResolution(x, y, Screen.fullScreen);
    }

    public void SetFullScreen(bool on)
    {
        Screen.fullScreen = on;
    }
}