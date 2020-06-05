using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShaker : MonoBehaviour
{
	public IEnumerator Shake(float duration, float magnitude)
    {
        // orig pos of cam
        Vector3 originalPos = transform.localPosition; 

        // time elapsed since shaking began
        float elapsedTime = 0.0f;

        // keep shaking ...
        // time elasped since shaking began doesn't exceed duration of shake; keep shaking
        while (elapsedTime < duration)
        {
            float x = Random.Range(-1.0f, 1.0f) * magnitude;
            float y = Random.Range(-1.0f, 1.0f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsedTime += Time.deltaTime;

            yield return null; // before con't to next iteration of while loop draw one frame.
        }

        transform.localPosition = originalPos;
    }
}
