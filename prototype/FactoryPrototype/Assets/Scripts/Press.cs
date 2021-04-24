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

    private void PressDown()
    {
        m_state = PressState.Down;
        m_animator.SetBool("pressDown", true);

        TryProcessMaterial();
    }

    private void PressUp()
    {
        m_state = PressState.Up;
        m_animator.SetBool("pressDown", false);
    }

    private void TryProcessMaterial()
    {
        if (!IsActive) return;
        
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
        switch (m_state)
        {
            case PressState.Down:
                PressUp();
                break;
            case PressState.Up:
                PressDown();
                break;
        }
    }

    private void Start()
    {
        m_logger = new LogUtils.DebugLogger("Press");

        m_state = PressState.Up;
        m_consumer = GetComponent<EnergyConsumer>();
        m_animator = GetComponent<Animator>();
    }
    
    private void FixedUpdate()
    {
        
    }

    private enum PressState
    {
        Up,
        Down
    }

    private PressState m_state;
    private MotionScript m_currentMotion;

    private EnergyConsumer m_consumer;
    private Animator m_animator;

    private LogUtils.DebugLogger m_logger;
}
