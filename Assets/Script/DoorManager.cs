using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
	[SerializeField] Transform openedDoor;
	[SerializeField] Transform closedDoor;

	private void Start()
	{
		openedDoor.gameObject.SetActive(false);
		closedDoor.gameObject.SetActive(true);
	}

	public void SwitchDoor()
	{
		closedDoor.gameObject.SetActive(!closedDoor.gameObject.activeSelf);
		openedDoor.gameObject.SetActive(!openedDoor.gameObject.activeSelf);
	}
}
