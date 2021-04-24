using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Press : MonoBehaviour, IProcessor, IMover
{
    public float ConveyerHeight = Conveyer.ConveyerHeight;
    public bool IsActive => m_consumer.IsEnergized;

    // IProcessor implementation
    public string Name => "press";

    public bool CanProcess(string material)
    {
        return RecipeManager.Instance.CanBeProcessed(material, Name);
    }

    public Material Process(Material material)
    {
        Material toMaterial = null;

        Recipe recipe = RecipeManager.Instance.FindRecipe(material.Name, Name);
        if (recipe != null)
        {
            toMaterial = MaterialInfoHolder.Instance.GetMaterialPrefab(recipe.To);
        }

        if (toMaterial == null)
        {
            Debug.Log($"cannot process {material.Name} via {Name}, making trash");
            return null;
            // TODO make a trash
            //toMaterial = MaterialInfoHolder.Instance.GetMaterialPrefab("trash");
        }
        
        var position = GetComponent<TileObject>().GetPosition();
        position.y += Height;
        GameObject newInstance = Instantiate(toMaterial.gameObject, position, TileUtils.InitRotation);
        m_currentMotion = newInstance.GetComponent<MotionScript>();

        return newInstance.GetComponent<Material>();
    }
    // IProcessor end

    // IMover
    public IMover Next => null;

    public float Height => ConveyerHeight;

    public void Move(MotionScript motionObject)
    {
        throw new NotImplementedException();
    }

    public bool IsAbleToMove()
    {
        throw new NotImplementedException();
    }

    public bool IsDirectionAllowed(TileUtils.Direction direction)
    {
        return true;
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

    private void FireAnimation()
    {
        const string trigger = "pressTrigger";
        m_animator.ResetTrigger(trigger);
        m_animator.SetTrigger(trigger);
    }

    private void TryProcessMaterial()
    {
        if (m_currentMotion?.IsFinished ?? false)
        {
            var material = m_currentMotion.GetComponent<Material>();
            if (material == null)
            {
                m_logger.Warn("current motion has no Material component");
                return;
            }

            Process(material);
        }
    }

    private void OnMouseDown()
    {
        if (IsActive)
        {
            FireAnimation();
            TryProcessMaterial();
        }
    }

    private void Start()
    {
        m_logger = new LogUtils.DebugLogger("Press");

        m_consumer = GetComponent<EnergyConsumer>();
        m_animator = GetComponent<Animator>();
    }
    
    private void FixedUpdate()
    {
        
    }

    private MotionScript m_currentMotion;

    private EnergyConsumer m_consumer;
    private Animator m_animator;

    private LogUtils.DebugLogger m_logger;
}
