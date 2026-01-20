using UnityEngine;

public class OlharJogador : MonoBehaviour
{
    // Corrige o erro CS0029 ao usar GameObject.FindWithTag e acessar o Transform diretamente.
    public Transform camera;

    void Start()
    {
        // Obtém o GameObject com a tag "MainCamera" e acessa seu Transform.
        GameObject cameraGameObject = GameObject.FindWithTag("MainCamera");
        if (cameraGameObject != null)
        {
            camera = cameraGameObject.transform;
        }
    }

    void Update()
    {
        if (camera != null)
        {
            // Calcula a direção do objeto para a câmera.
            Vector3 direcaoParaCamera = camera.position - transform.position;

            // Cria uma rotação que olha na direção OPOSTA à câmera.
            Quaternion rotacaoAlvo = Quaternion.LookRotation(-direcaoParaCamera);

            // Aplica a rotação ao objeto.
            transform.rotation = rotacaoAlvo;
        }
    }
}