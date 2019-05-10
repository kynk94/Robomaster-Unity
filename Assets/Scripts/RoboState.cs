using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RoboState : MonoBehaviour
{
    private RoboAgent roboAgent;
    private Slider healthSlider;
    private GameObject roboWorld;
    private GameObject mapReload;
    private MapManager mapManager;
    private RoboShooter roboShooter;
    private RoboMovement roboMovement;
    private Transform vGimbalPivot;
    private MeshRenderer gimbalCoverRenderer;
    private float startingHealth = 2000f;
    private float collideTime = 0.3f;
    private float collideTimer = 0f;
    private bool canCollide = false;
    private float hitTime = 0.05f;
    private float hitTimer = 0f;
    private bool canHit = false;
    public int ammoRemain { get; private set; }
    public float health { get; private set; }
    public float damage { get; private set; }
    public float fireSpeed { get; private set; }
    public float currentHeat { get; private set; }
    public bool isAttacked { get; private set; }
    public int reloadCount { get; private set; }
    public int shieldCount { get; private set; }
    public int enemyShieldCount { get; private set; }
    public bool reloading { get; private set; }
    public bool isAllyReloading { get; private set; }
    public bool isShield { get; private set; }
    public bool isEnemyShield { get; private set; }    
    public float shieldOnTime { get; private set; }
    public float enemyShieldOnTime { get; private set; }
    public float chargeTime { get; private set; }
    public bool frontAttacked { get; private set; }
    public bool leftAttacked { get; private set; }
    public bool rearAttacked { get; private set; }
    public bool rightAttacked { get; private set; }
    public bool isCollide { get; private set; }
    public bool dead { get; private set; }
    public bool allyDead { get; private set; }
    public bool amIShootDead { get; private set; }
    public bool isWobble { get; private set; }
    private void OnEnable()
    {
        roboAgent = GetComponent<RoboAgent>();
        health = 2000f;
        healthSlider = transform.Find("Canvas/Slider").GetComponent<Slider>();
        healthSlider.gameObject.SetActive(true);
        healthSlider.maxValue = startingHealth;
        healthSlider.value = health;
        roboWorld = transform.parent.Find("Robo World").gameObject;
        if (tag == "redAgent") mapReload = roboWorld.transform.Find("Red Zone/Red Reload").gameObject;
        else if (tag == "blueAgent") mapReload = roboWorld.transform.Find("Blue Zone/Blue Reload").gameObject;
        mapManager = roboWorld.GetComponent<MapManager>();
        roboMovement = GetComponent<RoboMovement>();
        roboShooter = GetComponent<RoboShooter>();
        gimbalCoverRenderer = transform.Find("Gimbal/Gimbal Head/Gimbal Cover").GetComponent<MeshRenderer>();
        ammoRemain = 40;
        reloadCount = 2;
        shieldCount = 2;
        enemyShieldCount = 2;
        currentHeat = 0f;
        reloading = false;
        isAttacked = false;
        isShield = false;
        isEnemyShield = false;
        frontAttacked = false;
        leftAttacked = false;
        rearAttacked = false;
        rightAttacked = false;
        isCollide = false;
        dead = false;
        amIShootDead = false;
        isWobble = false;
        chargeTime = 0f;
        damage = 50f;
        fireSpeed = 15f;
    }
    public void ResetState()
    {
        GetComponent<MoveInput>().enabled = true;
        GetComponent<RoboMovement>().enabled = true;
        GetComponent<RoboShooter>().enabled = true;
        healthSlider.gameObject.SetActive(true);
        health = 2000f;
        healthSlider.value = health;
        ammoRemain = 40;
        reloadCount = 2;
        shieldCount = 2;
        enemyShieldCount = 2;
        currentHeat = 0f;
        reloading = false;
        isAttacked = false;
        isShield = false;
        isEnemyShield = false;
        frontAttacked = false;
        leftAttacked = false;
        rearAttacked = false;
        rightAttacked = false;
        isCollide = false;
        dead = false;
        amIShootDead = false;
        isWobble = false;
        chargeTime = 0f;
        damage = 50f;
        fireSpeed = 15f;
    }
    private void Update()
    {
        ShieldUpdate();
        IsAllyReloading();
        AllyDeadCheck();
        collideTimer += Time.deltaTime;
        hitTimer += Time.deltaTime;
        if (collideTimer > collideTime)
        {
            canCollide = true;
            isCollide = false;
        }
        if (hitTimer > hitTime)
        {
            canHit = true;
            isAttacked = false;
            frontAttacked = false;
            leftAttacked = false;
            rearAttacked = false;
            rightAttacked = false;
        }
        if (dead) Dead();
        currentHeat = roboShooter.currentHeat;
        healthSlider.value = health;
        isWobble = roboMovement.realWobble;
    }
    private void AllyDeadCheck()
    {
        if (name == "Agent Red 1") allyDead =
                transform.parent.Find("Agent Red 2").GetComponent<RoboState>().dead;
        else if (name == "Agent Red 2") allyDead =
                transform.parent.Find("Agent Red 1").GetComponent<RoboState>().dead;
        else if (name == "Agent Blue 1") allyDead =
                transform.parent.Find("Agent Blue 2").GetComponent<RoboState>().dead;
        else if (name == "Agent Blue 2") allyDead =
                transform.parent.Find("Agent Blue 1").GetComponent<RoboState>().dead;
    }
    public void ShootDead(bool deadrobot)
    {
        amIShootDead = deadrobot;
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
        if (tag == "redAgent")
        {
            shieldCount = mapManager.redShieldCount;
            enemyShieldCount = mapManager.blueShieldCount;
            shieldOnTime = mapManager.redShieldOnTime;
            enemyShieldOnTime = mapManager.blueShieldOnTime;
            isShield = mapManager.redShieldOn;
            isEnemyShield = mapManager.blueShieldOn;
        }
        else if (tag == "blueAgent")
        {
            shieldCount = mapManager.blueShieldCount;
            enemyShieldCount = mapManager.redShieldCount;
            shieldOnTime = mapManager.blueShieldOnTime;
            enemyShieldOnTime = mapManager.redShieldOnTime;
            isShield = mapManager.blueShieldOn;
            isEnemyShield = mapManager.redShieldOn;
        }
        chargeTime = mapManager.chargeTime;
        DepenseBuff();
    }
    public void ArmorAttacked(string armor, string firedRobot)
    {
        Debug.Log(health.ToString() + "\t" + armor);
        if (canHit)
        {
            if (armor == "Front Armor")
            {
                frontAttacked = true;
                isAttacked = true;
            }
            else if (armor == "Left Armor")
            {
                leftAttacked = true;
                isAttacked = true;
            }
            else if (armor == "Rear Armor")
            {
                rearAttacked = true;
                isAttacked = true;
            }
            else if (armor == "Right Armor")
            {
                rightAttacked = true;
                isAttacked = true;
            }
            health -= damage;
            if (health <= 0)
            {
                health = 0;
                dead = true;
            }
            roboAgent.Attacked();
            canHit = false;
        }
    }
    
    public void Dead()
    {
        GetComponent<MoveInput>().enabled = false;
        GetComponent<RoboMovement>().enabled = false;
        GetComponent<RoboShooter>().enabled = false;
        healthSlider.gameObject.SetActive(false);        
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        GameObject collideObject = collision.collider.gameObject;
        if (collideObject.CompareTag("wall") || collideObject.tag.Contains("Agent") || collideObject.tag.Contains("Armor"))
        {
            if (canCollide)
            {
                roboAgent.GetCollide();
                isCollide = true;
                canCollide = false;
                collideTimer = 0f;
                health -= 10f;
            }
            if (health <= 0)
            {
                health = 0;
                dead = true;
            }
        }
    }
}