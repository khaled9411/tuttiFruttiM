using UnityEngine;
using UnityEngine.UIElements;
using Kostom.Animation;


namespace Kostom.Demo
{
    public class Example3 : MonoBehaviour
    {
        [SerializeField] float duration;
        [SerializeField] Color firstHoverColor, secondHoverColor, thirdHoverColor, fourthHoverColor;

        Label label;
        VisualElement first, second, third, fourth, bar;
        VisualElement navbar;

        void Start()
        {
            if(TryGetComponent<UIDocument>(out var document))
            {
                VisualElement root = document.rootVisualElement;

                //disabling the press play panel
                Container(root, 0).style.display = DisplayStyle.None;

                //getting example3's container
                var container = Container(root, 3);

                label = (Label)container.ElementAt(0);
                bar = container.ElementAt(1).ElementAt(1);

                //Gettin element's reference
                navbar = container.ElementAt(1);
                first = container.ElementAt(1).ElementAt(0).ElementAt(0);
                second = container.ElementAt(1).ElementAt(0).ElementAt(1);
                third = container.ElementAt(1).ElementAt(0).ElementAt(2);
                fourth = container.ElementAt(1).ElementAt(0).ElementAt(3);


                //Animating
                bar.AEMoveY(-10, 1.5f).SetLoops(-1, LoopType.YOYO);
                Hover();
            }
        }

        VisualElement Container(VisualElement root, int index)
        {
            root.ElementAt(index).style.display = DisplayStyle.Flex;
            return root?.ElementAt(index);
        }

        void Hover()
        {

            //FIRST CIRCLE HOVER
            first.AEOnHover((x) => x.AEBorderWidth(12, duration));
            first.AEOnHover((x) => x.AEBorderColor(Color.white, duration),true,
                new AEAction(navbar, (x) => x.AEBackgroundColor(firstHoverColor, duration)),
                new AEAction(navbar, (x) => x.AEScale(new Vector2(1.1f, 1.1f), duration)),
                new AEAction(label, (x) => ((Label)x).AEText("First with animate reverse", duration)));

            //SECOND CIRCLE HOVER
            second.AEOnHover((x) => x.AEBorderWidth(12, duration));
            second.AEOnHover((x) => x.AEBackgroundColor(Color.black, duration));
            second.AEOnHover((x) => x.AEBorderColor(Color.white, duration),
                new AEAction(navbar, (x) => x.AEBackgroundColor(secondHoverColor, duration)),
                new AEAction(navbar, (x) => x.AEScale(new Vector2(1.1f, 1.1f), duration)),
                new AEAction(first, (x) => x.AEBackgroundColor(Color.black,duration)),
                new AEAction(third, (x) => x.AEBackgroundColor(Color.black, duration)),
                new AEAction(fourth, (x) => x.AEBackgroundColor(Color.black, duration)),
                new AEAction(bar, (x) => x.AEBackgroundColor(Color.black, duration)),
                new AEAction(label, (x) => ((Label)x).AEText("Second", duration)));

            //THIRD CIRCLE HOVER
            third.AEOnHover((x) => x.AEBorderWidth(12, duration));
            third.AEOnHover((x) => x.AEBackgroundColor(Color.black, duration));
            third.AEOnHover((x) => x.AEBorderColor(Color.white, duration),
                new AEAction(navbar, (x) => x.AEBackgroundColor(thirdHoverColor, duration)),
                new AEAction(navbar, (x) => x.AEScale(new Vector2(1.1f, 1.1f), duration)),
                new AEAction(first, (x) => x.AEBackgroundColor(Color.black, duration)),
                new AEAction(second, (x) => x.AEBackgroundColor(Color.black, duration)),
                new AEAction(fourth, (x) => x.AEBackgroundColor(Color.black, duration)),
                new AEAction(bar, (x) => x.AEBackgroundColor(Color.black, duration)),
                new AEAction(label, (x) => ((Label)x).AEText("Third", duration)));

            //THIRD CIRCLE HOVER
            fourth.AEOnHover((x) => x.AEBorderWidth(12, duration));
            fourth.AEOnHover((x) => x.AEBackgroundColor(Color.black, duration));
            fourth.AEOnHover((x) => x.AEBorderColor(Color.white, duration),
                new AEAction(navbar, (x) => x.AEBackgroundColor(fourthHoverColor, duration)),
                new AEAction(navbar, (x) => x.AEScale(new Vector2(1.1f, 1.1f), duration)),
                new AEAction(first, (x) => x.AEBackgroundColor(Color.black, duration)),
                new AEAction(second, (x) => x.AEBackgroundColor(Color.black, duration)),
                new AEAction(third, (x) => x.AEBackgroundColor(Color.black, duration)),
                new AEAction(bar, (x) => x.AEBackgroundColor(Color.black, duration)),
                new AEAction(label, (x) => ((Label)x).AEText("Fourth", duration)));
        }
    }
}