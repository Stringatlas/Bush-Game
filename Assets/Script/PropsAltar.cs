using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PropsAltar : MonoBehaviour
{
    public List<SpriteRenderer> runes;
    public float lerpSpeed;

    private Color curColor;
    private Color targetColor;

    bool hasFinishedChangingColor;

    [SerializeField] Transform mainCam;
    CameraFollow mainCamScript;

    [SerializeField] Transform player;
    [SerializeField] Transform chestKey;

    [SerializeField] float showKeyWaitTime;
    [SerializeField] float showKeyForTime;

	private void Start()
	{
        chestKey.gameObject.SetActive(false);
        mainCamScript = mainCam.GetComponent<CameraFollow>();
	}

	private void OnTriggerEnter2D(Collider2D other)
    {
        targetColor = new Color(1, 1, 1, 1);
        hasFinishedChangingColor = false;
        chestKey.gameObject.SetActive(true);
        StartCoroutine(ShowKey(true));

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        targetColor = new Color(1, 1, 1, 0);
        hasFinishedChangingColor = false;
        chestKey.gameObject.SetActive(false);
        StartCoroutine(ShowKey(false));
    }

    private void Update()
    {
		if (!hasFinishedChangingColor)
		{
            ChangeColor();
		}
    }

    IEnumerator ShowKey(bool active)
	{
        yield return new WaitForSeconds(showKeyWaitTime);
        mainCamScript.target = chestKey;


          chestKey.gameObject.SetActive(active);


        yield return new WaitForSeconds(showKeyForTime);
        mainCamScript.target = player;
	}


    void ChangeColor()
	{
        curColor = Color.Lerp(curColor, targetColor, lerpSpeed * Time.deltaTime);

        foreach (var r in runes)
        {
            r.color = curColor;
        }

		if (curColor == targetColor)
		{
            hasFinishedChangingColor = true;
		}
    }

        
}

