﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, Player> players = new Dictionary<int, Player>();
    public static Dictionary<int, ItemSpawner> itemSpawners = new Dictionary<int, ItemSpawner>();
    public static Dictionary<int, ProjectileManager> projectiles = new Dictionary<int, ProjectileManager>();
    public static Dictionary<int, EnemyManager> enemies = new Dictionary<int, EnemyManager>();

    public GameObject world;

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;
    public GameObject itemSpawnerPrefab;
    public GameObject projectilePrefab;
    public GameObject enemyPrefab;
    public TrailRenderer bulletTrail;

    public float ping;
    public float pong;
    public float latency
    {
        get
        {
            return pong - ping;
        }
    }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    /// <summary>Spawns a player.</summary>
    /// <param name="_id">The player's ID.</param>
    /// <param name="_name">The player's name.</param>
    /// <param name="_position">The player's starting position.</param>
    /// <param name="_rotation">The player's starting rotation.</param>
    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation)
    {
        GameObject _player;
        if (_id == Client.instance.myId)
        {
            _player = Instantiate(localPlayerPrefab, _position, _rotation);
        }
        else
        {
            _player = Instantiate(playerPrefab, _position, _rotation);
        }

        _player.GetComponent<Player>().Initialize(_id, _username);
        players.Add(_id, _player.GetComponent<Player>());
    }

    public void CreateItemSpawner(int _spawnerId, Vector3 _position, bool _hasItem)
    {
        GameObject _spawner = Instantiate(itemSpawnerPrefab, _position, itemSpawnerPrefab.transform.rotation);
        _spawner.transform.SetParent(world.transform, false);
        _spawner.GetComponent<ItemSpawner>().Initialize(_spawnerId, _hasItem);
        itemSpawners.Add(_spawnerId, _spawner.GetComponent<ItemSpawner>());
    }

    public void SpawnProjectile(int _id, Vector3 _position)
    {
        GameObject _projectile = Instantiate(projectilePrefab, _position, Quaternion.identity);
        _projectile.GetComponent<ProjectileManager>().Initialize(_id);
        projectiles.Add(_id, _projectile.GetComponent<ProjectileManager>());
    }

    public void DrawRayShot(Vector3 _shotOrigin, Vector3 _rayHitPoint)
    {
        TrailRenderer trail = Instantiate(bulletTrail, _shotOrigin, Quaternion.identity);

        StartCoroutine(SpawnTrail(trail, _rayHitPoint));
    }

    private IEnumerator SpawnTrail(TrailRenderer _trail, Vector3 _rayHitPoint)
    {
        float time = 0;
        Vector3 startPosition = _trail.transform.position;

        while(time < 1)
        {
            _trail.transform.position = Vector3.Lerp(startPosition, _rayHitPoint, time);
            time += Time.deltaTime / _trail.time;

            yield return null;
        }
        _trail.transform.position = _rayHitPoint;

        Destroy(_trail.gameObject, _trail.time);
    }
    IEnumerator CurrentPlayers()
    {
        while (true)
        {
            yield return new WaitForSeconds(5.0f);
            foreach (Player player in players.Values)
            {
                Debug.Log($"playerId: {player.id} - connected successfully and is now player {player.username}.");
            }

        }
    }
    public void SpawnEnemy(int _id, Vector3 _position)
    {
        GameObject _enemy = Instantiate(enemyPrefab, _position, Quaternion.identity);
        _enemy.GetComponent<EnemyManager>().Initialize(_id);
        enemies.Add(_id, _enemy.GetComponent<EnemyManager>());
    }
}
