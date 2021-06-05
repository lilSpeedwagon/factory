using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputProducer : MonoBehaviour, IOutput
{
    public float ConveyerHeight = Conveyer.ConveyerHeight;
    public float Speed = 0.2f;
    public float DelayBetweenEmissions = 0.5f;

    // IMover
    public IMover Next => TileManagerScript.TileManager.GetGameObject(m_tile.GetNextPosition())?.GetComponent<IMover>();

    public float Height => ConveyerHeight;

    public MotionScript Motion => m_currentMotion;
    public MotionScript ReleaseMotion()
    {
        return null;
    }

    public void Move(MotionScript motionObject)
    {
        if (motionObject.IsFinished && IsAbleToMove())
        {
            m_currentMotion = null;
            var toPosition = m_tile.GetNextPosition();
            toPosition.y += Next.Height;
            motionObject.StartMotion(toPosition, Speed);
            Next.HoldMotion(motionObject);
        }
    }

    public bool IsAbleToMove()
    {
        var mover = Next;
        return mover != null && mover.IsDirectionAllowed(m_tile.Direction) && mover.IsFree();
    }

    public bool IsDirectionAllowed(TileUtils.Direction direction)
    {
        return false;
    }

    public bool IsFree()
    {
        return m_currentMotion == null;
    }

    public void HoldMotion(MotionScript obj)
    {
        throw new System.NotImplementedException();
    }
    // end IMover

    // IOutput
    public string MaterialToEmit
    {
        get => m_materialToEmit != null ? m_materialToEmit.Name : "";
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                m_isEnabled = false;
                m_materialToEmit = null;
                return;
            }

            if (!MaterialInfoHolder.Instance.Exist(value))
            {
                throw new KeyNotFoundException($"Cannot emit '{value}'. Material is unknown.");
            }

            m_materialToEmit = MaterialInfoHolder.Instance.GetMaterialPrefab(value);
            m_isEnabled = true;
        }
    }

    public bool IsReadyToEmit => m_currentMotion == null && m_isEnabled;

    public void Emit()
    {
        if (m_materialToEmit == null || !StorageManager.Instance.RemoveMaterial(m_materialToEmit.Name, 1))
        {
            Debug.LogWarning($"Cannot emit '{m_materialToEmit}'");
            m_isEnabled = false;
            return;
        }

        var position = m_tile.GetPosition();
        position.y += ConveyerHeight;

        var obj = Instantiate(m_materialToEmit.gameObject, position, TileUtils.InitRotation);
        m_currentMotion = obj.GetComponent<MotionScript>();
        Move(m_currentMotion);
    }
    // end IOutput

    private void EmitInternal()
    {
        if (IsReadyToEmit)
            Emit();
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_tile = GetComponent<TileObject>();
        InvokeRepeating("EmitInternal", DelayBetweenEmissions, DelayBetweenEmissions);
        m_isEnabled = false;
    }

    private void FixedUpdate()
    {
        if (GetComponent<TileObject>().IsShadow) return;

        if (m_currentMotion != null)
            Move(m_currentMotion);
    }

    private void OnMouseDown()
    {
        if (!MenuManager.Manager.IsNonDefaultActive)
        {
            ProducerMenu.Instance.ShowFor(this);
        }
    }

    private void OnDestroy()
    {
        if (m_currentMotion != null)
            Destroy(m_currentMotion.gameObject);
    }

    private bool m_isEnabled;
    private Material m_materialToEmit;
    private TileObject m_tile;
    private MotionScript m_currentMotion;
}
