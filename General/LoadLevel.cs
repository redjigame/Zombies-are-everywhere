using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LoadLevel : MonoBehaviour
{
    [SerializeField]
    private GameObject loadingScreen;
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private Text progressTxt;
    public void LevelLoader(string sceneName)
    {
        StartCoroutine(LoadAsynchronously(sceneName));
    }//LevelLoader

    IEnumerator LoadAsynchronously(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            slider.value = progress;
            progressTxt.text = progress * 100f + "%";

            yield return null;
        }
    }//LoadAsynchronously

}//Class
