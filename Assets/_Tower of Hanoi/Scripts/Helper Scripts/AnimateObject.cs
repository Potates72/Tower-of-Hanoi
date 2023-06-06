using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimateObject
{
    public static bool isAnimating;

    public static IEnumerator LerpTransform(Transform objectToMove, Vector3 targetPosition, float duration)
    {
        float currTime = 0;
        float perc = 0;

        Vector3 initialPos = objectToMove.position;

        while (perc < 1)
        {
            currTime += Time.deltaTime;
            perc = currTime / duration;

            perc = Mathf.Clamp(perc, 0, 1);

            objectToMove.position = Vector3.Lerp(initialPos, targetPosition, perc);
            yield return new WaitForEndOfFrame();
        }
    }

    public static void Shake(Ring objectToShake, float shakeDuration, float shakeIntensity, float returnDuration)
    {
        if (isAnimating) return;

        objectToShake.StartCoroutine(AnimateObject.ShakeObject(objectToShake.transform, shakeDuration, shakeIntensity, returnDuration));
    }

    private static IEnumerator ShakeObject(Transform objectToShake, float shakeDuration, float shakeIntensity, float returnDuration)
    {
        isAnimating = true;
        Vector3 originalPosition = objectToShake.position;
        float timer = shakeDuration;

        while (timer > 0f)
        {
            objectToShake.position = originalPosition + Random.insideUnitSphere * shakeIntensity;

            timer -= Time.deltaTime;
            yield return null;
        }

        timer = returnDuration;

        while (timer > 0f)
        {
            float t = 1f - (timer / returnDuration);
            objectToShake.position = Vector3.Lerp(objectToShake.position, originalPosition, t);

            timer -= Time.deltaTime;
            yield return null;
        }

        objectToShake.position = originalPosition;
        isAnimating = false;
    }
}

