using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerStateMachine : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float jumpYVelocity = 8f;
    [SerializeField] float runXVelocity = 4f;
    [SerializeField] float raycastDistance = 0.7f;
    [SerializeField] LayerMask collisionMask;
    [SerializeField] float attackDuration = 1f;

    Animator animator;
    Rigidbody2D physics;
    SpriteRenderer sprite;

    enum State { Idle, Run, Jump, Glide, Attack, Climb }

    State state = State.Idle;
    bool isGrounded = false;
    bool jumpInput = false;
    bool isAttack = false;
    bool isClimb = false;

    float horizontalInput = 0f;

    void FixedUpdate()
    {
        // get player input
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, collisionMask).collider != null;
        jumpInput = Input.GetKey(KeyCode.Space);
        horizontalInput = Input.GetAxisRaw("Horizontal");
        isAttack = Input.GetKey(KeyCode.E);
        isClimb = Input.GetKey(KeyCode.UpArrow);

        // flip sprite based on horizontal input
        if (horizontalInput > 0f)
        {
            sprite.flipX = false;
        }
        else if (horizontalInput < 0f)
        {
            sprite.flipX = true;
        }

        if (Parede == true)
        {

            state = State.Climb;

        }


        if (Parede == false)
        {

            Debug.Log("Ccccc");

        }

        // run current state
        switch (state)
        {
            case State.Idle: IdleState(); break;
            case State.Run: RunState(); break;
            case State.Jump: JumpState(); break;
            case State.Glide: GlideState(); break;
            case State.Attack: AttackState(); break;
            case State.Climb: ClimbState(); break;
        }
    }

    void IdleState()
    {
        // actions
        animator.Play("Idle");

        // transitions
        if (isAttack && horizontalInput == 0f)
            state = State.Attack;

        if (isGrounded)
        {
            if (jumpInput)
            {
                state = State.Jump;
            }
            else if (horizontalInput != 0f)
            {
                state = State.Run;
            }
        }
    }

    void RunState()
    {
        // actions
        animator.Play("Run");
        physics.velocity = runXVelocity * horizontalInput * Vector2.right;

        // transitions
        if (isGrounded && jumpInput)
        {
            state = State.Jump;
        }
        else if (horizontalInput == 0f)
        {
            state = State.Idle;
        }
    }

    void JumpState()
    {
        // actions
        animator.Play("Jump");
        physics.velocity = runXVelocity * horizontalInput * Vector2.right + jumpYVelocity * Vector2.up;

        // transitions
        state = State.Glide;

    }

    void GlideState()
    {
        // actions
        if (physics.velocity.y > 0f)
        {
            animator.Play("Jump");
        }
        else
        {
            animator.Play("Fall");
        }

        physics.velocity = physics.velocity.y * Vector2.up + runXVelocity * horizontalInput * Vector2.right;

        // transitions
        if (isGrounded)
        {
            if (horizontalInput != 0f)
            {
                state = State.Run;
            }
            else
            {
                state = State.Idle;
            }
        }
    }

    float currentAttack;
    void AttackState()
    {
        // actions
        animator.Play("Attack");
        currentAttack += Time.fixedDeltaTime;
        // transitions
        if (currentAttack > attackDuration)
        {
            state = State.Idle;
            currentAttack = 0f;
        }
    }

    void ClimbState()
    {

        // transitions
        if (isClimb)
        {
            physics.velocity = runXVelocity * Vector2.up;
            animator.Play("Climb");
            Debug.Log("Bbbbbb");

        }
        else if (horizontalInput != 0f)
        {

            state = State.Run;
            
        }
        else { state = State.Idle; Parede = false; }
        
    }

    bool Parede;
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Wall"))
        {

            Parede = true;
            Debug.Log("Aaaaaa");

        }
        
    }


    void Awake()
    {
        animator = GetComponent<Animator>();
        physics = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }
}
