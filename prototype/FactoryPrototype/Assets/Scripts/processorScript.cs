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
        if (!CanProcess(processableObj.Type))
            throw new System.InvalidOperationException("cannot process the material of type " + processableObj.Type.ToString());
        
        Destroy(processableObj.gameObject);

        try
        {
            TimeUtils.Delay(RecipeManager.ProcessingTime(processableObj, Type)); // coroutine delay  
        }
        catch (Exception e) { Debug.LogWarning(e.Message); }
        
        GameObject newInstance = Instantiate(RecipeManager.FindPrefab(Type, processableObj.Type), GetPosition(), TileUtils.qInitRotation);

        try
        {
            var obj = newInstance.GetComponent<MotionScript>();
            if (obj != null)
            {
                m_currentObject = obj;
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
        return RecipeManager.CanBeProcessed(Type, type);
    }
    // IProcessor implementation end

    // IMover implementation

    public IMover Next => TileManagerScript.TileManager.GetGameObject(GetNextPostion())?.GetComponent<IMover>();

    public void Move(MotionScript motionObject)
    {
        if (motionObject.IsFinished && IsAbleToMove())
        {
            m_currentObject = null;
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
        return m_currentObject == null;
    }

    public void HoldMotion(MotionScript obj)
    {
        m_currentObject = obj;
    }
    // IMover implementation end

    // Start is called before the first frame update
    void Start()
    {
        RecipeManager = PlayerUtils.Player.GetComponent<RecipeManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var collisions = new List<Collider2D>();
        var collider = GetComponent<PolygonCollider2D>();
        collider.OverlapCollider(m_filter, collisions);

        foreach (var collision in collisions)
        {
            if (collision.gameObject.tag.Equals("detail"))
            {
                if (collision.GetComponent<IProcessable>() != null && collision.GetComponent<MotionScript>().IsFinished)
                {
                    try
                    {
                        Process(collision.GetComponent<IProcessable>());
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(e.Message);
                    }
                }
            }
        }

        if (m_currentObject != null)
        {
            Move(m_currentObject);
        }
    }

    // current movable object on belt
    private MotionScript m_currentObject;
    static RecipeManager RecipeManager;
}
