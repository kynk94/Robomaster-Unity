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
    private RayPerception rayPer;
    private string myTeam;
    private string enemyTeam;
    private string myShield;
    private string enemyShield;
    private string myReload;
    private string enemyReload;
    public bool manual;
    public bool useVectorObs;
    public Vector3 direction { get; private set; }
    public float rotate { get; private set; }

    private bool deadTrigger = true;

    public override void InitializeAgent()
    {
        base.InitializeAgent();
        roboState = GetComponent<RoboState>();
        roboMovement = GetComponent<RoboMovement>();
        rayPer = GetComponent<RayPerception>();
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
        if (useVectorObs)
        {
            GetRay();
        }
    }
    public void GetRay()
    {
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
        const float rayDistance = 3f;
        float[] rayAngles = { 20f, 90f, 160f, 45f, 135f, 70f, 110f };
        string[] detectableObjects = { myTeam, myShield, myReload, enemyTeam, enemyShield, enemyReload, "wall" };
        List<float> ray = rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0.2f, 0.2f);
        AddVectorObs(ray);
        for(int i=0; i<ray.Count;i++)
        {
            print("i"+i.ToString()+" "+ray[i].ToString());
        }
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
