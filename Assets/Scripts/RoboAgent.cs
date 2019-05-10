using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using MLAgents;

public class RoboAgent : Agent
{
    private RoboState roboState;
    private RoboMovement roboMovement;
    private RoboShooter roboShooter;
    private RoboRayPerception rayPer;
    private string myTeam;
    private string enemyTeam;
    private string myShield;
    private string enemyShield;
    private string myReload;
    private string enemyReload;
    private float enemyTriggerTime;
    public bool manual;
    public bool useRayPerception;
    public GameObject rayObject { get; private set; }
    public bool enemyDetect { get; private set; }
    public Vector3 direction { get; private set; }
    public float rotate { get; private set; }
    public float fire { get; private set; }
    public float agentWobble { get; private set; }

    private bool deadTrigger = true;

    public bool ammoInf { get; private set; }

    public override void InitializeAgent()
    {
        ammoInf = true;
        base.InitializeAgent();
        roboState = GetComponent<RoboState>();
        roboMovement = GetComponent<RoboMovement>();
        roboShooter = GetComponent<RoboShooter>();
        rayPer = GetComponent<RoboRayPerception>();
        if (gameObject.CompareTag("redAgent"))
        {
            myTeam = tag;
            myShield = "redShield";
            myReload = "redReload";
            enemyTeam = "blueAgent";
            enemyShield = "blueShield";
            enemyReload = "blueReload";
        }
        else if (gameObject.CompareTag("blueAgent"))
        {
            myTeam = tag;
            myShield = "blueShield";
            myReload = "blueReload";
            enemyTeam = "redAgent";
            enemyShield = "redShield";
            enemyReload = "redReload";
        }
    }

    private void Update()
    {
        enemyTriggerTime += Time.deltaTime;
        if (enemyTriggerTime >= 1f)
        {
            enemyTriggerTime = 0f;
            enemyDetect = false;
        }
    }

    public override void CollectObservations()
    {        
        AddVectorObs(transform.position.x / 4f); // -1~1
        AddVectorObs(transform.position.z / 2f); // -1~1
        AddVectorObs(transform.Find("Fixed Pivot").transform.eulerAngles.y / 360f); // 0~1
        AddVectorObs(roboState.health / 2000f); // 0~1
        AddVectorObs(roboState.currentHeat / 360f); // 0~1
        if (!ammoInf) AddVectorObs(roboState.ammoRemain / 200); // 0~1
        AddVectorObs(roboState.shieldOnTime / 30f); // 0~1
        AddVectorObs(roboState.chargeTime / 60f); // 0~1
        AddVectorObs(roboState.reloadCount); // 0,1,2
        AddVectorObs(roboState.shieldCount); // 0,1,2 // 10개
        //bool
        AddVectorObs(roboState.dead);
        AddVectorObs(roboState.allyDead);
        AddVectorObs(roboState.reloading);
        AddVectorObs(roboState.isAllyReloading);
        AddVectorObs(roboState.isShield);
        AddVectorObs(roboState.amIShootDead);
        AddVectorObs(roboState.isWobble);
        AddVectorObs(roboState.isAttacked);
        AddVectorObs(roboState.frontAttacked);
        AddVectorObs(roboState.leftAttacked); // 20개
        AddVectorObs(roboState.rearAttacked);
        AddVectorObs(roboState.rightAttacked);
        AddVectorObs(roboState.isCollide); // 23개
        if (useRayPerception)
        {
            GetRay();
        }
    }
    public void GetRay()
    {
        const float rayDistance = 4f;
        float[] rayOnAngles = { -20f, 0f, 20f, 45f, 70f, 90f, 110f, 135f, 160f, 180f, 200f };
        float[] rayUnderAngles = { 0f, 30f, 60f, 90f, 120f, 150f, 180f, 210f, 240f, 270f, 300f, 330f };
        string[] detectableOnFloor = { myTeam, enemyTeam, "wall" };
        string[] detectableUnderFloor = { myShield, myReload, enemyShield, enemyReload };
        List<float> rayOnFloor = rayPer.RoboPerceive(rayDistance, rayOnAngles, detectableOnFloor, 0.1f, 0f);        
        for (int i = 0; i < rayOnFloor.Count; i += 5)
        {
            //rayOnFloor[i]=myTeam, rayOnFloor[i+1]=enemyTeam, rayOnFloor[i+2]=wall
            //print(rayOnAngles[(i) / 5].ToString() + "\t" + rayOnFloor[i + 1].ToString());
            if (rayOnFloor[i + 1] == 1)
            {
                //print(rayPer.GetHitObject().transform.position);
                enemyDetect = true;
                EnemyDetect();
                rayObject = rayPer.GetHitObject()[i+1];                
            }
        }
        AddVectorObs(rayOnFloor);
        List<float> rayUnderFloor = rayPer.Perceive(rayDistance, rayUnderAngles, detectableUnderFloor, -1f, 0f);        
        AddVectorObs(rayUnderFloor);
    }
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        if (!roboState.dead)
        {
            HealthReward();
        }
        direction = new Vector3(vectorAction[0], 0, vectorAction[1]);
        rotate = vectorAction[2];
        fire = vectorAction[3];
        agentWobble = vectorAction[4];
        if (roboState.dead && deadTrigger && !roboState.allyDead) AgentDead();
        if (roboState.dead && deadTrigger && roboState.allyDead) AgentDead();
    }
    public void GetCollide()
    {
        AddReward(-1f);        
    }
    public void EnemyDetect()
    {
        AddReward(1f);
    }
    public void Attacked()
    {
        AddReward(-1f);
    }
    public void AgentDead()
    {
        deadTrigger = false;
        AddReward(-10f);
    }
    public void Attack()
    {
        AddReward(1f);
    }
    public void OnEnemyReloadzone()
    {
        AddReward(-30f);
    }
    public void OnMyShieldzone()
    {
        AddReward(1f);
    }
    public void OnEnemyShieldzone()
    {
        AddReward(-1f);
    }
    public void TriggerMyShield()
    {
        AddReward(3f);
    }
    public void TriggerEnemyShield()
    {
        AddReward(-30f);
    }
    public void HealthReward()
    {
        AddReward(roboState.health / 2000f);
    }
    public override void AgentReset()
    {        
        //transform.parent.GetComponent<StageManager>().Restart(true);
    }
    public override void AgentOnDone()
    {        
    }
}
