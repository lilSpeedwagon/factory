using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class DataPipeMenu : MonoBehaviour, IMenu
{
    private DataPipeMenu() {}
    private static DataPipeMenu g_instance;
    public static DataPipeMenu Menu => g_instance ?? (g_instance = GameObject.FindWithTag("WiresMenu").GetComponent<DataPipeMenu>());

    public RectTransform PortList;
    public LineRenderer WirePrefab;
    public RectTransform ListItemPrefab;


    public void OnPortSelected(DataPublisher.DataPort port)
    {
        if (m_state == State.SelectPortFrom)
        {
            m_selectedPortFrom = port;
            m_state = State.WirePlacing;
        }
        else if (m_state == State.SelectPortTo)
        {
            m_selectedPortTo = port;
            WirePorts();
            m_line = null; // prevent line destroying
            Reset();
        }

        HidePortList();
    }

    // IMenu implementation
    public void Hide()
    {
        HidePortList();
        Reset();
        cameraScript.SetLayerVisibility(WireLayer, false);

        m_state = State.Inactive;
    }

    public bool IsCameraZoomAllowed()
    {
        return true;
    }

    public void Show()
    {
        MenuManager.Manager.SetActive(this);
        cameraScript.SetLayerVisibility(WireLayer, true);

        m_state = State.Idle;
    }

    public bool IsActive => m_state != State.Inactive;

    // MonoBehavior implementation
    private void Start()
    {
        Hide();
    }

    private void Update()
    {
        if (IsActive)
        {
            Vector2 mousePos = TileUtils.MouseCellPosition();
            GameObject obj = TileManagerScript.TileManager.GetGameObject(mousePos);
            DataPublisher publisher = obj?.GetComponent<DataPublisher>();

            if (m_state == State.WirePlacing)
            {
                DrawLine(mousePos);

                if (Input.GetMouseButtonUp(MouseUtils.PRIMARY_MOUSE_BUTTON) && publisher != null)
                {
                    m_state = State.SelectPortTo;
                    ShowPortList(mousePos, publisher.PortList);
                }
            }
            else if (m_state == State.Idle)
            {
                if (Input.GetMouseButtonUp(MouseUtils.PRIMARY_MOUSE_BUTTON) && publisher != null)
                {
                    m_fromPosition = mousePos;
                    m_state = State.SelectPortFrom;
                    ShowPortList(mousePos, publisher.PortList);
                }
            }
        }
    }

    private void WirePorts()
    {
        if (m_selectedPortFrom == null || m_selectedPortTo == null)
        {
            return;
        }

        m_selectedPortFrom.SetDestination(m_selectedPortTo);
        m_selectedPortTo.SetSource(m_selectedPortFrom);
    }

    private void ShowPortList(Vector2 pos, List<DataPublisher.DataPort> portList)
    {
        PortList.GetComponent<RectTransform>().SetPositionAndRotation(pos, Quaternion.identity);

        foreach (var port in portList)
        {
            var item = Instantiate(ListItemPrefab, pos, Quaternion.identity, PortList);
            item.localScale = new Vector3(1.0f, 1.0f, 1.0f); // prevent wrong scaling

            string buttonLabel = port.Name;
            var toggle = item.GetComponent<Toggle>();

            if (!port.IsConnected)
            {
                toggle.onValueChanged.AddListener((val) =>
                {
                    if (val)
                    {
                        OnPortSelected(port);
                    }
                });
            }
            else
            {
                toggle.enabled = false;
                buttonLabel += " (connected)";
            }

            var text = item.GetComponent<Text>();
            text.alignment = TextAnchor.MiddleCenter;
            text.text = buttonLabel;
        }

        //PortList.gameObject.SetActive(true);
        foreach (RectTransform t in GetComponent<RectTransform>())
        {
            if (t != gameObject)
                t.gameObject?.SetActive(true);
        }
    }

    private void HidePortList()
    {
        // destroy port list items
        foreach (RectTransform t in PortList.GetComponentInChildren<RectTransform>())
        {
            if (t != PortList)
                Destroy(t.gameObject);
        }

        // hide port list panel and label
        foreach (RectTransform t in GetComponent<RectTransform>())
        {
            if (t != gameObject)
                t.gameObject?.SetActive(false);
        }
    }

    private void DrawLine(Vector2 to)
    {
        if (m_line == null && WirePrefab != null)
        {
            m_line = Instantiate(WirePrefab, new Vector2(), Quaternion.identity);
            Color color = ColorUtils.GetRandomColor();
            m_line.startColor = color;
            m_line.endColor = color;
        }
        m_line.SetPositions(new[] { (Vector3)m_fromPosition, (Vector3)to });
    }

    

    private void SetActiveForChildren(bool isActive)
    {
        foreach (Transform t in PortList.GetComponent<Transform>())
        {
            if (t != gameObject)
                t.gameObject?.SetActive(isActive);
        }
    }

    private void Reset()
    {
        m_state = State.Idle;
        m_selectedPortFrom = null;
        m_selectedPortTo = null;

        if (m_line != null)
        {
            Destroy(m_line);
            m_line = null;
        }
    }

    private enum State
    {
        Inactive, 
        Idle,
        SelectPortFrom,
        WirePlacing,
        SelectPortTo
    }
    private State m_state = State.Inactive;

    private LineRenderer m_line;
    private Vector2 m_fromPosition;
    private DataPublisher.DataPort m_selectedPortFrom;
    private DataPublisher.DataPort m_selectedPortTo;

    LogUtils.DebugLogger m_logger = new LogUtils.DebugLogger("DataPipeMenu");

    private const string WireLayer = "wires";
}
