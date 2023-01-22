using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerManager : MonoBehaviour
{
	[SerializeField] float speed;
	[SerializeField] Transform mainCamera;
	float normalSpeed;
	[SerializeField] ParticleSystem moveParticles;
	[SerializeField] ParticleSystem explosionParticleSystem;
	[SerializeField] float dieWaitTime;
	[SerializeField] Transform youWinText;

	[SerializeField] Transform gameOverText;
	[SerializeField] Transform gameOverText2;

	[SerializeField] AudioSource sprintSound;
	[SerializeField] AudioSource walkSound;

	[SerializeField] Volume postProcessVolume;

	[SerializeField] float lowEnergyAmount;
	Rigidbody2D rigidBody;
	Animator animator;

	bool isMoving;
	bool isTouchingPushable;
	[Range(0, 100)] public float energy;
	bool hasDoorKey;
	bool hasChestKey;

	bool isSprinting;

	Transform pushable;
	Rigidbody2D pushableRigidbody;

	float timeSinceLastMoved;
	[SerializeField] float timeToSprint = 0.25f;

	float normalFov;
	CameraFollow mainCameraScript;

	SpriteRenderer spriteRenderer;

	Vignette vg;
	bool justStaredMoving;
	GameManager gameManager;

	bool sprintSoundIsPlaying;
	bool walkSoundIsPlaying;

	public void RemoveVelocity()
	{
		rigidBody.velocity = Vector2.zero;
	}
	private void Awake()
	{

		gameOverText.gameObject.SetActive(false);
		gameOverText2.gameObject.SetActive(false);
		youWinText.gameObject.SetActive(false);
		spriteRenderer = GetComponent<SpriteRenderer>();
		mainCameraScript = mainCamera.GetComponent<CameraFollow>();
		normalFov = mainCameraScript.normalFov;
		normalSpeed = speed;
		//energy = 100f;
		animator = GetComponent<Animator>();
		rigidBody = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		postProcessVolume.profile.TryGet(out vg);
		gameManager = FindObjectOfType<GameManager>();
		ClearVignette();

	}

	private void Update()
	{
		UpdateMovement();
		if (Input.GetKeyDown(KeyCode.T))
		{
			Explode(dieWaitTime);
			Debug.Log("explode");
		
		}
		CheckEnergy();
		UpdateCameraFov();
		if (energy < lowEnergyAmount)
		{
			LowEnergyEffect();
		}
		else
		{
			ClearVignette();
		}
		var moveParticlesRenderer = moveParticles.GetComponent<Renderer>();
		moveParticlesRenderer.sortingLayerID = spriteRenderer.sortingLayerID;

		if (isSprinting && !sprintSoundIsPlaying)
		{
			sprintSound.Play();
			sprintSoundIsPlaying = true;
			return;
		}
		else if (!isSprinting && sprintSoundIsPlaying)
		{
			sprintSound.Stop();
			sprintSoundIsPlaying = false;
			return;
		}

		if (isMoving && !walkSoundIsPlaying)
		{
			walkSound.Stop();
			walkSoundIsPlaying = true;
			return;
		}
		else if (!isMoving && walkSoundIsPlaying)
		{
			walkSound.Stop();
			walkSoundIsPlaying = false;
			return;
		}

	}

	void LowEnergyEffect()
	{
		vg.intensity.value = Mathf.Abs(Mathf.Sin(Time.realtimeSinceStartup * 3f)) * 0.18f + 0.38f;
	}
	void ClearVignette()
	{
		vg.intensity.value = 0f;
	}

	bool isPulling;
	void UpdateMovement()
	{
		float xInput = Input.GetAxisRaw("Horizontal");
		float yInput = Input.GetAxisRaw("Vertical");

		Vector2 direction = new Vector2(xInput, yInput).normalized;

		isMoving = direction != Vector2.zero;
		animator.SetBool("IsMoving", isMoving);

		if (isSprinting)
		{
			speed = normalSpeed * 1.5f;
		}
		else
		{
			speed = normalSpeed;
		}

		if (Input.GetKey(KeyCode.P) && (pushable.position - transform.position).sqrMagnitude < 1f)
		{
			print("pulling");
			isPulling = true;
			pushableRigidbody.velocity = speed * direction;
			//pushable.parent = transform;
		}
		else
		{
			isPulling = false;
		}

		//if (!isPulling)
		//{
		//	if (Input.GetKeyDown(KeyCode.P) && (pushable.position - transform.position).sqrMagnitude < 1f)
		//	{
		//		isPulling = true;
		//		pushableRigidbody.bodyType = RigidbodyType2D.Kinematic;
		//		pushable.parent = transform;
		//	}
		//}
		//else
		//{
		//	if (Input.GetKeyUp(KeyCode.P))
		//	{
		//		isPulling = false;
		//		pushableRigidbody.bodyType = RigidbodyType2D.Dynamic;
		//		pushable.parent = null;
		//	}
		//}


		rigidBody.velocity = speed * direction;

		if (isMoving)
		{
			if (justStaredMoving)
			{
				if (Time.time - timeSinceLastMoved <= timeToSprint)
				{
					isSprinting = true;
				}
				if (energy < lowEnergyAmount)
				{
					isSprinting = false;
				}
				timeSinceLastMoved = Time.time;

				justStaredMoving = false;
			}


			var shape = moveParticles.shape;
			shape.rotation = Vector3.forward * -(Mathf.Rad2Deg * Mathf.Atan2(direction.x, direction.y) + 90f);

			float energyDecreaseMultiplier = 1f;
			if (isTouchingPushable || isPulling)
			{
				energyDecreaseMultiplier *= 2f;
			}

			if (isSprinting)
			{
				energyDecreaseMultiplier *= 2.5f;
			}

			energy -= Time.deltaTime * energyDecreaseMultiplier;
		}
		else
		{
			justStaredMoving = true;
			isSprinting = false;
		}

	}

	void CheckEnergy()
	{
		if (energy < 0f)
		{
			Explode(dieWaitTime);
			GameOver();
		}
	}

	void UpdateCameraFov()
	{
		if (isSprinting)
		{
			mainCameraScript.SetFov(normalFov + 1f);
		}
		else
		{
			mainCameraScript.SetFov(normalFov);
		}
	}

	IEnumerator Explode(float waitTime)
	{
		explosionParticleSystem.Play();
		yield return new WaitForSeconds(waitTime);
	}

	public float AddEnergy(float energyAmount)
	{
		energy += energyAmount;
		if (energy > 100f)
		{
			energy = 100f;
		}

		return energy;
	}

	public float SetEnergy(float energyAmount)
	{
		energy = energyAmount;
		energy = Mathf.Clamp(energy, 0f, 100f);
		return energy;
	}

	void GameOver()
	{
		ClearVignette();
		ClearVignette();
		gameOverText.gameObject.SetActive(true);
		gameOverText2.gameObject.SetActive(true);
		gameManager.DisablePlayerScript();
	}

	void YouWin()
	{
		ClearVignette();
		youWinText.gameObject.SetActive(true);
		gameManager.DisablePlayerScript();
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Pushable"))
		{
			if (isTouchingPushable == false)
			{
				pushable = collision.transform;
				pushableRigidbody = pushable.GetComponent<Rigidbody2D>();

			}
			isTouchingPushable = true;
		}
		else if (collision.gameObject.CompareTag("Door Key"))
		{
			collision.gameObject.SetActive(false);
			hasDoorKey = true;
		}
		else if (collision.gameObject.CompareTag("Chest Key"))
		{
			collision.gameObject.SetActive(false);
			hasChestKey = true;
		}
		else if (hasDoorKey && collision.gameObject.CompareTag("Door"))
		{
			collision.gameObject.GetComponent<DoorManager>().SwitchDoor();
		}
		else if(hasChestKey && collision.gameObject.CompareTag("Chest"))
		{
			collision.gameObject.GetComponent<DoorManager>().SwitchDoor();
			YouWin();

		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		isTouchingPushable = false; 
	}
}
