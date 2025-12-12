using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    public float LoadingProgress { get; private set; }
    public event Action<float> OnProgress;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadAsync(sceneName));
    }

    IEnumerator LoadAsync(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            LoadingProgress = op.progress / 0.9f;
            OnProgress?.Invoke(LoadingProgress);
            yield return null;
        }

        LoadingProgress = 1f;
        OnProgress?.Invoke(1f);

        yield return new WaitForSeconds(0.2f);
        op.allowSceneActivation = true;
    }
}
