using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetPlayer : MonoBehaviour
{
    // carrega a mesma cena
    private string sceneToLoad;

    // quad que esta no main camera do jogador
    public FadeScreen fadeScreen;

    private void Awake()
    {
        sceneToLoad = SceneManager.GetActiveScene().name;

        // Procura o componente FadeScreen nos filhos da Câmera Principal (ou na própria câmera)
        fadeScreen = Camera.main.GetComponentInChildren<FadeScreen>();
    }

    private void OnTriggerEnter(Collider other)
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
