using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private RoboState[] roboStates;
    private float chargeTime = 0f;
    private int timeCount = 0;
    public int redShieldCount { get; private set; }
    public int blueShieldCount { get; private set; }
    public int redReloadCount { get; private set; }
    public int blueReloadCount { get; private set; }
    public float redShieldOnTime { get; private set; }
    public float blueShieldOnTime { get; private set; }
    public bool redShieldOn { get; private set; }
    public bool blueShieldOn { get; private set; }
    public float redReloadOnTime { get; private set; }
    public float blueReloadOnTime { get; private set; }
    public bool redReloadOn { get; private set; }
    public bool blueReloadOn { get; private set; }

    private void OnEnable()
    {
        roboStates = (RoboState[])FindObjectsOfType(typeof(RoboState));
        
        redShieldOn = false;
        blueShieldOn = false;
        redReloadOn = false;
        blueReloadOn = false;
        redShieldCount = 2;
        blueShieldCount = 2;
    }
        
    private void Update()
    {        
        Charge();
        MapShieldOn();
        //Debug.Log("RedShield : " + redShieldOn.ToString());
        //Debug.Log("BlueShield : " + blueShieldOn.ToString());
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

    public void ReloadUse(string team)
    {
        if (team == "redReload")
        {
            if (redReloadCount > 0)
            {
                redReloadOn = true;
                redReloadOnTime = 0f;
                redReloadCount--;
            }
        }
        else if (team == "blueReload")
        {
            if (blueReloadCount > 0)
            {
                blueReloadOn = true;
                blueReloadOnTime = 0f;
                blueReloadCount--;
            }
        }
    }
    private void MapReloadOn()
    {
        if (redReloadOn)
        {
            if (redReloadOnTime <= 3f)
            {
                redReloadOnTime += Time.deltaTime;
            }
            
            
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
