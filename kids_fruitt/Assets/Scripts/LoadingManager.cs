using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingText;
    private int dotsCount = 0;
    private float loadingProgress = 0f;

    void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            loadingProgress = asyncOperation.progress;

            StartCoroutine(UpdateLoadingText());

            if (asyncOperation.progress >= 0.9f)
            {
                if (dotsCount >= 5)
                {
                    asyncOperation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }

    IEnumerator UpdateLoadingText()
    {
        while (dotsCount < 5)
        {
            loadingText.text = "Loading" + new string('.', dotsCount);
            dotsCount++;
            yield return new WaitForSeconds(0.5f);
        }

        if (dotsCount >= 5)
        {
            dotsCount = 0;
        }
    }
}