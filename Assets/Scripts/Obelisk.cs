using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obelisk : Entity
{
    [SerializeField] private int health = 3;
    public override void GetDamage()
    {
        health--;
        Debug.Log("У обеліска залишилося " + health + "життів");
        if (health < 1)
        {
            Die();
        }
    }
    public override void Die() {
        Hero.Instance.currentHealth += 1;
        Destroy(this.gameObject);
    }
}
