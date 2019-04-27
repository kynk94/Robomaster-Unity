using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed;
    public string firedRobot { get; private set; }
    private Rigidbody bulletRigidbody;
    void Start()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
        bulletRigidbody.velocity = transform.forward * bulletSpeed;

        Destroy(gameObject, 2f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject collideObject = collision.collider.gameObject;
        if (collideObject.tag.Contains("Armor"))        
        {
            firedRobot = collision.transform.tag;
            Debug.Log(collideObject.tag);
            Destroy(gameObject, 0.3f);
        }
        else Destroy(gameObject, 0.5f);
    }
}
