using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] private int health = 5;
    public bool isAlive = true;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Update() {
        if (isAlive) {
            State = States.idle;
        }
        else
        {
            State = States.death;
        }
    }
    public override void GetDamage()
    {
        health--;
        Debug.Log("У скелета залишилося " + health + "життів");
        if (health < 1) {
            Die();
        }
    }
    public override void Die()
    {
        isAlive = false;
    }
    public enum States
    {
        idle,
        death

    }
    public void DestroyEnemy() {
        Destroy(this.gameObject);
    }
    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }
}
