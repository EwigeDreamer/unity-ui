using ED.Additional.Utilities;

namespace ED.UI.Enums
{
    public sealed partial class UIRootKey : TypeSafeValueEnum<int, UIRootKey>
    {
        public static readonly UIRootKey Background = new(nameof(Background), -1000);
        public static readonly UIRootKey Main = new(nameof(Main), 0);
        public static readonly UIRootKey Overlay = new(nameof(Overlay), 1000);
        
        private UIRootKey(string name, int value) : base(name, value) { }
    }
}