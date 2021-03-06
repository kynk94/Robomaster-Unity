﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public string restartButtonName = "Restart";
    public bool restart { get; private set; }
    private GameObject roboWorld;
    private List<GameObject> agents = new List<GameObject>();
    private List<Vector3> firstPosition = new List<Vector3>();
    private List<Vector3> firstRotation = new List<Vector3>();
    private List<Vector3> firstGimbalRotation = new List<Vector3>();
    private void OnEnable()
    {
        roboWorld = transform.Find("Robo World").gameObject;
        FindAgents(); 
    }
    private void FindAgents()
    {
        if(transform.Find("Agent Red 1") != null)
        {
            agents.Add(transform.Find("Agent Red 1").gameObject);
        }
        if (transform.Find("Agent Red 2") != null)
        {
            agents.Add(transform.Find("Agent Red 2").gameObject);
        }
        if (transform.Find("Agent Blue 1") != null)
        {
            agents.Add(transform.Find("Agent Blue 1").gameObject);
        }
        if (transform.Find("Agent Blue 2") != null)
        {
            agents.Add(transform.Find("Agent Blue 2").gameObject);
        }
        foreach (GameObject agent in agents)
        {
            firstPosition.Add(agent.transform.position);
            firstRotation.Add(agent.transform.eulerAngles);
            firstGimbalRotation.Add(agent.transform.Find("Gimbal").eulerAngles);
        }
    }
    private void Update()
    {
        restart = Input.GetButtonDown(restartButtonName);
        Restart(restart);
        if (agents[0].GetComponent<RoboState>().dead && agents[1].GetComponent<RoboState>().dead) { Restart(true); }
        if (agents[2].GetComponent<RoboState>().dead && agents[3].GetComponent<RoboState>().dead) { Restart(true); }
    }
    public void Restart(bool reset)
    {
        if (reset)
        {
            for (int idx = 0; idx < agents.Count; idx++)
            {
                agents[idx].GetComponent<RoboState>().ResetState();
                agents[idx].transform.position = firstPosition[idx];
                agents[idx].transform.rotation = Quaternion.Euler(firstRotation[idx]);
                agents[idx].transform.Find("Gimbal").rotation = Quaternion.Euler(firstGimbalRotation[idx]);
                agents[idx].GetComponent<RoboAgent>().Done();
                //RoboState agentState = agents[idx].GetComponent<RoboState>();
            }
            roboWorld.GetComponent<MapManager>().ResetWorld();
        }
    }
}
