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
    private RoboRayPerception rayPer;
    private string myTeam;
    private string enemyTeam;
    private string myShield;
    private string enemyShield;
    private string myReload;
    private string enemyReload;
    public bool manual;
    public bool useRayPerception;
    public Vector3 direction { get; private set; }
    public float rotate { get; private set; }

    private bool deadTrigger = true;

    public override void InitializeAgent()
    {
        base.InitializeAgent();
        roboState = GetComponent<RoboState>();
        roboMovement = GetComponent<RoboMovement>();
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

    public override void CollectObservations()
    {
        AddVectorObs(transform.position.x / 4f); // -1~1
        AddVectorObs(transform.position.z / 2f); // -1~1
        AddVectorObs(transform.eulerAngles.y / 360f); // 0~1
        AddVectorObs(roboState.health / 2000f); // 0~1
        AddVectorObs(roboState.currentHeat / 360f); // 0~1
        AddVectorObs(roboState.ammoRemain / 200); // 0~1
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
        //float[] rayAngles = { 20f, 45f, 70f, 90f, 110f, 135f, 160f };
        float[] rayOnAngles = { 90f };
        float[] rayUnderAngles = { 20f, 45f, 70f, 90f, 110f, 135f, 160f };
        string[] detectableOnFloor = { myTeam, enemyTeam, "wall" };
        string[] detectableUnderFloor = { myShield, myReload, enemyShield, enemyReload };
        List<float> rayOnFloor = rayPer.Perceive(rayDistance, rayOnAngles, detectableOnFloor, 0.1f, 0.1f);
        AddVectorObs(rayOnFloor);
        List<float> rayUnderFloor = rayPer.Perceive(rayDistance, rayUnderAngles, detectableUnderFloor, -0.3f, -0.3f);        
        AddVectorObs(rayUnderFloor);
    }
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(-1f / agentParameters.maxStep);
        direction = new Vector3(vectorAction[0], 0, vectorAction[1]);
        rotate = vectorAction[2];
        if (roboState.dead && deadTrigger) AgentDead();
        if (roboState.dead && roboState.allyDead) Done();
    }
    public void GetCollide()
    {
        AddReward(-1f);        
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
    public override void AgentReset()
    {
        transform.parent.GetComponent<StageManager>().Restart(true);
    }
    public void OnCollisionEnter(Collision collision)
    {
    }
    public override void AgentOnDone()
    {        
    }
}
