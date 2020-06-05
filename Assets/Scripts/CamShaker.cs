using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShaker : MonoBehaviour
{
    private Vector3 _originalPos;
    private float _elapsedTime;


	public IEnumerator Shake(float duration, float magnitude)
    {
        // orig pos of cam
        _originalPos = transform.localPosition; 

        // time elapsed since shaking began
        _elapsedTime = 0.0f;

        // keep shaking ...
        // time elasped since shaking began doesn't exceed duration of shake; keep shaking
        while (_elapsedTime < duration)
        {
            float x = Random.Range(-1.0f, 1.0f) * magnitude;
            float y = Random.Range(-1.0f, 1.0f) * magnitude;

            transform.localPosition = new Vector3(x, y, _originalPos.z);

            _elapsedTime += Time.deltaTime;

            // wait for a frame before con't to next iteration of while loop; draws one frame.
            yield return null; 
        }

        transform.localPosition = _originalPos;
    }
}
