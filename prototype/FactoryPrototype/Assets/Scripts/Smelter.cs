using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Smelter : MonoBehaviour, IProcessor, IMover
{
    public float ConveyerHeight = Conveyer.ConveyerHeight;
    public float ConveyerSpeed = 0.6f;
    public float ProcessTriggerOffset = 0.3f;

    public bool IsActive => m_consumerComponent?.IsEnergized ?? false;

    // IProcessor implementation
    public string Name => "smelter";

    public Material Process(Material processable)
    {
        Recipe recipe = RecipeManager.Instance.FindRecipe(processable.Name, Name);
        if (recipe == null)
        {
            m_logger.Log($"Processor: cannot process {processable.Name} via {Name}.");
            return null;
        }

        bool isHeatedEnough = recipe.Temperature <= processable.Temperature;
        Material toMaterial = MaterialInfoHolder.Instance.GetMaterialPrefab(recipe.To);
        bool makeTrash = !isHeatedEnough || toMaterial == null;
        var temp = processable.Temperature;

        Destroy(processable.gameObject);

        if (makeTrash)
        {
            m_logger.Log("Material is not meet recipe conditions. Making trash.");
            toMaterial = MaterialInfoHolder.Instance.GetTrashPrefab();
        }

        var position = m_tileComponent.GetPosition();
        position.y += Height;
        GameObject newInstance = Instantiate(toMaterial.gameObject, position, TileUtils.InitRotation);
        var material = newInstance.GetComponent<Material>();
        //material.Temperature = temp;
        return material;
    }

    public bool CanProcess(string material)
    {
        return RecipeManager.Instance.CanBeProcessed(material, Name);
    }
    // IProcessor implementation end

    // IMover implementation
    public MotionScript Motion => m_currentMotion;
    public IMover Next => TileManagerScript.TileManager.GetGameObject(m_tileComponent.GetNextPosition())?.GetComponent<IMover>();

    public float Height => ConveyerHeight;

    public MotionScript ReleaseMotion()
    {
        return null;
    }

    public void Move(MotionScript motionObject)
    {
        if (motionObject.IsFinished && IsAbleToMove())
        {
            m_currentMotion = null;
            motionObject.StartMotion(m_tileComponent.GetNextPosition() + new Vector2(0.0f, Next.Height), ConveyerSpeed);
            Next?.HoldMotion(motionObject);
        }
    }

    public bool IsAbleToMove()
    {
        var mover = Next;
        return mover != null && mover.IsDirectionAllowed(m_tileComponent.Direction) && mover.IsFree();
    }

    public bool IsDirectionAllowed(TileUtils.Direction direction)
    {
        return direction == m_tileComponent.Direction;
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

    private void Start()
    {
        m_logger = new LogUtils.DebugLogger("Smelter");
        m_consumerComponent = GetComponent<EnergyConsumer>();
        m_tileComponent = GetComponent<TileObject>();
    }
    
    private void FixedUpdate()
    {
        if (IsActive && m_currentObjectToProcess != null && m_currentObjectToProcess.IsFinished)
        {
            var material = m_currentObjectToProcess.GetComponent<Material>();
            if (material != null)
            {
                var result = Process(material);
                if (result != null)
                {
                    m_currentMotion = result.GetComponent<MotionScript>();
                    Move(m_currentMotion);
                }
            }
            m_currentObjectToProcess = null;
        }

        if (m_currentMotion != null)
        {
            Move(m_currentMotion);
        }
    }
    
    private MotionScript m_currentMotion;
    private MotionScript m_currentObjectToProcess;
    private EnergyConsumer m_consumerComponent;
    private TileObject m_tileComponent;

    private LogUtils.DebugLogger m_logger;
}
