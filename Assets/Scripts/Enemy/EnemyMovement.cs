using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]

public class NewBehaviourScript : MonoBehaviour
{
    public Transform player;
    public float updateSpeed = 0.1f; //how long until new calculation of new path

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FollowPlayer());
    }

    private IEnumerator FollowPlayer()
    {
        WaitForSeconds wait = new WaitForSeconds(updateSpeed);

        while (enabled)
        {
            agent.SetDestination(player.transform.position);

            yield return wait;
        }
    }
}
