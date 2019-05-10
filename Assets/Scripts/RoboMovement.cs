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
    private RoboAgent roboAgent;
    private bool doesWobble = true;
    private bool wobbleDir = true;
    public bool realWobble { get; private set; }
    private float wobble = 0f;
    private float wobbleMax = 40f;
    private float wSign = 0f;
    private float wobbleSpeed = 180f;

    private void OnEnable()
    {
        moveInput = GetComponent<MoveInput>();
        roboAgent = GetComponent<RoboAgent>();
        if (moveInput.manual)
        {
            moveSpeed = 4f;
            rotateSpeed = 240f;
        }
        else
        {
            moveSpeed = 4f;
            rotateSpeed = 240f;
        }
        firstPosition = transform.position;
        firstRotation = transform.eulerAngles;
        
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
        //GimbalRotate();
        AgentGimbal(roboAgent.rayObject);
        realWobble = doesWobble && moveInput.wobbleInput;
        // Wobble은 조건을 따진다.
        if (realWobble) Wobble(wobbleSpeed * Time.deltaTime);
        else
        {
            wobble = 0f;
            fixedPivot.rotation = Quaternion.Euler(transform.eulerAngles);
        }
        Move();
        OverTurn();
        //NavReload();
        //GoReload();
    }
    private void OverTurn()
    {
        //Debug.Log(name + "\t" + transform.eulerAngles.ToString());
        Vector3 currentAngle = transform.eulerAngles;
        if (currentAngle.x > 180f) currentAngle.x -= 360f;
        if (currentAngle.z > 180f) currentAngle.z -= 360f;
        if (Mathf.Abs(currentAngle.x) > 40f || Mathf.Abs(currentAngle.z) > 40f)
        {
            roboRigidbody.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
        }
    }
    
    public void Wobble(float wSpeed)
    {
        float angle = Vector3.Angle(transform.forward, fixedPivot.forward);
        if (Vector3.Cross(transform.forward, fixedPivot.forward).y > 0) angle *= -1;
        if (wobbleDir)
        {
            wSign += 0.2f;
            if (wSign >= 1f) wSign = 1f;
            if (angle >= wobbleMax)
            {
                wobbleDir = false;
            }
        }
        else
        {
            wSign -= 0.2f;
            if (wSign <= -1f) wSign = -1f;
            if (angle <= -wobbleMax)
            {
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
        if (!moveInput.manual) return;
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

    public void AgentGimbal(GameObject robot)
    {
        if (!roboAgent.enemyDetect) return;
        if (moveInput.manual) return;
        if (robot.GetComponent<RoboState>().dead) return;
        Transform target = transform;
        List<Transform> armor = new List<Transform>();
        List<Vector3> armorCenter = new List<Vector3>();
        List<float> armorDistance = new List<float>();
        armor.Add(robot.transform.Find("Armor Plate/Front Armor"));
        armor.Add(robot.transform.Find("Armor Plate/Left Armor"));
        armor.Add(robot.transform.Find("Armor Plate/Rear Armor"));
        armor.Add(robot.transform.Find("Armor Plate/Right Armor"));
        foreach (Transform armorName in armor) armorCenter.Add(armorName.GetComponent<Collider>().bounds.center);
        foreach (Vector3 center in armorCenter)
        {
            armorDistance.Add(Vector2.Distance(new Vector2(transform.position.x, transform.position.z),
                                               new Vector2(center.x, center.z)));
        }
        float disMin = 30f;
        for(int i=0;i<armorDistance.Count;i++)
        {
            if (armorDistance[i] < disMin)
            {
                disMin = armorDistance[i];
                target = armor[i];
            }
        }
        float hgim = hGimbalPivot.localEulerAngles.y;
        float vgim = vGimbalPivot.localEulerAngles.x;
        if (hgim > 180f) hgim -= 360f;
        if (vgim > 180f) vgim -= 360f;
        //Debug.Log(name+"\t"+target.parent.parent.name + ":" + target.name + "\t(" + target.position.x.ToString() + ",\t" + target.position.z.ToString() + ")");
        Vector3 hdir = target.GetComponent<Collider>().bounds.center - hGimbalPivot.position;
        Vector3 hforward = hGimbalPivot.forward;
        Vector3 vdir = target.GetComponent<Collider>().bounds.center - vGimbalPivot.position;
        //Vector3 vforward = vGimbalPivot.forward;
        hdir.y = 0;
        hforward.y = 0;
        float hAngle = Vector3.Angle(hdir, hforward);
        //float vAngle = Vector3.Angle(vdir, vforward);
        // 외적 값이 양수면 음수로
        if (Vector3.Cross(hdir, hforward).y > 0) hAngle *= -1;
        //if (Vector3.Cross(vdir, vforward).x > 0) vAngle *= -1;
        if (hgim + hAngle >= hMax)
        {
            hAngle = hMax - hgim;
        }
        else if (hgim + hAngle <= -hMax)
        {
            hAngle = -hMax - hgim;
        }        
        //if (vgim + vAngle <= -vMaxU)
        //{
        //    vAngle = -vMaxU - vgim;
        //}
        //else if (vgim + vAngle >= vMaxD)
        //{
        //    vAngle = vMaxD - vgim;
        //}
        //Debug.Log(hdir.ToString() + "\t" + hAngle.ToString());        
        //Debug.Log(vdir.ToString() + "\t" + vAngle.ToString());
        Debug.DrawRay(vGimbalPivot.position, vdir, Color.blue, 0.01f, true);
        gimbalRigidbody.transform.RotateAround(hGimbalPivot.position, hGimbalPivot.up, hAngle * 20 * Time.deltaTime);
        hGimbalPivot.rotation *= Quaternion.Euler(0f, hAngle * 20 * Time.deltaTime, 0f);
        //gimbalHeadRigidbody.transform.RotateAround(vGimbalPivot.position, vGimbalPivot.right, vAngle * 20 * Time.deltaTime);
        //vGimbalPivot.rotation *= Quaternion.Euler(vAngle * 20 * Time.deltaTime, 0f, 0f);
        GetComponent<RoboShooter>().AgentFire(target);
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
}
