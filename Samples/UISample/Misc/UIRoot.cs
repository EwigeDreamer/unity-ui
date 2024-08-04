using UnityEngine;

namespace ED.UI.Samples
{
    public class UIRoot : MonoBehaviour, IUIViewRoot
    {
        [field: SerializeField] public Transform Container { get; private set; }
    }
}
