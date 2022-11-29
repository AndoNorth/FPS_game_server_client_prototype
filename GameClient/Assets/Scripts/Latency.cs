using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Latency : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(PingPong());
    }
    private void OnDisable()
    {
        StopCoroutine(PingPong());
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
