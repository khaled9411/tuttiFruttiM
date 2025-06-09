using UnityEngine;
using UnityEngine.UIElements;
using Kostom.Animation;


namespace Kostom.Demo
{
    public class Example5 : MonoBehaviour
    {
        void Start()
        {
            if (TryGetComponent<UIDocument>(out var document))
            {
                VisualElement root = document.rootVisualElement;

                //disabling the press play panel
                Container(root, 0).style.display = DisplayStyle.None;

                //getting example5's container
                var container = Container(root, 5);

                //querying elements
                #region query

                Button btn = (Button)container.ElementAt(0);
                Label label = (Label)container.ElementAt(0).ElementAt(3);
                var circle1 = container.ElementAt(0).ElementAt(0);
                var leftCurtain = container.ElementAt(0).ElementAt(1);
                var rightCurtain = container.ElementAt(0).ElementAt(2);

                #endregion

                //onclick animation
                #region animation

                var res = btn.AEOnClick(true,
                    new AEAction(leftCurtain,  (x) => x.AEMoveX(-1000, 2)),
                    new AEAction(rightCurtain, (x) => x.AEMoveX(1000, 2)),
                    new AEAction(circle1,      (x) => x.AEScale(new Vector2(10, 10), 1).SetDelay(() => 1f))
                );

                //onclick event
                res.OnStart(() => label.AEText("click to revert", .5f).SetDelay(() => 1f));
                res.OnRevert(() => label.AEText("click to animate", .5f).SetDelay(() => 1f));

                //label animation
                label.AEScale(new Vector2(1.1f, 1.1f), .5f).SetLoops(-1, LoopType.YOYO).SetStepDelay(() => Random.Range(0.1f, 0.3f));

                #endregion
            }
        }

        VisualElement Container(VisualElement root, int index)
        {
            root.ElementAt(index).style.display = DisplayStyle.Flex;
            return root?.ElementAt(index);
        }
    }
}