using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class processorScript : tileObjectScript, IProcessor
{
    public ProcessorType Type;
    public float ConveerSpeed = 0.2f;
    public float ProcessTriggerOffset = 0.3f;

    // IProcessor implementation
    public GameObject Process(IProcessable obj)
    {
        if (!CanProcess(obj.Type))
            throw new System.InvalidOperationException("cannot process the material of type " + obj.Type.ToString());
        
        Destroy(obj.gameObject);
        TimeUtils.Delay(ProcessingTimeSec); // coroutine delay  
        GameObject newInstance = Instantiate(RecipeManager.FindPrefab(Type, obj.Type), transform.position, TileUtils.qInitRotation);

        try
        {
            var motion = newInstance.GetComponent<MotionScript>();
            if (motion.IsFinished)
            {
                Vector2 target = GetVector() * TileUtils.tileSize + (Vector2)transform.position;
                motion.StartMotion(target, ConveerSpeed);
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
                    Vector2 pos = collision.transform.position;
                    var obj = collision.GetComponent<IProcessable>();
                    Vector2 input = (Vector2)transform.position - GetVector() * TileUtils.tileSize / 2;
                    if (obj != null && Vector2.Distance(pos, input) <= ProcessTriggerOffset)
                    {                  
                        Process(obj);
                    }
                    else
                    {
                        Debug.LogWarning("material without IProcessable detected and cannot be processed");
                    }
                }
            }
        }
    }

    static RecipeManager RecipeManager;
}
