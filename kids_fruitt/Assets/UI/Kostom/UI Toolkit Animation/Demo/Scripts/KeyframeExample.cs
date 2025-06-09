using Kostom.Animation;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kostom.Demo
{
    public class KeyframeExample : MonoBehaviour
    {
        [SerializeField] bool useController;
        [SerializeField] UITKAnimation m_animation;
        [SerializeField] UITKController m_controller;

        // Start is called before the first frame update
        void Start()
        {
            if (TryGetComponent<UIDocument>(out var document))
            {
                VisualElement root = document.rootVisualElement;

                if (useController)
                {
                    root.Q("Card").PlayUITKAnim(m_controller, "Roll").OnComplete(() => Debug.Log("Completed"));
                }
                else
                {
                    root.Q("Card").PlayUITKAnim(m_animation).OnComplete(() => Debug.Log("Completed"));
                }
            }
        }
    }
}
