using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;
    public float health;
    public float maxHealth = 100f;
    public int itemCount = 0;
    public MeshRenderer model;

    public float ping;
    public float pong;
    public float latency
    {
        get
        {
            return pong - ping;
        }
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;
    }

    public void SetHealth(float _health)
    {
        health = _health;

        if (health <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        model.enabled = false;
    }

    public void Respawn()
    {
        model.enabled = true;
        SetHealth(maxHealth);
    }
    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        // Code to execute after the delay
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

    IEnumerator PingPong()
    {
        while (true)
        {
            yield return new WaitForSeconds(5.0f);
            GameManager.instance.ping = Time.deltaTime;
            ClientSend.Ping();

        }
    }
}
