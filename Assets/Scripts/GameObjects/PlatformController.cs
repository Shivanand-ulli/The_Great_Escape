using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public Transform posA, posB;
    public float speed;

    Vector2 targetPos;
    void Start()
    {
        targetPos = posB.position;
    }
    void Update()
    {
        if (Vector2.Distance(transform.position, posA.position) < 1.0f)
        {
            targetPos = posB.position;
        }
        if (Vector2.Distance(transform.position, posB.position) < 1.0f)
        {
            targetPos = posA.position;
        }
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.SetParent(this.transform);
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(UnparentAfterFrame(collision.transform));
        }
    }

    private IEnumerator UnparentAfterFrame(Transform playerTransform)
    {
        yield return null; // Wait for the next frame
        playerTransform.SetParent(null);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(posA.position, posB.position);
    }
}
