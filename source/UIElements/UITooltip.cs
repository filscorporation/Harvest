using System.Collections;
using Steel;

namespace SteelCustom.UIElements
{
    public class UITooltip : ScriptComponent
    {
        private static Entity current = null;
        
        public static Entity ShowTooltip(string info, float height = 40f)
        {
            UIImage image = UI.CreateUIImage(ResourcesManager.GetImage("ui_frame.png"), "Tooltip", GameController.Instance.UIManager.UIRoot);
            image.RectTransform.AnchorMin = Vector2.One;
            image.RectTransform.AnchorMax = Vector2.One;
            image.RectTransform.Pivot = Vector2.One;
            image.RectTransform.AnchoredPosition = new Vector2(-8, -8);
            image.RectTransform.Size = new Vector2(200, height);

            UIText text = UI.CreateUIText(info, "Text", image.Entity);
            text.Color = Color.Black;
            text.TextAlignment = AlignmentType.TopLeft;
            text.TextOverflowMode = OverflowMode.WrapByWords;
            text.RectTransform.AnchorMin = Vector2.Zero;
            text.RectTransform.AnchorMax = Vector2.One;
            text.RectTransform.OffsetMin = new Vector2(12, 12);
            text.RectTransform.OffsetMax = new Vector2(12, 12);
            
            FinishShow(image.Entity);

            return image.Entity;
        }

        private static void FinishShow(Entity tooltip)
        {
            tooltip.StartCoroutine(HideTooltipRoutine(tooltip));

            if (current != null)
                HideTooltip(current);
            current = tooltip;
        }

        public static void HideTooltip(Entity tooltip)
        {
            if (tooltip == null || tooltip.IsDestroyed() || current != tooltip)
                return;
            
            current.Destroy();
            current = null;
        }

        private static IEnumerator HideTooltipRoutine(Entity tooltip)
        {
            yield return new WaitForSeconds(20.0f);
            
            HideTooltip(tooltip);
        }
    }
}