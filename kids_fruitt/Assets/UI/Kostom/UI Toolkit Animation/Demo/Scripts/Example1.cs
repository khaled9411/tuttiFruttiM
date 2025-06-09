using UnityEngine;
using UnityEngine.UIElements;
using Kostom.Animation;
using Unity.Mathematics;

namespace Kostom.Demo
{
    public class Example1 : MonoBehaviour
    {
        [SerializeField] AnimationCurve pbCurve;
        void Start()
        {
            if (TryGetComponent<UIDocument>(out var document))
            {
                VisualElement root = document.rootVisualElement;

                //disabling the press play panel
                Containers(root, 0).style.display = DisplayStyle.None;

                //getting example1's container
                var container = Containers(root, 1);

                CircleAnimation(container);

                ProgressBarAnimation(container);
            }
        }

        private void CircleAnimation(VisualElement container)
        {
            var elementsParent = ElementsParent(container);

            var val = GetElement(elementsParent, 0);

            var val2 = GetElement(elementsParent, 1);

            var val3 = GetElement(elementsParent, 2);

            //first circle scale animation
            //this scales from (1, 1) - (1.5f, 1.5f)
            val.Item1.AEScale(new float2(1.5f), 1f).SetLoops(-1, LoopType.YOYO);

            //first circle's inner circle scale animation
            //this scales from (.5, .5) - (0.25f, 0.25f)
            val.Item2.AEScale(new float2(.25f), 1f).SetLoops(-1, LoopType.YOYO);

            ////second circle move animation without step delay
            val2.Item1.AEMoveY(65f, .5f).SetLoops(-1, LoopType.YOYO).SetDelay(() => 1f).SetEase(Ease.OutBounce);

            ////third circle move animation with step delay
            val3.Item1.AEMoveY(65f, .5f).SetLoops(-1, LoopType.YOYO).SetDelay(() => 1f).SetEase(Ease.OutBounce).SetStepDelay(() => .5f);

            VisualElement ElementsParent(VisualElement container)
            {
                return container?.ElementAt(0)?.ElementAt(0);
            }

            (VisualElement, VisualElement) GetElement(VisualElement elementsParent, int index)
            {
                return (elementsParent?.ElementAt(index), elementsParent?.ElementAt(index)?.ElementAt(0));
            }
        }

        private void ProgressBarAnimation(VisualElement container)
        {
            ProgressBarAnim((ProgressBar)container.ElementAt(1).ElementAt(0).ElementAt(0), Ease.InOutElastic);
            ProgressBarAnim((ProgressBar)container.ElementAt(1).ElementAt(0).ElementAt(1), pbCurve);
        }

        //This uses Ease Func
        void ProgressBarAnim(ProgressBar pb, Ease ease)
        {
            //AnimationEngine.Custom(() => UnityEngine.Random.Range(0f, 5f), (x) => { pb.value = x; pb.title = Mathf.RoundToInt(x).ToString(); }, 25, 2f).SetEase(ease).SetLoops(-1, LoopType.YOYO).SetStepDelay(() => 1f);
            AnimationEngine.Custom(() => UnityEngine.Random.Range(0f, 5f), (x) => { pb.value = x; pb.title = Mathf.RoundToInt(x).ToString(); }, 25, 1f).SetEase(ease).OnComplete(() =>
            {
                AnimationEngine.Custom(() => 25, (x) => { pb.value = x; pb.title = Mathf.RoundToInt(x).ToString(); }, 75, 1f).SetDelay(() => UnityEngine.Random.Range(0f, 1f)).SetEase(ease).OnComplete(() =>
                {
                    AnimationEngine.Custom(() => 75, (x) => { pb.value = x; pb.title = Mathf.RoundToInt(x).ToString(); }, 100, 1f).SetDelay(() => UnityEngine.Random.Range(0f, 1f)).SetEase(ease);
                });
            });
        }

        //This uses Animation Curve
        void ProgressBarAnim(ProgressBar pb, AnimationCurve ease)
        {
            //AnimationEngine.Custom(() => UnityEngine.Random.Range(0f, 5f), (x) => { pb.value = x; pb.title = Mathf.RoundToInt(x).ToString(); }, 25, 2f).SetEase(ease).SetLoops(-1, LoopType.YOYO).SetStepDelay(() => 1f);
            AnimationEngine.Custom(() => UnityEngine.Random.Range(0f, 5f), (x) => { pb.value = x; pb.title = Mathf.RoundToInt(x).ToString(); }, 25, 1f).SetEase(ease).OnComplete(() =>
            {
                AnimationEngine.Custom(() => 25, (x) => { pb.value = x; pb.title = Mathf.RoundToInt(x).ToString(); }, 75, 1f).SetDelay(() => UnityEngine.Random.Range(0f, 1f)).SetEase(ease).OnComplete(() =>
                {
                    AnimationEngine.Custom(() => 75, (x) => { pb.value = x; pb.title = Mathf.RoundToInt(x).ToString(); }, 100, 1f).SetDelay(() => UnityEngine.Random.Range(0f, 1f)).SetEase(ease);
                });
            });
        }

        VisualElement Containers(VisualElement root, int index)
        {
            root.ElementAt(index).style.display = DisplayStyle.Flex;
            return root?.ElementAt(index);
        }
    }
}