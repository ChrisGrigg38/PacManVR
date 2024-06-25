using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    public string tagName;
    public string name;
    public GameObject prefab;


    private GameObject cachedObject;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (cachedObject == null || !cachedObject.activeInHierarchy)
        {
            GameObject newObj = GameObject.Instantiate(prefab, transform.position, transform.rotation);
            newObj.name = name;
            cachedObject = newObj;
        }
    }
}
