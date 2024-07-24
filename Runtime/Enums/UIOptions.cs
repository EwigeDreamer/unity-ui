using System;

namespace ED.UI
{
    [Flags]
    public enum UIOptions : byte
    {
        None = 0,
        
        ShowingAnimation = 1 << 0,
        HidingAnimation = 1 << 1,
        IsHideable = 1 << 2,
        HidePrevious = 1 << 3,
        
        Animation = ShowingAnimation | HidingAnimation,
        Hiding = IsHideable | HidePrevious,
        Default = Animation | Hiding,
    }
}