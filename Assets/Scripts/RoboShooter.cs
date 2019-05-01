using System.Collections;
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
    private float currentHeat = 0f;
    private float fireRate = 0.1f; // 초당 10발 발사 가능
    private float timeAfterFire = 0f;
    private float heatTime = 0f;

    private float fireSpeed;
    public int ammoRemain { get; private set; } // 남은 전체 탄알
    private int maxAmmoCapacity = 200; // 탄창 용량

    private void Start()
    {
        roboState = GetComponent<RoboState>();
        ammoRemain = roboState.ammoRemain;
        moveInput = GetComponent<MoveInput>();
        vGimbalPivot = transform.Find("Gimbal/V Gimbal Pivot").transform;
    }

    private void Update()
    {
        fireSpeed = roboState.fireSpeed;
        Fire();
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

        if (moveInput.fire &&
            timeAfterFire >= fireRate &&
            currentHeat < maxHeat &&
            ammoRemain > 0) 
        {
            timeAfterFire = 0f;
            GameObject firedBullet =
                Instantiate(bulletPrefab, vGimbalPivot.position, vGimbalPivot.rotation);
            Bullet bullet = firedBullet.GetComponent<Bullet>();
            bullet.bulletSpeed = fireSpeed;
            firedBullet.transform.LookAt(vGimbalPivot);
            currentHeat += bullet.bulletSpeed;
            if (currentHeat > maxHeat) currentHeat = maxHeat;
            ammoRemain--;
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
}
