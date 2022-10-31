using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerZ : MonoBehaviour
{
    PlayerController playerController;
    private float offset;
    private void Start()
    {
        playerController = SingletonFactory.Instance.playerController;
        offset = transform.position.z;
    }

    void LateUpdate()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, playerController.transform.position.z + offset);
    }
}
