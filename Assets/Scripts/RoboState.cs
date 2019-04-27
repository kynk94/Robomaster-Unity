using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboState : MonoBehaviour
{
    private MapManager mapManager;
    private RoboShooter roboShooter;
    private Transform vGimbalPivot;

    public float health { get; private set; }
    public float damage { get; private set; }
    public float fireSpeed { get; private set; }
    public bool isAttacked { get; private set; }
    public int reloadCount { get; private set; }
    public bool reloading { get; private set; }
    public bool isAllyReloading { get; private set; }
    public bool isShield { get; private set; }
    public bool isEnemyShield { get; private set; }
    public bool redShieldOn { get; private set; }
    public bool blueShieldOn { get; private set; }
    public float redShieldOnTime { get; private set; }
    public float blueShieldOnTime { get; private set; }
    private void OnEnable()
    {
        mapManager = transform.parent.Find("Robo World").GetComponent<MapManager>();
        reloadCount = 2;
        reloading = false;
        isAttacked = false;
        isShield = false;
        isEnemyShield = false;
        redShieldOn = false;
        blueShieldOn = false;
        health = 2000f;
        damage = 50f;
        fireSpeed = 15f;
    }

    private void Update()
    {
        ShieldUpdate();
    }
    public void ResetReloadCount()
    {
        reloadCount = 2;
    }
    private void DepenseBuff()
    {
        if (isShield) damage = 25f;
        else damage = 50f;
    }
    private void ShieldUpdate()
    {
        redShieldOn = mapManager.redShieldOn;
        blueShieldOn = mapManager.blueShieldOn;
        redShieldOnTime = mapManager.redShieldOnTime;
        blueShieldOnTime = mapManager.blueShieldOnTime;
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