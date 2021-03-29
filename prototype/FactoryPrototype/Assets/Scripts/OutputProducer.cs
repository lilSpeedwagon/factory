using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputProducer : MonoBehaviour, IOutput
{
    public float Speed = 0.2f;
    public float DelayBetweenEmissions = 0.5f;

    // IMover
    public void Move(MotionScript motionObject)
    {
        if (motionObject.IsFinished && IsAbleToMove())
        {
            m_currentMotion = null;
            motionObject.StartMotion(m_tile.GetNextPostion(), Speed);
            Next.HoldMotion(motionObject);
        }
    }

    public bool IsAbleToMove()
    {
        var mover = Next;
        return mover != null && mover.IsDirectionAllowed(m_tile.direction) && mover.IsFree();
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

        var obj = Instantiate(m_materialToEmit.gameObject, m_tile.GetPosition(), TileUtils.InitRotation);
        m_currentMotion = obj.GetComponent<MotionScript>(); // could be null
        Move(m_currentMotion);
    }
    // end IOutput

    public IMover Next => TileManagerScript.TileManager.GetGameObject(m_tile.GetNextPostion())?.GetComponent<IMover>();

    private void EmitInternal()
    {
        if (IsReadyToEmit)
            Emit();
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_tile = GetComponent<tileObjectScript>();
        InvokeRepeating("EmitInternal", DelayBetweenEmissions, DelayBetweenEmissions);
        m_isEnabled = false;
    }

    private void FixedUpdate()
    {
        if (GetComponent<tileObjectScript>().isShadow) return;

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
    private tileObjectScript m_tile;
    private MotionScript m_currentMotion;
}
