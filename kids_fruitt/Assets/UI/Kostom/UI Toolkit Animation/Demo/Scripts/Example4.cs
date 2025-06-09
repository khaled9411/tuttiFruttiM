using UnityEngine;
using UnityEngine.UIElements;
using Kostom.Animation;


namespace Kostom.Demo
{
    public class Example4 : MonoBehaviour
    {
        void Start()
        {
            if (TryGetComponent<UIDocument>(out var document))
            {
                VisualElement root = document.rootVisualElement;

                //disabling the press play panel
                Container(root, 0).style.display = DisplayStyle.None;

                //getting example4's container
                var container = Container(root, 4);


                var email = container.ElementAt(0).ElementAt(0);
                Button button = (Button)(container.ElementAt(0).ElementAt(3));
                Label label = (Label)container.ElementAt(0).ElementAt(4);
                label.AEText("click <b>LOGIN</b> to see the animation", 1.5f);

                //Animation
                var res = button.AEOnClick((x) => ((Button)x).AEText("Register", .5f), true,
                    new AEAction(email, (x) =>
                    {
                        return x.AEOpacity(0, .5f).OnComplete(HideEmail);
                    }),
                    new AEAction(label, (x) =>
                    {
                        return ((Label)x).AEText("click <b>REGISTER</b> to undo animation", 1f);
                    }),
                    new AEAction(button, (x) =>
                    {
                        return x.AEBackgroundColor(Color.red, .5f);
                    })
                );

                res.OnRevert(ShowEmail);

                void HideEmail() => email.style.display = DisplayStyle.None;

                void ShowEmail() => email.style.display = DisplayStyle.Flex;
            }
        }

        VisualElement Container(VisualElement root, int index)
        {
            root.ElementAt(index).style.display = DisplayStyle.Flex;
            return root?.ElementAt(index);
        }
    }
}