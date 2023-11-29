using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    public static event Action OnEnemyKill;
    [SerializeField]
    private float Health = 6f; // if health reaches 0, this is destroyed
    // Start is called before the first frame update
    public void OnDamaged(float damage)
    {
        if (damage < 6)
            Debug.Log("Ouch!");
        else
            Debug.Log("CRUSHED!!");
        Health -= damage;
        if(Health <= 0)
            Destroy(gameObject);
    }

    // Update is called once per frame
    private void OnDisable()
    {
        OnEnemyKill?.Invoke();
    }
}
