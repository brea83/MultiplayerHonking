using UnityEngine;
using UnityEngine.UIElements;
public static class UiExtensions
{
    public static void Display(this VisualElement element, bool enabled)
    {
        if (element == null) return;
        element.style.display = enabled ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public static void Flex(this VisualElement element, bool isFlex)
    {
        if (element == null) return;
        element.style.display = isFlex ? DisplayStyle.Flex : DisplayStyle.None;
    }
    public static void FocusFirstElement(this VisualElement element, string focusTarget)
    {
        element.Q<VisualElement>(focusTarget).Focus();
    }
}
