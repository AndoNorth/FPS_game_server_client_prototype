using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public int id;
    public float health;
    public float maxHealth = 100f;
    public MeshRenderer model;

    public void Initialize(int _id)
    {
        id = _id;
        health = maxHealth;
    }

    public void SetHealth(float _health)
    {
        health = _health;

        if (health <= 0f)
        {
            GameManager.enemies.Remove(id);
            Destroy(gameObject);
        }
    }
    public void TookDamage()
    {
        StartCoroutine(ColorToggle());
    }
    IEnumerator ColorToggle()
    {
        Color oldColor = model.material.color;
        model.material.SetColor("_Color", Color.red);
        yield return new WaitForSeconds(0.1f);
        model.material.SetColor("_Color", oldColor);
    }
}
