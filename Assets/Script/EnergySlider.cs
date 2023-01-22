using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergySlider : MonoBehaviour
{
	[SerializeField] PlayerManager player;
	Slider slider;
	private void Start()
	{
		slider = GetComponent<Slider>();
	}

	private void Update()
	{
		slider.value = player.energy / 100f;
	}
}
