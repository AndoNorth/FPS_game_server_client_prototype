﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;
    public CharacterController controller;
    public Transform shootOrigin;
    public float gravity = -9.81f;
    public float moveSpeed = 5f;
    public float jumpSpeed = 5f;
    public float throwForce = 600f;
    public float health;
    public float maxHealth = 100f;
    public int itemAmount = 0;
    public int maxItemAmount = 3;
    // shooting
    private bool addBulletSpread = false;
    private Vector3 bulletSpreadVariance = new Vector3(0.1f, 0.1f, 0.1f);
    private float maxShotDistance = 25f;
    private float maxFarShotDistance = 50f;
    private float shotDelay = 1.0f;
    private float lastShotTime;
    // latency


    private bool[] inputs;
    private float yVelocity = 0;

    private void Start()
    {
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        jumpSpeed *= Time.fixedDeltaTime;
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;

        inputs = new bool[5];
    }

    /// <summary>Processes player input and moves the player.</summary>
    public void FixedUpdate()
    {
        if (health <= 0f)
        {
            return;
        }

        Vector2 _inputDirection = Vector2.zero;
        if (inputs[0])
        {
            _inputDirection.y += 1;
        }
        if (inputs[1])
        {
            _inputDirection.y -= 1;
        }
        if (inputs[2])
        {
            _inputDirection.x -= 1;
        }
        if (inputs[3])
        {
            _inputDirection.x += 1;
        }

        Move(_inputDirection);
    }

    /// <summary>Calculates the player's desired movement direction and moves him.</summary>
    /// <param name="_inputDirection"></param>
    private void Move(Vector2 _inputDirection)
    {
        Vector3 _moveDirection = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;
        _moveDirection *= moveSpeed;

        if (controller.isGrounded)
        {
            yVelocity = 0f;
            if (inputs[4])
            {
                yVelocity = jumpSpeed;
            }
        }
        yVelocity += gravity;

        _moveDirection.y = yVelocity;
        controller.Move(_moveDirection);

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }

    /// <summary>Updates the player input with newly received input.</summary>
    /// <param name="_inputs">The new key inputs.</param>
    /// <param name="_rotation">The new rotation.</param>
    public void SetInput(bool[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
    }

    public void ShootRay(Vector3 _viewDirection)
    {
        if (health <= 0f)
        {
            return;
        }

        if (lastShotTime + shotDelay < Time.time)
        {
            Vector3 shotDirection = _viewDirection;
            if (addBulletSpread)
            {
                shotDirection = applyBulletSpread(shotDirection);
            }
            if (Physics.Raycast(shootOrigin.position, shotDirection, out RaycastHit _hit, maxShotDistance))
            {
                ServerSend.RayShot(shootOrigin.position, _hit.point);
                if (_hit.collider.CompareTag("Player"))
                {
                    _hit.collider.GetComponent<Player>().TakeDamage(50f);
                }
                else if (_hit.collider.CompareTag("Enemy"))
                {
                    _hit.collider.GetComponent<Enemy>().TakeDamage(50f);
                }
            }
            else if (Physics.Raycast(shootOrigin.position, shotDirection, out RaycastHit _hitFar, maxFarShotDistance))
            {
                ServerSend.RayShot(shootOrigin.position, _hitFar.point);
                if (_hitFar.collider.CompareTag("Player"))
                {
                    _hitFar.collider.GetComponent<Player>().TakeDamage(25f);
                }
                else if (_hit.collider.CompareTag("Enemy"))
                {
                    _hit.collider.GetComponent<Enemy>().TakeDamage(25f);
                }
            }
            else if (Physics.Raycast(shootOrigin.position, shotDirection, out RaycastHit _hitEnd))
            {
                ServerSend.RayShot(shootOrigin.position, _hitEnd.point);
            }
        }
    }
    private Vector3 applyBulletSpread(Vector3 _direction)
    {
        Vector3 direction = _direction;
        direction += new Vector3(
            Random.Range(-bulletSpreadVariance.x, bulletSpreadVariance.x),
            Random.Range(-bulletSpreadVariance.y, bulletSpreadVariance.y),
            Random.Range(-bulletSpreadVariance.z, bulletSpreadVariance.z)
            );
        direction.Normalize();
        return direction;
    }
    // TODO: try projectile shooting
    public void ShootProjectile(Vector3 _viewDirection)
    {
        if (health <= 0f)
        {
            return;
        }
        // shooting logic
        if (Physics.Raycast(shootOrigin.position, _viewDirection, out RaycastHit _hit, 25f))
        {
            if (_hit.collider.CompareTag("Player"))
            {
                _hit.collider.GetComponent<Player>().TakeDamage(50f);
            }
        }
    }

    public void ThrowItem(Vector3 _viewDirection)
    {
        if (health <= 0f)
        {
            return;
        }

        if (itemAmount > 0)
        {
            itemAmount--;
            NetworkManager.instance.InstantiateProjectile(shootOrigin).Initialize(_viewDirection, throwForce, id);
        }
    }

    public void TakeDamage(float _damage)
    {
        if (health <= 0f)
        {
            return;
        }

        health -= _damage;
        if (health <= 0f)
        {
            health = 0f;
            controller.enabled = false;
            transform.position = new Vector3(0f, 25f, 0f);
            ServerSend.PlayerPosition(this);
            StartCoroutine(Respawn());
        }

        ServerSend.PlayerHealth(this);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);

        health = maxHealth;
        controller.enabled = true;
        ServerSend.PlayerRespawned(this);
    }

    public bool AttemptPickupItem()
    {
        if (itemAmount >= maxItemAmount)
        {
            return false;
        }

        itemAmount++;
        return true;
    }
}
