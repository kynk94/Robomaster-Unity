using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboArmor : MonoBehaviour
{
    private RoboState roboState;
    public string firedRobot { get; private set; }
    void OnEnable()
    {
        roboState = transform.parent.parent.GetComponent<RoboState>();
    }
        
    public void Attacked(string robot)
    {
        firedRobot = robot;
        roboState.ArmorAttacked(tag, firedRobot);
    }
}
