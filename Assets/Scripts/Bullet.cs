using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed;
    public string firedRobot { get; private set; }
    private GameObject firedObject;
    private Rigidbody bulletRigidbody;
    void Start()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
        bulletRigidbody.velocity = transform.forward * bulletSpeed;        
        Destroy(gameObject, 2f);
    }
    public void GetFiredRobot(string robot, GameObject fired)
    {
        firedRobot = robot;
        firedObject = fired;
    }
    private void OnCollisionEnter(Collision collision)
    {
        GameObject collideObject = collision.collider.gameObject;
        if (collideObject.tag.Contains("Armor"))        
        {
            RoboArmor roboArmor = collideObject.GetComponent<RoboArmor>();
            bool isdead = roboArmor.transform.parent.parent.GetComponent<RoboState>().dead;
            roboArmor.Attacked(firedRobot);
            firedObject.GetComponent<RoboState>().ShootDead(isdead);
            Destroy(gameObject, 0.3f);
        }
        else Destroy(gameObject, 0.5f);
    }
}
