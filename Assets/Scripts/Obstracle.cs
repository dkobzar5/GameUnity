using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstracle : Entity
{
    [SerializeField] private int lives = 3;
    public void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject == Hero.Instance.gameObject)
        {
            Hero.Instance.GetDamage();
        }
    }
    public override void GetDamage()
    {
        lives--;
        Debug.Log("У перешкоди залишилося " + lives + "життів");
        if (lives < 1)
        {
            Die();
        }
    }
}
