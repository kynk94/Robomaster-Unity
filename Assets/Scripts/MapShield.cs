using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapShield : MonoBehaviour
{
    private MapManager mapManager;
    private Collider sensorCollider;
    private float shieldTriggerTime = 0f;
    private float rewardTriggerTime = 0f;
    private bool rewardTrigger = true;
    private string opponentAgent;    

    void OnEnable()
    {
        mapManager = transform.parent.parent.GetComponent<MapManager>();        
        sensorCollider = GetComponent<Collider>();
        if (transform.tag == "redReload") opponentAgent = "blueAgent";
        else if (transform.tag == "blueReload") opponentAgent = "redAgent";
    }
    private void FixedUpdate()
    {
        if (rewardTriggerTime > 0.1f)
        {
            rewardTriggerTime = 0f;
            rewardTrigger = true;
        }
    }

    private void ShieldOn(Collider other)
    {
        if (shieldTriggerTime >= 5f)
        {
            shieldTriggerTime = 0f;
            if (other.transform.tag == opponentAgent) other.transform.GetComponent<RoboAgent>().TriggerEnemyShield();
            else other.transform.GetComponent<RoboAgent>().TriggerMyShield();
            // MapManager에서 해당 팀 실드 발동
            mapManager.ShieldUse(gameObject.tag);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.parent.GetComponent<RoboState>().dead) return;
        if (rewardTrigger)
        {
            rewardTriggerTime = 0f;
            if (other.transform.tag == opponentAgent) other.transform.GetComponent<RoboAgent>().OnEnemyShieldzone();
            else other.transform.GetComponent<RoboAgent>().OnMyShieldzone();
        }
        if (other.tag=="shieldSensor")
        {
            Vector3 min = sensorCollider.bounds.min + other.bounds.extents;
            Vector3 max = sensorCollider.bounds.max - other.bounds.extents;
            // collider 안에 완벽히 들어왔을 경우에만 체크한다.
            // 밖으로 나가면 쉴드 발동 시간을 초기화
            if (min.x <= other.bounds.center.x && other.bounds.center.x <= max.x &&
                min.z <= other.bounds.center.z && other.bounds.center.z <= max.z) 
            {
                shieldTriggerTime += Time.deltaTime;
                rewardTriggerTime += Time.deltaTime;
            }
            else
            {
                shieldTriggerTime = 0f;
                rewardTriggerTime = 0f;
            }
        }
        ShieldOn(other);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "shieldSensor")
        {
            shieldTriggerTime = 0f;
        }
    }
}
