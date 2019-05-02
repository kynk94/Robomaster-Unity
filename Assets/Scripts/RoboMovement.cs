using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class RoboMovement : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float rotateSpeed = 240f;

    private Vector3 firstPosition;
    private Vector3 firstRotation;
    private Vector3 moveDistance;
    private float gimbalRotateSpeed = 1.5f;
    private float hMax = 90f;
    private float vMaxU = 20f;
    private float vMaxD = 25f;
    private Transform fixedPivot;
    private Transform hGimbalPivot;
    private Transform vGimbalPivot;
    private MoveInput moveInput;
    private GameObject gimbalObject;
    private GameObject gimbalHeadObject;
    private Rigidbody roboRigidbody;
    private Rigidbody gimbalRigidbody;
    private Rigidbody gimbalHeadRigidbody;
    private NavMeshAgent pathFinder;

    private bool doesWobble = true;
    private bool wobbleDir = true;
    public bool realWobble { get; private set; }
    private float wobble = 0f;
    private float wobbleMax = 40f;
    private float wSign;
    private float wobbleSpeed = 180f;

    private void OnEnable()
    {
        if (moveInput.manual)
        {
            moveSpeed = 4f;
            rotateSpeed = 240f;
        }
        else
        {
            moveSpeed = 2f;
            rotateSpeed = 240f;
        }
        firstPosition = transform.position;
        firstRotation = transform.eulerAngles;

        moveInput = GetComponent<MoveInput>();
        
        gimbalObject = transform.Find("Gimbal").gameObject;
        gimbalHeadObject = gimbalObject.transform.Find("Gimbal Head").gameObject;

        fixedPivot = transform.Find("Fixed Pivot").transform;
        hGimbalPivot = transform.Find("H Gimbal Pivot").transform;
        vGimbalPivot = gimbalObject.transform.Find("V Gimbal Pivot").transform;

        roboRigidbody = GetComponent<Rigidbody>();
        gimbalRigidbody = gimbalObject.GetComponent<Rigidbody>();
        gimbalHeadRigidbody = gimbalHeadObject.GetComponent<Rigidbody>();
        pathFinder = GetComponent<NavMeshAgent>();
        pathFinder.enabled = false;
        pathFinder.speed = moveSpeed;
        pathFinder.angularSpeed = rotateSpeed;
    }

    private void FixedUpdate()
    {
        Rotate();
        GimbalRotate();
        realWobble = doesWobble && moveInput.wobbleInput;
        // Wobble은 조건을 따진다.
        if (realWobble) Wobble(wobbleSpeed * Time.deltaTime);
        else
        {
            wobble = 0;
            fixedPivot.rotation = Quaternion.Euler(transform.eulerAngles);
        }
        Move();
        //NavReload();
        //GoReload();
    }
    
    public void Wobble(float wSpeed)
    {
        if (wobbleDir)
        {
            wobble += wSpeed;
            wSign = 1f;
            if (wobble >= wobbleMax)
            {
                wobble = wobbleMax;
                wobbleDir = false;
            }
        }
        else
        {
            wobble -= wSpeed;
            wSign = -1f;
            if (wobble <= -wobbleMax)
            {
                wobble = -wobbleMax;
                wobbleDir = true;
            }
        }
        roboRigidbody.rotation *= Quaternion.Euler(0f, wSign * wSpeed, 0f);
        fixedPivot.rotation *= Quaternion.Euler(0f, -wSign * wSpeed, 0f);
        // 첫번째 방법
        //gimbalRigidbody.transform.rotation *= Quaternion.Euler(0f, -wSign * wSpeed, 0f);
        // 두번째 방법
        gimbalRigidbody.transform.RotateAround(hGimbalPivot.position, hGimbalPivot.up, -wSign * wSpeed);
        hGimbalPivot.rotation *= Quaternion.Euler(0f, -wSign * wSpeed, 0f);        
    }

    public void Rotate()
    {
        float velocity = moveDistance.magnitude;
        float realRotate = rotateSpeed - 800f * velocity;
        float turn = moveInput.rotate * realRotate;
        wobbleSpeed = realRotate - Mathf.Abs(turn);        
        roboRigidbody.rotation *= Quaternion.Euler(0f, turn * Time.deltaTime, 0f);
    }

    public void Move()
    {
        // 로봇의 현재 방향에 대해서 움직임
        moveDistance = Vector3.zero;
        moveDistance += fixedPivot.right * moveInput.direction.x;
        moveDistance += fixedPivot.forward * moveInput.direction.z;
        moveDistance *= moveSpeed * Time.deltaTime;
        roboRigidbody.MovePosition(roboRigidbody.position + moveDistance);
    }
    
    public void GimbalRotate()
    {
        float hturn = moveInput.hGimbalRotate * gimbalRotateSpeed * Time.deltaTime;
        float vturn = moveInput.vGimbalRotate * gimbalRotateSpeed * Time.deltaTime;
        float hgim = hGimbalPivot.localEulerAngles.y;
        float vgim = vGimbalPivot.localEulerAngles.x;
        if (hgim > 180f) hgim -= 360f;
        if (vgim > 180f) vgim -= 360f;
        if(hgim + hturn >= hMax)
        {
            hturn = hMax - hgim;
        }
        else if(hgim + hturn <= -hMax)
        {
            hturn = -hMax - hgim;
        }
        if(vgim + vturn <= -vMaxU)
        {
            vturn = -vMaxU - vgim;
        }
        else if (vgim + vturn >= vMaxD)
        {
            vturn = vMaxD - vgim;
        }
        gimbalRigidbody.transform.RotateAround(hGimbalPivot.position, hGimbalPivot.up, hturn);
        hGimbalPivot.rotation *= Quaternion.Euler(0f, hturn, 0f);
        gimbalHeadRigidbody.transform.RotateAround(vGimbalPivot.position, vGimbalPivot.right, vturn);
        vGimbalPivot.rotation *= Quaternion.Euler(vturn, 0f, 0f);
    }
    public void NavReload()
    {
        Vector3 destination = Vector3.zero;
        if (tag == "redAgent")
        {
            destination = transform.parent.Find("Robo World/Red Zone/Red Reload").GetComponent<MapReload>().center;
        }
        else if (tag == "blueAgent")
        {
            destination = transform.parent.Find("Robo World/Blue Zone/Blue Reload").GetComponent<MapReload>().center;
        }
        else return;
        pathFinder.enabled = true;
        pathFinder.SetDestination(destination);
    }
    public void GoReload()
    {
        Vector3 destination = Vector3.zero;
        if (tag == "redAgent")
        {
            destination = transform.parent.Find("Robo World/Red Zone/Red Reload").GetComponent<MapReload>().center;
        }
        else if (tag == "blueAgent")
        {
            destination = transform.parent.Find("Robo World/Blue Zone/Blue Reload").GetComponent<MapReload>().center;
        }
        else return;
        roboRigidbody.MovePosition(destination);
        roboRigidbody.rotation = Quaternion.Euler(firstRotation);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "wall")
        {
            doesWobble = false;
            wobble = 0;
            fixedPivot.rotation = Quaternion.Euler(transform.eulerAngles);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "wall") doesWobble = true;
    }
}
