using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] GameObject attackPoint;
    [SerializeField] float attackPointRadius;
    public LayerMask enemy;
    Animator animator;


    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void StartAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
            SoundManager.Instance.PlaySFX(Sfx.Sword);  
        }
    }

    void OnAttack(InputValue value)
    {
        if (value.isPressed)
        {
            animator.SetTrigger("Attack");
            SoundManager.Instance.PlaySFX(Sfx.Sword);
        }
    }

    public void Attack()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackPointRadius, enemy);

        foreach (Collider2D hit in enemiesHit)
        {
            if (hit.CompareTag("Hazard"))
            {
                Animator enemyAnim = hit.GetComponent<Animator>();
                enemyAnim.SetTrigger("Die");

                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.speed = 0;
                    enemy.myBoxCollider.enabled = false;
                    SoundManager.Instance.PlaySFX(Sfx.EnemyDeath);
                    enemy.vfx1();
                }

                hit.gameObject.layer = LayerMask.NameToLayer("Default");
            }

            StartCoroutine(enemyDeath(hit));
        }
    }

    IEnumerator enemyDeath(Collider2D hit)
    {
        yield return new WaitForSeconds(2);
        Destroy(hit.gameObject);
    }

    void OnDrawGizmos()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.transform.position, attackPointRadius);
    }

    // public void playSfx()
    // {
    //     SoundManager.Instance.PlaySFX(SfxName.EnemyDeath);
    // }
}
