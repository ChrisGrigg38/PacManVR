using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ghost : MonoBehaviour
{
    public GameObject normalGhost;
    public GameObject scaredGhost;
    private GameObject VRRig;
    public float locationUpdateTime = 5;

    //globals
    private NavMeshAgent navMeshAgent;
    private float updateTime;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        updateTime = locationUpdateTime;
        VRRig = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        updateTime -= Time.deltaTime;

        if (updateTime <= 0)
        {
            if (VRRig != null)
            {
                navMeshAgent.SetDestination(VRRig.transform.position);
            }
            else
            {
                VRRig = GameObject.FindGameObjectWithTag("Player");
            }

            updateTime = locationUpdateTime;
        }
    }
}
