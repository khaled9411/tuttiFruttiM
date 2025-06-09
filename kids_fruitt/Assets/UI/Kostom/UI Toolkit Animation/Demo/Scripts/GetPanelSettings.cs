using UnityEngine;
using UnityEngine.UIElements;

namespace Kostom.Demo
{
    [RequireComponent(typeof(UIDocument))]
    public class GetPanelSettings : MonoBehaviour
    {
#if UNITY_EDITOR
        UIDocument document;
        private void OnValidate()
        {
            if (Application.isPlaying) return;

            document = GetComponent<UIDocument>();
            string[] guild = UnityEditor.AssetDatabase.FindAssets("t:panelsettings", new[] { "Assets" });

            if (guild.Length == 0)
            {
                Debug.Log("<color=red>Panel Settings not available in project</color>");
                return;
            }

            document.panelSettings = UnityEditor.AssetDatabase.LoadAssetAtPath<PanelSettings>(UnityEditor.AssetDatabase.GUIDToAssetPath(guild[0]));
        }
#endif
    }
}
