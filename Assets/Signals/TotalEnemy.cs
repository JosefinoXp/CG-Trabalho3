using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TotalEnemy : MonoBehaviour
{
    [Header("Configurações")]
    public string tagDoInimigo = "Enemy"; // A tag que seus inimigos usam
    
    public int stepIndex;
    private int quantidadeInimigos;

    public PlaySteps playSteps;

    private List<GameObject> listaDeInimigos = new List<GameObject>();


    private void Awake()
    {
        playSteps = FindAnyObjectByType(typeof(PlaySteps)) as PlaySteps;
    }

    void Start()
    {
        // 1. Encontra todos os inimigos pela tag e adiciona na lista
        GameObject[] encontrados = GameObject.FindGameObjectsWithTag(tagDoInimigo);
        listaDeInimigos.AddRange(encontrados);

        Debug.Log($"Jogo começou! Total de inimigos na lista: {listaDeInimigos.Count}");
    }

    void Update()
    {
        // 2. Remove da lista qualquer inimigo que seja 'null' (ou seja, foi destruído)
        // A função RemoveAll retorna QUANTOS foram removidos nessa verificação
        int inimigosMortosAgora = listaDeInimigos.RemoveAll(inimigo => inimigo == null);

        // 3. Se removeu alguém (> 0), significa que um inimigo foi derrotado neste frame
        if (inimigosMortosAgora > 0)
        {
            MetodoQuandoInimigoMorre();
        }
    }

    // --- Este é o método específico que você pediu ---
    void MetodoQuandoInimigoMorre()
    {
        Debug.Log($"Um inimigo foi derrotado! Restam: {listaDeInimigos.Count}");

        // Verifica se a lista ficou vazia
        if (listaDeInimigos.Count == 0)
        {
            LiberarProximaEtapa();
        }
    }

    void LiberarProximaEtapa()
    {
        Debug.Log("Fase Concluída! Carregando próxima etapa...");

        playSteps.PlayStep(stepIndex);
    }
}