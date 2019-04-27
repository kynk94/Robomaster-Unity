using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInput : MonoBehaviour
{
    public string vMoveAxisName = "Vertical"; // 본체 전후
    public string hMoveAxisName = "Horizontal"; // 본체 좌우
    public string rotateAxisName = "Rotate"; // 본체 회전
    public string gimbalUpDownName = "Mouse Y"; // 짐벌 위아래
    public string gimbalRotateName = "Mouse X"; // 짐벌 좌우
    public string reloadButtonName = "Reload";
    public string restartButtonName = "Restart";
    public string fireButtonName = "Fire1";
    public string wobbleButtonName = "Wobble";

    public Vector3 direction { get; private set; }
    public float vMove { get; private set; }
    public float hMove { get; private set; }
    public float vGimbalRotate { get; private set; }
    public float hGimbalRotate { get; private set; }
    public float rotate { get; private set; }
    public bool fire { get; private set; }
    public bool reload { get; private set; }
    public bool restart { get; private set; }
    public bool wobble { get; private set; }
    public bool wobbleInput { get; private set; }

    private void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isGameover)
        {
            vMove = 0;
            hMove = 0;
            vGimbalRotate = 0;
            hGimbalRotate = 0;
            rotate = 0;
            fire = false;
            reload = false;
            wobble = false;
            wobbleInput = false;
            // restart는 여기 넣으면 안됨.
            return;
        }
        vMove = Input.GetAxis(vMoveAxisName);
        hMove = Input.GetAxis(hMoveAxisName);
        direction = new Vector3(hMove, 0, vMove);
        if (direction.magnitude >= 1)
        {
            direction /= direction.magnitude;
        }
        vGimbalRotate = Input.GetAxis(gimbalUpDownName);
        hGimbalRotate = Input.GetAxis(gimbalRotateName);
        rotate = Input.GetAxis(rotateAxisName);
        fire = Input.GetButton(fireButtonName);
        reload = Input.GetButtonDown(reloadButtonName);
        restart = Input.GetButtonDown(restartButtonName);
        wobble = Input.GetButtonDown(wobbleButtonName);
        if (wobble) wobbleInput = !wobbleInput;
    }
}
