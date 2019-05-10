using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private List<RoboState> roboStates = new List<RoboState>();    
    private int timeCount = 0;
    public float chargeTime { get; private set; }
    public int redShieldCount { get; private set; }
    public int blueShieldCount { get; private set; }
    public float redShieldOnTime { get; private set; }
    public float blueShieldOnTime { get; private set; }
    public bool redShieldOn { get; private set; }
    public bool blueShieldOn { get; private set; }

    private void OnEnable()
    {
        FindRoboStates();
        chargeTime = 0f;
        redShieldOn = false;
        blueShieldOn = false;
        redShieldCount = 2;
        blueShieldCount = 2;
    }
    public void ResetWorld()
    {
        FindRoboStates();
        chargeTime = 0f;
        redShieldOn = false;
        blueShieldOn = false;
        redShieldCount = 2;
        blueShieldCount = 2;
    }
    private void FindRoboStates()
    {
        if (transform.parent.Find("Agent Red 1") != null)
        {
            roboStates.Add(transform.parent.Find("Agent Red 1").GetComponent<RoboState>());
        }
        if (transform.parent.Find("Agent Red 2") != null)
        {
            roboStates.Add(transform.parent.Find("Agent Red 2").GetComponent<RoboState>());
        }
        if (transform.parent.Find("Agent Blue 1") != null)
        {
            roboStates.Add(transform.parent.Find("Agent Blue 1").GetComponent<RoboState>());
        }
        if (transform.parent.Find("Agent Blue 2") != null)
        {
            roboStates.Add(transform.parent.Find("Agent Blue 2").GetComponent<RoboState>());
        }
        //foreach (RoboState roboState in roboStates)
        //{
        //    print(roboState.name);
        //}
    }

    private void FixedUpdate()
    {        
        Charge();
        MapShieldOn();
        ShieldMaterialChange();
    }    

    public void ShieldUse(string team)
    {
        if (team == "redShield")
        {
            if (redShieldCount > 0)
            {
                redShieldOn = true;
                redShieldOnTime = 0f;
                redShieldCount--;
            }          
        }
        else if (team == "blueShield")
        {
            if (blueShieldCount > 0)
            {
                blueShieldOn = true;
                blueShieldOnTime = 0f;
                blueShieldCount--;
            }
        }
    }

    private void MapShieldOn()
    {
        if (redShieldOn)
        {
            redShieldOnTime += Time.deltaTime;
            if (redShieldOnTime >= 30f)
            {
                redShieldOn = false;
                redShieldOnTime = 0f;
            }
        }
        if (blueShieldOn)
        {
            blueShieldOnTime += Time.deltaTime;
            if (blueShieldOnTime >= 30f)
            {
                blueShieldOn = false;
                blueShieldOnTime = 0f;
            }
        }
    }
    private void ShieldMaterialChange()
    {
        if (redShieldCount == 0)
        {
            transform.Find("Red Zone/Red Shield").GetComponent<MeshRenderer>().material =
                (Material)Resources.Load("Red Shield Off");
        }
        if (blueShieldCount == 0)
        {
            transform.Find("Blue Zone/Blue Shield").GetComponent<MeshRenderer>().material =
                (Material)Resources.Load("Blue Shield Off");
        }
    }
    private void Charge()
    {
        chargeTime += Time.deltaTime;
        if (chargeTime >= 60f)
        {
            chargeTime = 0;
            if(timeCount == 0 || timeCount ==1)
            {
                transform.Find("Red Zone/Red Shield").GetComponent<MeshRenderer>().material =
                    (Material)Resources.Load("Red Shield");
                transform.Find("Blue Zone/Blue Shield").GetComponent<MeshRenderer>().material =
                    (Material)Resources.Load("Blue Shield");
                redShieldCount = 2;
                blueShieldCount = 2;
                timeCount++;
            }            
            foreach(RoboState roboState in roboStates)
            {
                roboState.ResetReloadCount();
            }
        }        
    }
}
