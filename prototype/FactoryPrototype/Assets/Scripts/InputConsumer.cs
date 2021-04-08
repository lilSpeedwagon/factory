using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputConsumer : MonoBehaviour, IInput, IMover
{
    public float ConveyerHeight = Conveyer.ConveyerHeight;

    // IMover
    public IMover Next => null;

    public float Height => ConveyerHeight;

    public void Move(MotionScript motionObject)
    {
        throw new NotImplementedException();
    }

    public bool IsAbleToMove()
    {
        return IsReadyToConsume;
    }

    public bool IsDirectionAllowed(TileUtils.Direction direction)
    {
        return GetComponent<TileObject>().Direction == direction;
    }

    public bool IsFree()
    {
        return m_currentMotion == null;
    }

    public void HoldMotion(MotionScript obj)
    {
        m_currentMotion = obj;
    }

    // IMover end

    // IConsumer
    public bool IsReadyToConsume => true;

    public void Consume(MotionScript obj)
    {
        Material material = obj.GetComponent<Material>();
        if (material != null)
        {
            StorageManager.Instance.StoreMaterial(material.Name, 1);
        }
        else
        {
            Debug.LogWarning("Material component not found for consumable.");
        }

        Destroy(obj.gameObject);
    }
    // IConsumer end

    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        if ((m_currentMotion?.IsFinished ?? false) && IsReadyToConsume)
        {
            Consume(m_currentMotion);
            m_currentMotion = null;
        }
    }

    private void OnDestroy()
    {
        if (m_currentMotion != null)
            Destroy(m_currentMotion.gameObject);
    }

    private MotionScript m_currentMotion;
}
