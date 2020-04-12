using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputProducer : MonoBehaviour, IOutput
{
    public float Speed = 0.2f;
    public GameObject PrefabToEmit;
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
    public bool IsReadyToEmit => m_currentMotion == null;

    public void Emit()
    {
        var obj = Instantiate(PrefabToEmit, m_tile.GetPosition(), TileUtils.qInitRotation);
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
    }

    private void FixedUpdate()
    {
        if (GetComponent<tileObjectScript>().isShadow) return;

        if (m_currentMotion != null)
            Move(m_currentMotion);
    }

    private void OnDestroy()
    {
        if (m_currentMotion != null)
            Destroy(m_currentMotion.gameObject);
    }

    private tileObjectScript m_tile;
    private MotionScript m_currentMotion;
}
