using UnityEngine;
using UnityEngine.UIElements;
using Kostom.Animation;
using Unity.Mathematics;
using Random = UnityEngine.Random;

namespace Kostom.Demo
{
    public class Example2 : MonoBehaviour
    {
        [SerializeField, Range(0.05f, 0.15f)] float ringSpread = 0.05f;
        [SerializeField, Range(1, 5)] float duration;

        void Start()
        {
            if(TryGetComponent<UIDocument>(out var document))
            {
                VisualElement root = document.rootVisualElement;

                //disabling the press play panel
                Containers(root, 0).style.display = DisplayStyle.None;

                //getting example2's container
                var container = Containers(root, 2);

                SpaceRings(container);
            }
        }

        VisualElement Containers(VisualElement root, int index)
        {
            root.ElementAt(index).style.display = DisplayStyle.Flex;
            return root?.ElementAt(index);
        }

        void SpaceRings(VisualElement container)
        {
            float padding = 0;
            float flexPadding = 0;

            for (int i = 0; i < container.childCount; i++)
            {
                padding = ringSpread * i;
                flexPadding += padding / 2;


                container.ElementAt(i).AEScale(new float2(1f + padding), new float2(.5f + flexPadding), duration).SetLoops(-1, LoopType.YOYO);
            }
        }
    }
}