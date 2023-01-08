using System;
using Steel;

namespace SteelCustom.UIElements
{
    public class UIResource : ScriptComponent
    {
        public bool IsGold { get; set; }
        public ResourceType ResourceType { get; set; }
        private Entity _tooltip;

        public override void OnMouseEnterUI()
        {
            _tooltip = UITooltip.ShowTooltip(GetDescription(), 200);
        }

        public override void OnMouseExitUI()
        {
            UITooltip.HideTooltip(_tooltip);
        }

        private string GetDescription()
        {
            if (IsGold)
                return "Gold is a precious metal with a bright yellow color, high luster, and good conductivity, used in jewelry, currency, and a variety of industrial and decorative applications.\n" +
                       "More gold will give you more score in the end.";

            return GetResourceDescription(ResourceType);
        }

        public static string GetResourceDescription(ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.Wood:
                    return "Wood is a porous and fibrous structural tissue found in the stems and roots of trees and other woody plants, used for construction, fuel, and a variety of other purposes.\n" +
                           "Can be used to increase storage capacity of your settlement.";
                case ResourceType.Tobacco:
                    return "Tobacco is an annual herbaceous plant grown for its leaves, which are cured and used in the production of cigarettes, cigars, and other tobacco products.\n" +
                           "Can be sold for a good price.";
                case ResourceType.Corn:
                    return "Corn, also known as maize, is a cereal grain that is native to the Americas and is widely cultivated for its edible starchy seeds, which are used as a food source and in the production of a variety of products.\n" +
                           "Can be used to feed people and grow your settlement.";
                case ResourceType.Cotton:
                    return "Cotton is a soft, white fiber that grows around the seeds of the cotton plant, a flowering shrub native to tropical and subtropical regions, and is used to make a variety of products including textiles, clothing, and home furnishings.\n" +
                           "Can be sold for a good price.";
                case ResourceType.Spices:
                    return "Chili peppers are plants in the nightshade family that are cultivated for their spicy, pungent fruits, which are used as a spice or condiment in many cuisines around the world.\n" +
                           "Can be sold for a good price.";
                default:
                    throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null);
            }
        }
    }
}