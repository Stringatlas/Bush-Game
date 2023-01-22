using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] PlayerManager playerManager;

    bool isAnsweringQuestion;

    void Start()
    {
        playerManager.enabled = true;
    }

    public void DisablePlayerScript()
	{
        playerManager.RemoveVelocity();
        playerManager.enabled = false;
	}

    public void EnablePlayerScript()
	{
        playerManager.enabled = true;
	}

    void Update()
    {

    }
}
