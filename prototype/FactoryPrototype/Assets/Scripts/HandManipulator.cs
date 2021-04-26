using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HandManipulator : MonoBehaviour
{
    public float Speed;

    public bool IsActive => GetComponent<EnergyConsumer>().IsEnergized;

    public GameObject PickupObject => TileManagerScript.TileManager
        .GetGameObject(GetComponent<TileObject>().GetPrevPosition());

    public GameObject GetterObject => TileManagerScript.TileManager
        .GetGameObject(GetComponent<TileObject>().GetNextPosition());

    private void OnPickup()
    {
        var source = PickupObject;
        if (source != null)
        {
            var materialMotion = source.GetComponent<IMover>()?.ReleaseMotion();
            if (materialMotion != null)
            {
                materialMotion.Stop();
                materialMotion.StartMotion(GetMaterialTrajectory(), Speed);
                m_currentMotion = materialMotion;
            }
        }
    }

    private List<Vector2> GetMaterialTrajectory()
    {
        var handPosition = GetComponent<TileObject>().GetPosition();

        const int pointsCount = 6;
        List<Vector2> points = new List<Vector2>(pointsCount)
        {
            handPosition + new Vector2(-0.3f, 0.3f),
            handPosition + new Vector2(-0.1f, 0.36f),
            handPosition + new Vector2(0.13f, 0.29f),
            handPosition + new Vector2(0.35f, 0.1f),
            handPosition + new Vector2(0.48f, -0.16f)
        };

        return points;
    }

    private void Update()
    {
        if (m_currentMotion != null)
        {
            if (m_currentMotion.IsFinished)
            {
                var getter = GetterObject;
                IMover getterMover = getter.GetComponent<IMover>();
                if (getterMover?.IsFree() ?? false)
                {
                    getterMover.HoldMotion(m_currentMotion);
                    m_currentMotion = null;
                }
            }
        }
    }

    private MotionScript m_currentMotion;
}
