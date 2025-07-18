using UnityEngine;

namespace Managers
{
    [CreateAssetMenu(menuName = "ScriptableObject/ScreenControl", fileName = "ScreenControl")]
    public class ScreenController : ScriptableObjectSingleton<ScreenController>
    {
        public void SetResolution(int x, int y)
        {
            Screen.SetResolution(x, y, Screen.fullScreen);
        }

        public void SetFullScreen(bool toggle)
        {
            Screen.fullScreen = toggle;
        }
    }
}