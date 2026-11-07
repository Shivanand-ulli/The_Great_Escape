using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public GameObject movingSpine;
    public float speed;
    public Transform topPos, bottomPos;

    bool moveToTop = false;
    bool moveToBottom = false;

    void Start()
    {
        movingSpine.transform.position = bottomPos != null ? bottomPos.position : movingSpine.transform.position;
    }


    void Update()
    {
        if (moveToTop)
        {
            movingSpine.transform.position = Vector2.MoveTowards(movingSpine.transform.position, topPos.position, speed * Time.deltaTime);

            if (movingSpine.transform.position == topPos.position)
            {
                moveToTop = false;
            }
        }
        else if (moveToBottom)
        {
            movingSpine.transform.position = Vector2.MoveTowards(movingSpine.transform.position, bottomPos.position, speed * Time.deltaTime);

            if (movingSpine.transform.position == bottomPos.position)
            {
                moveToBottom = false;
            }
        }
    }
    void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.collider.CompareTag("Player") || collision2D.collider.CompareTag("Box"))
        {
            // print("Player is on the capsule");
            moveToTop = true;
            moveToBottom = false;
        }
    }
    void OnCollisionExit2D(Collision2D collision2D)
    {
        if (collision2D.collider.CompareTag("Player") || collision2D.collider.CompareTag("Box"))
        {
            moveToTop = false;
            moveToBottom = true;
        }
    }
}
