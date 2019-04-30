using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapReload : MonoBehaviour
{
    private MapManager mapManager;
    private new Collider collider;
    private string agentName;
    private float reloadTriggerTime = 0f;
    void OnEnable()
    {
        mapManager = transform.parent.parent.GetComponent<MapManager>();
        collider = GetComponent<Collider>();
    }

    void Update()
    {
        
    }

    private void ReloadOn()
    {
        if (reloadTriggerTime >= 0.5f)
        {
            reloadTriggerTime = 0f;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "shieldSensor")
        {
            agentName = other.transform.parent.name;
            reloadTriggerTime += Time.deltaTime;       
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "shieldSensor")
        {
            reloadTriggerTime = 0f;
        }
    }
}
