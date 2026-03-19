using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BoardLane : MonoBehaviour
{
    public List<BoardSlot> slots = new List<BoardSlot>();
    public SlotOwner owner;
    void Awake()
    {
        slots.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            var slot = transform.GetChild(i).GetComponent<BoardSlot>();
            if (slot != null)
            {
                slot.line = this;
                slot.indexInLine = i;
                slots.Add(slot);
            }
        }
    }
    public bool IsFull()
    {
        int count = 0;
        foreach (var slot in slots)
        {
            if (slot.transform.childCount > 0)
                count++;
        }
        return count >= slots.Count;
    }
    public void HandleUnitDeath(int deadIndex)
    {
        StartCoroutine(HandleDeathRoutine());
    }
    IEnumerator HandleDeathRoutine()
    {
        yield return null;
        CompactLine();
    }
    public void CompactLine()
    {
        List<CardView> cards = new List<CardView>();
        foreach (var slot in slots)
        {
            if (slot.transform.childCount > 0)
            {
                var card = slot.transform.GetChild(0).GetComponent<CardView>();
                if (card != null && card.owner == SlotOwner.Player)
                    cards.Add(card);
            }
            slot.occupied = false;
        }
        int playerIndex = 0;

        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];

            if (slot.transform.childCount > 0)
            {
                var existing = slot.transform.GetChild(0).GetComponent<CardView>();
                if (existing != null && existing.owner == SlotOwner.Enemy)
                {
                    existing.CurrentSlot = slot;
                    slot.occupied = true;
                    continue;
                }
            }
            if (playerIndex >= cards.Count)
                continue;

            var card = cards[playerIndex++];
            card.CurrentSlot = slot;
            slot.occupied = true;

            var rt = card.GetComponent<RectTransform>();
            rt.SetParent(slot.transform);
            rt.localPosition = Vector3.zero;
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;
        }
    }
    public void Collapse()
    {
        List<CardView> cards = new List<CardView>();
        foreach (var slot in slots)
        {
            if (slot.transform.childCount > 0)
            {
                var card = slot.transform.GetChild(0).GetComponent<CardView>();
                if (card != null)
                    cards.Add(card);
            }
            slot.occupied = false;
        }

        for (int i = 0; i < cards.Count; i++)
        {
            var targetSlot = slots[i];
            var card = cards[i];

            card.CurrentSlot = targetSlot;
            targetSlot.occupied = true;

            var rt = card.GetComponent<RectTransform>();
            rt.SetParent(targetSlot.transform, false);
            rt.anchoredPosition = Vector2.zero;
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;
        }
    }
    public int GetInsertIndex(Vector3 worldPos)
    {
        int closestIndex = 0;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < slots.Count; i++)
        {
            float dist = Mathf.Abs(worldPos.x - slots[i].transform.position.x);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestIndex = i;
            }
        }
        return closestIndex;
    }
    bool ContainsCard(CardView card)
    {
        foreach (var slot in slots)
        {
            if (slot.transform.childCount > 0)
            {
                var c = slot.transform.GetChild(0).GetComponent<CardView>();
                if (c == card) return true;
            }
        }
        return false;
    }
    public void InsertCard(CardView card, int index)
    {
        if (card.owner == SlotOwner.Enemy)
            return;

        List<CardView> cards = new List<CardView>();
        foreach (var slot in slots)
        {
            if (slot.transform.childCount > 0)
            {
                var c = slot.transform.GetChild(0).GetComponent<CardView>();
                if (c != null && c != card && c.owner == SlotOwner.Player)
                    cards.Add(c);
            }
            slot.occupied = false;
        }

        index = Mathf.Clamp(index, 0, cards.Count);
        cards.Insert(index, card);
        int playerIndex = 0;

        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            if (slot.transform.childCount > 0)
            {
                var existing = slot.transform.GetChild(0).GetComponent<CardView>();
                if (existing != null && existing.owner == SlotOwner.Enemy)
                {
                    existing.CurrentSlot = slot;
                    slot.occupied = true;
                    continue;
                }
            }
            if (playerIndex >= cards.Count)
                continue;

            var c = cards[playerIndex++];
            c.CurrentSlot = slot;
            slot.occupied = true;

            var rt = c.GetComponent<RectTransform>();
            rt.SetParent(slot.transform, false);
            rt.anchoredPosition = Vector2.zero;
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;
        }
    }
    public int GetCardIndex(CardView card)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].transform.childCount > 0)
            {
                var cv = slots[i].transform.GetChild(0).GetComponent<CardView>();
                if (cv == card)
                    return i;
            }
        }
        return -1;
    }
    public BoardSlot GetFirstFreeSlot()
    {
        foreach (var slot in slots)
        {
            if (!slot.occupied)
                return slot;
        }
        return null;
    }
    public Unit GetFrontUnit()
    {
        foreach (var slot in slots)
        {
            if (slot.occupied)
                return slot.GetComponentInChildren<Unit>();
        }
        return null;
    }
}
