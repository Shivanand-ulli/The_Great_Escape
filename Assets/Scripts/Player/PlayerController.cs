using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpSpeed = 10f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] LayerMask groundMask;
    Animator animator;
    Vector2 moveInput;
    Rigidbody2D myRb;
    CapsuleCollider2D capsuleCollider2D;
    bool isAlive = true;
    float startGravityScale;
    public ParticleSystem playerParticle;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        myRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        isAlive = true;
        startGravityScale = myRb.gravityScale;
        playerParticle.gameObject.SetActive(false);
    }


    void Update()
    {
        if (!isAlive) return;
        onRun();
        ClimbLadder();
        Die();
    }


    void OnMove(InputValue value)
    {
        if (!isAlive) return;
        moveInput = value.Get<Vector2>();
    }

    void onRun()
    {
        if (!isAlive) return;

        Vector2 playerMovement = new Vector2(moveInput.x * moveSpeed, myRb.linearVelocity.y);
        myRb.linearVelocity = playerMovement;
        if (onGround())
        {
            animator.SetFloat("isRunning", Mathf.Abs(myRb.linearVelocity.x));
        }
        flipSprite();
    }

    void flipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRb.linearVelocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRb.linearVelocity.x), 1f);
        }
    }

    void OnJump(InputValue value)
    {
        if (!isAlive || !onGround()) return;

        if (value.isPressed)
        {
            AudioManager.instance.PlaySFX(8);
            myRb.linearVelocity = new Vector2(myRb.linearVelocity.x, jumpSpeed);
            animator.SetTrigger("Jump");
        }
    }

    bool onGround()
    {
        return Physics2D.BoxCast(capsuleCollider2D.bounds.center, capsuleCollider2D.bounds.size, 0, Vector2.down, 0.1f, groundMask);
    }

    void ClimbLadder()
    {
        if (!isAlive) return;
        if (!capsuleCollider2D.IsTouchingLayers(LayerMask.GetMask("Ladder")))
        {
            myRb.gravityScale = startGravityScale;
            return;
        }

        Vector2 climbVelocity = new Vector2(myRb.linearVelocity.x, moveInput.y * climbSpeed);
        myRb.linearVelocity = climbVelocity;

        bool playerHasVerticalSpeed = Mathf.Abs(myRb.linearVelocity.y) > Mathf.Epsilon;
        animator.SetBool("isClimbing", playerHasVerticalSpeed);
        animator.SetFloat("isRunning", 0);
        myRb.gravityScale = 0;
    }

    void Die()
    {
        if (!isAlive) return;
        if (capsuleCollider2D.IsTouchingLayers(LayerMask.GetMask("Hazard", "Enemy")))
        {
            isAlive = false;
            myRb.linearVelocity = Vector2.zero;
            myRb.gravityScale = 10;
            // myRb.bodyType = RigidbodyType2D.Dynamic;
            animator.SetTrigger("Death");
            AudioManager.instance.PlaySFX(5);
            StartCoroutine(CallCoroutine());
        }
    }

    IEnumerator CallCoroutine()
    {
        yield return new WaitForSeconds(2f);
        GameSessionController.instance.ProcessOfPlayerDeath();
    }


    public void MoveLeft()
    {
        moveInput = new Vector2(-1, moveInput.y); // Move left
    }

    public void MoveRight()
    {
        moveInput = new Vector2(1, moveInput.y); // Move right
    }

    public void MoveUp()
    {
        moveInput = new Vector2(moveInput.x, 1); // Move up (for ladder or vertical movement)
    }

    public void MoveDown()
    {
        moveInput = new Vector2(moveInput.x, -1); // Move down (for ladder or vertical movement)
    }

    public void StopHorizontalMovement()
    {
        moveInput = new Vector2(0, moveInput.y); // Stop horizontal movement
    }

    public void StopVerticalMovement()
    {
        moveInput = new Vector2(moveInput.x, 0); // Stop vertical movement
    }

    public void Jump()
    {
        if (!isAlive || !onGround()) return;
        myRb.linearVelocity = new Vector2(myRb.linearVelocity.x, jumpSpeed); // Perform jump
        AudioManager.instance.PlaySFX(8);
        animator.SetTrigger("Jump");
    }

    public void paritclePlay()
    {
        playerParticle.gameObject.SetActive(true);
    }

}
