using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private Pillar activePillar;
    [SerializeField] private Ring activeRing;
    [SerializeField] private Ring selectedRing;
    [SerializeField] private Transform holder;
    [SerializeField] private float delayDetectionDuration;
    [SerializeField] private AudioClip cannotDropAudio;

    private Collider col;

    private void Awake()
    {
        col = this.GetComponent<Collider>();
        col.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && activePillar != null)
        {
            if (CanSelectRing())
            {
                SelectPiece();
            }
            else if (CanDropRing())
            {
                DropPiece();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Pillar") == false)
            return;

        activePillar = other.GetComponent<Pillar>();
        activePillar.UseHovermat();

        if (activePillar.stack.Count == 0) return;

        if (selectedRing == null && (activeRing == null || CanSelectRing()))
        {
            activeRing = activePillar.stack[activePillar.stack.Count - 1];
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pillar") == false)
            return;

        activePillar.ResetMaterial();
        activePillar = null;

        if (activeRing == null)
            return;
        activeRing = null;
    }

    private IEnumerator DelayDetectionToggle(bool toggle, float duration = 0)
    {
        yield return new WaitForSeconds(duration);
        col.enabled = toggle;
    }

    public void DelayToggleCollider(bool toggle)
    {
        StartCoroutine(DelayDetectionToggle(toggle, delayDetectionDuration));
    }

    public void ToggleCollider(bool toggle)
    {
        activeRing = null;
        activePillar = null;

        StartCoroutine(DelayDetectionToggle(toggle));
    }

    public void SelectPiece()
    {
        if (activePillar == null)
            return;
        if (selectedRing != null)
            return;
        selectedRing = activeRing;
        activePillar.TakeRing(selectedRing, holder);
    }

    public void DropPiece()
    {
        if (activePillar == null && CanDropRing() == false)
            return;
        activePillar.PlaceRing(selectedRing);
        selectedRing.transform.parent = activePillar.transform;
        selectedRing = null;
    }

    public bool CanSelectRing()
    {
        if (activePillar.stack.Count == 0)
            return false;

        if (activeRing != null && activeRing.RingOrder != activePillar.stack[activePillar.stack.Count - 1].RingOrder)
            return false;

        if (selectedRing != null)
            return false;

        return true;
    }

    public bool CanDropRing()
    {
        if (activePillar.stack.Count == 0)
            return true;

        if (activePillar.stack.Count > 0 && selectedRing != null && activePillar.stack[activePillar.stack.Count - 1].RingOrder < selectedRing.RingOrder)
            return true;

        AnimateObject.Shake(selectedRing, 0.15f, 0.5f, 0.15f);
        AudioManager.Instance.PlayAudio(cannotDropAudio);
        return false;
    }
}
