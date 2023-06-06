using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformHandler : MonoBehaviour
{
    [SerializeField] private List<Transform> pillars;
    [SerializeField] private float animationDuration;
    [SerializeField] private int index;

    private bool isPlatformMoving = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MovePlatforms(-1);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MovePlatforms(1);
        }
    }

    private void MovePlatforms(int dir)
    {
        if (isPlatformMoving) return;

        List<Vector3> positions = GetPillarPositions();

        for (int i = 0; i < pillars.Count; i++)
        {
            int targetIndex = i + dir;
            if (targetIndex < 0) targetIndex = 2;
            if (targetIndex > 2) targetIndex = 0;

            Vector3 nextPos = positions[targetIndex];
            StartCoroutine(ShiftPlatform(pillars[i], nextPos));
        }
    }

    private List<Vector3> GetPillarPositions()
    {
        List<Vector3> positions = new List<Vector3>();

        for (int i = 0; i < pillars.Count; i++)
        {
            positions.Add(pillars[i].position);
        }

        return positions;
    }

    private IEnumerator ShiftPlatform(Transform pillarToMove, Vector3 targetPos)
    {
        isPlatformMoving = true;
        StartCoroutine(AnimateObject.LerpTransform(pillarToMove, targetPos, animationDuration));

        yield return new WaitForSeconds(animationDuration);
        isPlatformMoving = false;
    }
}
