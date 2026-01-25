using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class SceneDeath : MonoBehaviour
{
    // carrega a mesma cena
    public string sceneToLoad;

    // quad que esta no main camera do jogador
    public FadeScreen fadeScreen;

    private void Awake()
    {
        sceneToLoad = SceneManager.GetActiveScene().name;
    }

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
