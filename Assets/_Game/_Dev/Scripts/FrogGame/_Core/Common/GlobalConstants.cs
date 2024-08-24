using UnityEngine;

namespace FrogGame._Core.Common
{
    public static class GlobalConstants
    {
        //PlayerPrefs keys
        public static readonly string CURRENT_LEVEL_KEY = "current_level";

        //Durations
        public static readonly float TWEEN_DURATION_IDEAL = 0.33F;
        public static readonly float DELAY_LEVEL_FINISH = .68F;

        //Vector3
        public static readonly Vector3 CONTENT_SCALED_UP_VECTOR = Vector3.one;
        public static readonly Vector3 CONTENT_SCALED_DOWN_VECTOR = new Vector3(0.01F, 0.01F, 0.01F);

        //Material
        public static readonly string GRAPEMATERIAL_FLASH_COLOR_PROPERTY_ID = "_FlashAmount";
    }
}
