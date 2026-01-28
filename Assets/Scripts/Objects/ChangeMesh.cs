using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class ChangeMesh : MonoBehaviour
{
    public GameObject cube;
    public GameObject sphere;
    public GameObject cilider;
    public GameObject tv;

    private MeshRenderer cubeRenderer;
    private MeshRenderer sphereRenderer;
    private MeshRenderer ciliderRenderer;
    private MeshRenderer tvRenderer;

    private void Awake()
    {
        cubeRenderer = cube.GetComponent<MeshRenderer>();
        sphereRenderer = sphere.GetComponent<MeshRenderer>();
        ciliderRenderer = cilider.GetComponent<MeshRenderer>();
        tvRenderer = tv.GetComponent<MeshRenderer>();

        cubeRenderer.enabled = false;
        sphereRenderer.enabled = false;
        ciliderRenderer.enabled = false;
        tvRenderer.enabled = false;

    }

    private void Start()
    {
        StartCoroutine(Change());
    }

    IEnumerator Change()
    {
        while (true) { 
            cubeRenderer.enabled = !(cubeRenderer.enabled);

            yield return new WaitForSeconds(0.4f);

            cubeRenderer.enabled = !(cubeRenderer.enabled);
            sphereRenderer.enabled = !(sphereRenderer.enabled);

            yield return new WaitForSeconds(0.4f);

            sphereRenderer.enabled = !(sphereRenderer.enabled);
            ciliderRenderer.enabled = !(ciliderRenderer.enabled);

            yield return new WaitForSeconds(0.4f);

            ciliderRenderer.enabled = !(ciliderRenderer.enabled);
            tvRenderer.enabled = !(tvRenderer.enabled);

            yield return new WaitForSeconds(0.4f);

            tvRenderer.enabled = !(tvRenderer.enabled);
        }
    }
}
