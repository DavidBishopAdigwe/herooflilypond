using System.Reflection;
using UnityEngine.Rendering.Universal;

public static class Light2DLayersMaskAccessExtension
{
    public static int[] GetLayers(this Light2D light)
    {
        FieldInfo targetSortingLayersField = typeof(Light2D).GetField("m_ApplyToSortingLayers",
            BindingFlags.NonPublic |
            BindingFlags.Instance);
        int[] mask = targetSortingLayersField!.GetValue(light) as int[];
        return mask;
    }
    public static void SetLayers(this Light2D light, int[] mask)
    {
        FieldInfo targetSortingLayersField = typeof(Light2D).GetField("m_ApplyToSortingLayers",
            BindingFlags.NonPublic |
            BindingFlags.Instance);
        targetSortingLayersField!.SetValue(light, mask);
    }
}