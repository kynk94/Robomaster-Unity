﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboShooter : MonoBehaviour
{
    public GameObject bulletPrefab;

    private RoboState roboState;
    private MoveInput moveInput;
    private Transform vGimbalPivot;
    private float maxHeat = 360f;
    private float decHeat = 120f;
    public float currentHeat { get; private set; }
    private float fireRate = 0.1f; // 초당 10발 발사 가능
    private float timeAfterFire = 0f;
    private float heatTime = 0f;

    private float fireSpeed;
    public int ammoRemain { get; private set; } // 남은 전체 탄알
    private int maxAmmoCapacity = 200; // 탄창 용량

    private void OnEnable()
    {
        currentHeat = 0f;
        roboState = GetComponent<RoboState>();
        ammoRemain = roboState.ammoRemain;
        moveInput = GetComponent<MoveInput>();
        vGimbalPivot = transform.Find("Gimbal/V Gimbal Pivot");
    }

    private void FixedUpdate()
    {
        fireSpeed = roboState.fireSpeed;
        if (moveInput.manual) Fire();
        Reload();
        roboState.AmmoUpdate();
    }
    private void Fire()
    {
        timeAfterFire += Time.deltaTime;
        heatTime += Time.deltaTime;

        if (heatTime >= 1f)
        {
            heatTime = 0f;
            currentHeat -= decHeat;
            if (currentHeat < 0) currentHeat = 0f;
        }
        if (moveInput.manual)
        {
            if (moveInput.fire &&
                timeAfterFire >= fireRate &&
                currentHeat < maxHeat &&
                ammoRemain > 0)
            {
                timeAfterFire = 0f;
                GameObject firedBullet =
                    Instantiate(bulletPrefab, vGimbalPivot.position, vGimbalPivot.rotation);
                Bullet bullet = firedBullet.GetComponent<Bullet>();
                bullet.GetFiredRobot(name, gameObject);
                bullet.bulletSpeed = fireSpeed;
                firedBullet.transform.LookAt(vGimbalPivot);
                currentHeat += bullet.bulletSpeed;
                if (currentHeat > maxHeat) currentHeat = maxHeat;
                ammoRemain--;
            }
        }
        else
        {
            if (GetComponent<RoboAgent>().fire > 0.5 &&
                timeAfterFire >= fireRate &&
                currentHeat < maxHeat &&
                ammoRemain > 0)
            {
                timeAfterFire = 0f;
                GameObject firedBullet =
                    Instantiate(bulletPrefab, vGimbalPivot.position, vGimbalPivot.rotation);
                Bullet bullet = firedBullet.GetComponent<Bullet>();
                bullet.GetFiredRobot(name, gameObject);
                bullet.bulletSpeed = fireSpeed;
                firedBullet.transform.LookAt(vGimbalPivot);
                currentHeat += bullet.bulletSpeed;
                if (currentHeat > maxHeat) currentHeat = maxHeat;
                ammoRemain--;
            }
        }
    }
    private void Reload()
    {
        if (moveInput.reload)
        {
            ammoRemain += 50;
            if (ammoRemain > maxAmmoCapacity) ammoRemain = maxAmmoCapacity;
        }
    }
    public void AmmoPlus()
    {
        ammoRemain++;
        if (ammoRemain > maxAmmoCapacity) ammoRemain = maxAmmoCapacity;
    }

    public void AgentFire(Transform target)
    {
        timeAfterFire += Time.deltaTime;
        heatTime += Time.deltaTime;

        if (heatTime >= 1f)
        {
            heatTime = 0f;
            currentHeat -= decHeat;
            if (currentHeat < 0) currentHeat = 0f;
        }
        if (moveInput.fire &&
            timeAfterFire >= fireRate &&
            currentHeat < maxHeat &&
            ammoRemain > 0)
        {
            timeAfterFire = 0f;
            GameObject firedBullet =
                Instantiate(bulletPrefab, vGimbalPivot.position, vGimbalPivot.rotation);
            Bullet bullet = firedBullet.GetComponent<Bullet>();
            bullet.GetFiredRobot(name, gameObject);
            bullet.bulletSpeed = fireSpeed;
            firedBullet.transform.LookAt(target.GetComponent<Collider>().bounds.center);
            currentHeat += bullet.bulletSpeed;
            if (currentHeat > maxHeat) currentHeat = maxHeat;
            if (!GetComponent<RoboAgent>().ammoInf) ammoRemain--;
        }
        GetComponent<RoboAgent>().Attack();
    }
}
