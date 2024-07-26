using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Hero : Entity
{
    [SerializeField] private float speed = 3f;
    [SerializeField] public int currentHealth;
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private float jumpForce = 1f;
    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;
    private bool isAlive;

    private bool isGrounded = false;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;

    [SerializeField] private AudioSource runSound;
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource missAttackSound;
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private AudioSource damageSound;
    [SerializeField] private AudioSource deathSound;

    public bool isAttacking = false;
    public bool isRecharger = true;

    public Transform attackPos;
    public float attackRange;
    public LayerMask enemy;

    public static Hero Instance { get; set; }

    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();
        Instance = this;
        isRecharger = true;
        currentHealth = maxHealth;
        isAlive = true;
    }
    private void Update() {
        CheckGround();
        if (isGrounded && !isAttacking && isAlive) State = States.idle;
        if (!isAttacking && isAlive && Input.GetButton("Horizontal")) {
            Run();
        }
        if (!isAttacking && isAlive && isGrounded && Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        if (isAlive && Input.GetButtonDown("Fire1")) {
            Attack();
        }

        if (currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }
        for (int i = 0; i < hearts.Length; i++) {
            if (i < currentHealth)
            {
                hearts[i].sprite = fullHeart;
            }
            else {
                hearts[i].sprite = emptyHeart;
            }

        }
    }
    private void Run() {
        if (isGrounded) State = States.run;
        Vector3 dir = transform.right * Input.GetAxis("Horizontal");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);
        sprite.flipX = dir.x < 0.0f;
    }
    private void Jump() {
        PlayJumpSound();
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }
    private void CheckGround() {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, 0.3f);
        isGrounded = collider.Length > 1;
        if (!isGrounded) State = States.jump;
    }
    public enum States
    {
        idle,
        run,
        jump,
        attack,
        death

    }
    public override void GetDamage() {
        currentHealth -= 1;
        PlayDamageSound();
        if (currentHealth < 1)
        {
            Die();
        }
    }
    public override void Die() {
        State = States.death;
        isAlive = false;
        PlayDeathSound();
    }
    public void RestartLevel() {
        SceneManager.LoadScene(1);
    }
    public void Attack()
    {
        if (isGrounded && isRecharger)
        {
            State = States.attack;
            isAttacking = true;
            isRecharger = false;

            StartCoroutine(AttackAnimation());
            StartCoroutine(AttackCoolDown());
        }
    }
    public void OnAttack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPos.position, attackRange, enemy);

        if (colliders.Length == 0)
        {
            PlayMissAttackSound();
        }
        else {
            PlayAttackSound();
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].GetComponent<Entity>().GetDamage();
        }
        //Debug.Log("Attacking");
    }
    private IEnumerator AttackAnimation()
    {
        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
    }
    private IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(0.5f);
        isRecharger = true;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("DeadZone"))
        {
            currentHealth = 0;
            RestartLevel();
        }
    }
    public void PlayRunSound() {
        runSound.Play();
    }
    public void PlayJumpSound()
    {
        jumpSound.Play();
    }
    public void PlayMissAttackSound()
    {
        missAttackSound.Play();
    }
    public void PlayAttackSound()
    {
        attackSound.Play();
    }
    public void PlayDamageSound()
    {
        damageSound.Play();
    }
    public void PlayDeathSound()
    {
        deathSound.Play();
    }
}
