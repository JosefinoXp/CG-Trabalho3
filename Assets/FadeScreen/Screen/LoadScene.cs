using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class LoadScene : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Nome da cena para carregar ap�s o grab.")]
    public string sceneToLoad;

    public FadeScreen fadeScreen;

    private void Awake()
    {
        fadeScreen = FindObjectOfType<FadeScreen>();
    }

    public void ChangeScene()
    {
        StartCoroutine(GoToSceneRoutine());
    }

    private IEnumerator GoToSceneRoutine()
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        // Launch the new scene
        SceneManager.LoadScene(sceneToLoad);
    }
}