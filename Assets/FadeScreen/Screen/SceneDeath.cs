using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class SceneDeath : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Nome da cena para carregar após o grab.")]
    public string sceneToLoad;

    // quad que esta no main camera do jogador
    public FadeScreen fadeScreen;

    public void PlayerDeath()
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
