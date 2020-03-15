using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class processorScript : tileObjectScript, IProcessor, IMover
{
    public ProcessorType Type;
    public float ConveerSpeed = 0.2f;
    public float ProcessTriggerOffset = 0.3f;

    // IProcessor implementation
    public GameObject Process(IProcessable processableObj)
    {
        if (!CanProcess(processableObj.Type))
            throw new System.InvalidOperationException("cannot process the material of type " + processableObj.Type.ToString());
        
        Destroy(processableObj.gameObject);
        TimeUtils.Delay(ProcessingTimeSec); // coroutine delay  
        GameObject newInstance = Instantiate(RecipeManager.FindPrefab(Type, processableObj.Type), GetPosition(), TileUtils.qInitRotation);

        try
        {
            var obj = newInstance.GetComponent<MotionScript>();
            if (obj != null)
                Move(obj);
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
    public void Move(MotionScript motionObject)
    {
        if (motionObject.IsFinished)
        {   
            motionObject.StartMotion(GetNextPostion(), ConveerSpeed);
        }
    }

    public bool IsAbleToMove()
    {
        throw new System.NotImplementedException();
    }

    public bool IsDirectionAllowed(TileUtils.Direction direction)
    {
        return direction == m_dir;
    }
    // IMover implementation end

    public float ProcessingTimeSec;

    // Start is called before the first frame update
    void Start()
    {
        RecipeManager = PlayerUtils.Player.GetComponent<RecipeManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isShadow)
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
                        Process(collision.GetComponent<IProcessable>());
                    }
                }
            }
        }
    }

    static RecipeManager RecipeManager;
}
