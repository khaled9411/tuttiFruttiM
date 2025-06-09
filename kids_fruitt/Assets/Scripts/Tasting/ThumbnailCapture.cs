using UnityEngine;
using System.IO;

public class ThumbnailCapture : MonoBehaviour
{
    public Camera thumbnailCamera;
    private int imageIndex = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Smile");
            CaptureThumbnail();
        }
    }

    void CaptureThumbnail()
    {
        int width = 1024;
        int height = 1024;

        RenderTexture rt = new RenderTexture(width, height, 24);
        thumbnailCamera.targetTexture = rt;

        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        thumbnailCamera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenShot.Apply();

        thumbnailCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        string fileName = "Thumbnail_" + imageIndex + ".png";
        string fullPath = Path.Combine(Application.dataPath, fileName);
        File.WriteAllBytes(fullPath, screenShot.EncodeToPNG());

        Debug.Log("Saved Thumbnail at: " + fullPath);

        imageIndex++;
    }
}

