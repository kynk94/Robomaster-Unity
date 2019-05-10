using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapReload : MonoBehaviour
{
    private GameObject agentObject;
    private MapManager mapManager;
    private Collider[] colliders;
    private Collider sensorCollider;
    private Collider bucketCollider;
    private bool sensorOn;
    public bool countTrigger { get; private set; }
    private float reloadTriggerTime = 0f;
    private float rewardTriggerTime = 0f;
    private bool rewardTrigger=true;
    private int plusAmmo = 0;
    private string opponentAgent;
    public Vector3 center { get; private set; }

    private void OnEnable()
    {
        mapManager = transform.parent.parent.GetComponent<MapManager>();
        colliders = GetComponents<Collider>();
        foreach(Collider myCollider in colliders)
        {
            if (myCollider.bounds.size.x > 0.5) sensorCollider = myCollider;
            else bucketCollider = myCollider;
        }
        center = sensorCollider.bounds.center;
        sensorOn = false;
        countTrigger = false;
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
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.parent.GetComponent<RoboState>().dead) return;
        if (other.transform.tag == opponentAgent &&
            rewardTrigger)
        {
            other.transform.parent.GetComponent<RoboAgent>().OnEnemyReloadzone();
            return;
        }
        if (other.tag == "shieldSensor")
        {
            agentObject = other.transform.parent.gameObject;
            Vector3 min = sensorCollider.bounds.min + other.bounds.extents;
            Vector3 max = sensorCollider.bounds.max - other.bounds.extents;
            // collider 안에 완벽히 들어왔을 경우에만 체크한다.
            // 밖으로 나가면 리로드 발동 시간을 초기화
            if (min.x <= other.bounds.center.x && other.bounds.center.x <= max.x &&
                min.z <= other.bounds.center.z && other.bounds.center.z <= max.z)
            {
                reloadTriggerTime += Time.deltaTime;
                rewardTriggerTime += Time.deltaTime;
                if (reloadTriggerTime > 0.5f && countTrigger == false &&
                    agentObject.GetComponent<RoboState>().reloadCount>0)
                {
                    agentObject.GetComponent<RoboState>().UseReloadCount();
                    countTrigger = true;
                }
                sensorOn = true;
            }
            else
            {
                agentObject.GetComponent<RoboState>().ReloadingToFalse();
                reloadTriggerTime = 0f;
                rewardTriggerTime = 0f;
                countTrigger = false;
                sensorOn = false;
            }
        }
        if (other.tag == "bulletBucket" && countTrigger == true)
        {
            if (0.5f < reloadTriggerTime && reloadTriggerTime < 3.5f)
            {
                // 3초간 50발 공급, 1발당 0.06초 소요
                int truncateAmmo = (int)System.Math.Truncate((reloadTriggerTime-0.5f) / 0.06f);
                if (truncateAmmo - plusAmmo == 1)
                {
                    agentObject.GetComponent<RoboShooter>().AmmoPlus();
                }
                plusAmmo = truncateAmmo;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "shieldSensor")
        {
            agentObject.GetComponent<RoboState>().ReloadingToFalse();
            reloadTriggerTime = 0f;
            countTrigger = false;
            sensorOn = false;
            plusAmmo = 0;
        }
    }
}
