using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboState : MonoBehaviour
{
    private GameObject roboWorld;
    private GameObject mapReload;
    private MapManager mapManager;
    private RoboShooter roboShooter;
    private Transform vGimbalPivot;
    private MeshRenderer gimbalCoverRenderer;
    private string myTag;
    public int ammoRemain { get; private set; }
    public float health { get; private set; }
    public float damage { get; private set; }
    public float fireSpeed { get; private set; }
    public bool isAttacked { get; private set; }
    public int reloadCount { get; private set; }
    public int shieldCount { get; private set; }
    public int enemyShieldCount { get; private set; }
    public bool reloading { get; private set; }
    public bool isAllyReloading { get; private set; }
    public bool isShield { get; private set; }
    public bool isEnemyShield { get; private set; }    
    public bool redShieldOn { get; private set; }
    public bool blueShieldOn { get; private set; }
    public float redShieldOnTime { get; private set; }
    public float blueShieldOnTime { get; private set; }
    public float chargeTime { get; private set; }

    private void OnEnable()
    {
        myTag = transform.tag;
        roboWorld = transform.parent.Find("Robo World").gameObject;
        if (transform.tag == "redAgent") mapReload = roboWorld.transform.Find("Red Zone/Red Reload").gameObject;
        else if (transform.tag == "blueAgent") mapReload = roboWorld.transform.Find("Blue Zone/Blue Reload").gameObject;
        mapManager = roboWorld.GetComponent<MapManager>();
        gimbalCoverRenderer = transform.Find("Gimbal/Gimbal Head/Gimbal Cover").GetComponent<MeshRenderer>();
        ammoRemain = 40;
        reloadCount = 2;
        shieldCount = 2;
        enemyShieldCount = 2;
        reloading = false;
        isAttacked = false;
        isShield = false;
        isEnemyShield = false;
        redShieldOn = false;
        blueShieldOn = false;
        chargeTime = 0f;
        health = 2000f;
        damage = 50f;
        fireSpeed = 15f;
    }

    private void Update()
    {
        ShieldUpdate();
        IsAllyReloading();
    }
    public void ResetReloadCount()
    {
        reloadCount = 2;
    }
    public void UseReloadCount()
    {
        reloading = true;
        reloadCount--;
    }
    public void ReloadingToFalse()
    {
        reloading = false;
    }
    private void IsAllyReloading()
    {
        if (!reloading) isAllyReloading = mapReload.GetComponent<MapReload>().countTrigger;
    }
    public void AmmoUpdate()
    {
        ammoRemain = GetComponent<RoboShooter>().ammoRemain;
    }
    
    private void DepenseBuff()
    {
        if (isShield)
        {
            damage = 25f;
            gimbalCoverRenderer.material = (Material)Resources.Load("Shield On");
        }
        else
        {
            damage = 50f;
            gimbalCoverRenderer.material = (Material)Resources.Load("Robot Base");
        }
    }
    private void ShieldUpdate()
    {
        if (myTag == "redAgent")
        {
            shieldCount = mapManager.redShieldCount;
            enemyShieldCount = mapManager.blueShieldCount;
        }
        else if (myTag == "blueAgent")
        {
            shieldCount = mapManager.blueShieldCount;
            enemyShieldCount = mapManager.redShieldCount;
        }
        redShieldOn = mapManager.redShieldOn;
        blueShieldOn = mapManager.blueShieldOn;
        redShieldOnTime = mapManager.redShieldOnTime;
        blueShieldOnTime = mapManager.blueShieldOnTime;
        chargeTime = mapManager.chargeTime;
        if (transform.tag == "redAgent")
        {
            isShield = redShieldOn;
            isEnemyShield = blueShieldOn;
        }
        else if (transform.tag == "blueAgent")
        {
            isShield = blueShieldOn;
            isEnemyShield = redShieldOn;
        }
        DepenseBuff();
    }
    
    private void OnTriggerEnter(Collider other)
    {

    }
}