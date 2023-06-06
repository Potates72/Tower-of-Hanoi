using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : Selectable
{
    public List<Ring> stack;
    public Transform topPos;
    public Transform botPos;

    private void Update()
    {
        if (stack.Count == 0) return;

        stack.ForEach(x => x.transform.position = new Vector3(botPos.position.x,
            x.transform.position.y,
            botPos.transform.position.z));
    }

    public void TakeRing(Ring ring, Transform targetPos)
    {
        StartCoroutine(ring.SelectPiece(targetPos.position));
        stack.Remove(ring);
        ring.transform.parent = targetPos;
    }

    public void PlaceRing(Ring ring)
    {
        StartCoroutine(ring.DropPiece(topPos.position));
        stack.Add(ring);
        ring.transform.parent = this.transform;
    }
}
