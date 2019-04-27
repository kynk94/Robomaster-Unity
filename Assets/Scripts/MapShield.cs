using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapShield : MonoBehaviour
{
    private MapManager mapManager;
    private new Collider collider;
    private float shieldTriggerTime = 0f;

    void OnEnable()
    {
        mapManager = transform.parent.parent.GetComponent<MapManager>();        
        collider = GetComponent<Collider>();
    }

    void Update()
    {
        ShieldOn();
    }

    private void ShieldOn()
    {
        if (shieldTriggerTime >= 5f)
        {
            shieldTriggerTime = 0f;
            // MapManager에서 해당 팀 실드 발동
            mapManager.ShieldUse(gameObject.tag);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag=="shieldSensor")
        {
            Vector3 min = collider.bounds.min + other.bounds.extents;
            Vector3 max = collider.bounds.max - other.bounds.extents;
            // collider 안에 완벽히 들어왔을 경우에만 체크한다.
            // 밖으로 나가면 쉴드 발동 시간을 초기화
            if (min.x <= other.bounds.center.x && other.bounds.center.x <= max.x &&
                min.z <= other.bounds.center.z && other.bounds.center.z <= max.z) 
            {
                shieldTriggerTime += Time.deltaTime;                
            }
            else
            {
                shieldTriggerTime = 0f;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "shieldSensor")
        {
            shieldTriggerTime = 0f;
        }
    }
}
