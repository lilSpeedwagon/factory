using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class processorScript : tileObjectScript, IProcessor, IMover
{
    public float ConveerSpeed = 0.2f;
    public float ProcessTriggerOffset = 0.3f;

    // IProcessor implementation
    public ProcessorType Type;

    public GameObject Process(IProcessable processableObj)
    {
        Destroy(processableObj.gameObject);

        try
        {
            TimeUtils.Delay(RecipeManager.instance.ProcessingTime(processableObj, Type));
        }
        catch (Exception e) { Debug.LogWarning(e.Message); }
        
        GameObject newInstance = Instantiate(RecipeManager.instance.FindPrefab(Type, processableObj.Type), GetPosition(), TileUtils.qInitRotation);

        try
        {
            var obj = newInstance.GetComponent<MotionScript>();
            if (obj != null)
            {
                m_currentMotion = obj;
                Move(obj);
            }
        }
        catch (System.NullReferenceException e)
        {
            Debug.LogError(e.Message);
        }

        return newInstance;
    }

    public bool CanProcess(MaterialType type)
    {     
        return RecipeManager.instance.CanBeProcessed(Type, type);
    }
    // IProcessor implementation end

    // IMover implementation

    public IMover Next => TileManagerScript.TileManager.GetGameObject(GetNextPostion())?.GetComponent<IMover>();

    public void Move(MotionScript motionObject)
    {
        if (motionObject.IsFinished && IsAbleToMove())
        {
            m_currentMotion = null;
            motionObject.StartMotion(GetNextPostion(), ConveerSpeed);
            Next?.HoldMotion(motionObject);
        }
    }

    public bool IsAbleToMove()
    {
        var mover = Next;
        return mover != null && mover.IsDirectionAllowed(m_dir) && mover.IsFree();
    }

    public bool IsDirectionAllowed(TileUtils.Direction direction)
    {
        return direction == m_dir;
    }

    public bool IsFree()
    {
        return m_currentObjectToProcess == null && m_currentMotion == null;
    }

    public void HoldMotion(MotionScript obj)
    {
        m_currentObjectToProcess = obj;
    }
    // IMover implementation end

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (m_currentObjectToProcess != null && m_currentObjectToProcess.IsFinished)
        {
            var proc = m_currentObjectToProcess.GetComponent<IProcessable>();
            if (proc != null && CanProcess(proc.Type))
            {
                Process(proc);
            }
            else
            {
                m_currentMotion = m_currentObjectToProcess;
            }
            m_currentObjectToProcess = null;
        }

        if (m_currentMotion != null)
        {
            Move(m_currentMotion);
        }
    }

    private void OnDestroy()
    {
        if (m_currentMotion != null)
            Destroy(m_currentMotion.gameObject);
        if (m_currentObjectToProcess != null)
            Destroy(m_currentObjectToProcess.gameObject);
    }

    // current movable object on belt
    private MotionScript m_currentObjectToProcess;
    private MotionScript m_currentMotion;
}
