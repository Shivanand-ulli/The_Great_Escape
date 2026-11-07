using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Rigidbody2D myRigidbody;
    public BoxCollider2D myBoxCollider;
    public Transform posA, posB; 
    public float speed;

    private Vector2 targetPos;
    private bool isFacingRight = true; 
    public List<ParticleSystem> enemyParticle;

    void Start()
    {
        targetPos = posB.position; 
        myRigidbody = GetComponent<Rigidbody2D>();
        enemyParticle[1].gameObject.SetActive(false);
    }

    void Update()
    {
        MoveEnemy(); 
        FlipEnemy(); 
    }

    void MoveEnemy()
    {
        // Move towards the target position
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // Switch target position when reaching posA or posB
        if (Vector2.Distance(transform.position, posA.position) < 1.0f)
        {
            targetPos = posB.position;
        }
        else if (Vector2.Distance(transform.position, posB.position) < 1.0f)
        {
            targetPos = posA.position;
        }
    }

    void FlipEnemy()
    {
        if (targetPos.x > transform.position.x && !isFacingRight)
        {
            Flip();
        }
        else if (targetPos.x < transform.position.x && isFacingRight)
        {
            Flip(); 
        }
    }

    void Flip()
    {
        // Flip the enemy by scaling the X-axis
        isFacingRight = !isFacingRight;
        Vector3 enemyScale = transform.localScale;
        enemyScale.x *= -1; // Invert the x-axis scale
        transform.localScale = enemyScale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(posA.position, posB.position); // Visualize the movement path
    }

    public void vfx1()
    {
        enemyParticle[0].Play();
    }

    public void vfx2()
    {
        enemyParticle[1].gameObject.SetActive(true);
        //enemyParticle[1].Play();
    }
}
