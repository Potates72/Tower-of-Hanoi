using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : Selectable
{
    public int RingOrder
    {
        get { return ringOrder; }
        set { ringOrder = value; }
    }

    [SerializeField] private AudioClip selectSFX;
    [SerializeField] private List<AudioClip> dropSFX;
    [SerializeField] private AudioClip unselectSFX;

    private int ringOrder;

    private Rigidbody rb;
    private Collider col;
    private bool wasDropped = false;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        col = this.GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision colission)
    {
        GameObject other = colission.gameObject;

        if (other.CompareTag("Platform") || other.CompareTag("Ring"))
        {
            if (wasDropped)
            {
                AudioManager.Instance.PlayAudio(dropSFX[Random.Range(0, dropSFX.Count)]);
                wasDropped = false;
            }

            rb.velocity = Vector3.zero;
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public IEnumerator SelectPiece(Vector3 targetPosition, float duration = 0.1f)
    {
        col.enabled = false;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        yield return AnimateObject.LerpTransform(this.transform, targetPosition, duration);
        //rb.velocity = Vector3.zero;
        AudioManager.Instance.PlayAudio(selectSFX);
    }

    public IEnumerator DropPiece(Vector3 targetPosition, float duration = 0.1f)
    {
        wasDropped = true;
        yield return AnimateObject.LerpTransform(this.transform, targetPosition, duration);
        //rb.constraints = RigidbodyConstraints.FreezeRotation;
        //rb.velocity = Vector3.zero;
        AudioManager.Instance.PlayAudio(unselectSFX);
        rb.useGravity = true;
        col.enabled = true;
    }
}
