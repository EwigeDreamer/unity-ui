using System;

namespace ED.UI.Enums
{
    [Flags]
    public enum UIOptions : byte
    {
        None = 0,
        
        ShowAnimation = 1 << 0,
        HideAnimation = 1 << 1,
        IsHideable = 1 << 2,
        HidePrevious = 1 << 3,
        
        Animation = ShowAnimation | HideAnimation,
        Hiding = IsHideable | HidePrevious,
        Default = Animation | Hiding,
    }
}