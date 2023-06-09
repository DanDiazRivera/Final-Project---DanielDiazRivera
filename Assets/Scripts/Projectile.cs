using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigidbody2d;
    
    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }
    
    public void Launch(Vector2 direction, float force)
    {
        rigidbody2d.AddForce(direction * force);
    }
    
    void Update()
    {
        if(transform.position.magnitude > 1000.0f)
        {
            Destroy(gameObject);
        }
    }
    
    void OnCollisionEnter2D(Collision2D other)
    {
        EnemyController e1 = other.collider.GetComponent<EnemyController>();
        if (e1 != null)
        {
            e1.Fix();
        }
        HardEnemyController e2 = other.collider.GetComponent<HardEnemyController>();
        if (e2 != null)
        {
            e2.Fix();
        }
    
        Destroy(gameObject);
    }
}