using System;
using System.Collections.Generic;
using System.Numerics;
using System.Xml.Xsl;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class DataPipeMenu : MonoBehaviour, IMenu
{
    private DataPipeMenu() {}
    private static DataPipeMenu g_instance;
    public static DataPipeMenu Menu => g_instance ?? (g_instance = GameObject.Find("WiresMenu").GetComponent<DataPipeMenu>());

    public RectTransform PortList;
    public LineRenderer WirePrefab;
    public RectTransform ListItemPrefab;


    public void OnPortSelected(DataPublisher.DataPort port)
    {
        m_mouseEventReady = false;

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

    public void OnPortReleased(DataPublisher.DataPort port)
    {
        port.Reset();
        Reset();
        HidePortList();

        m_logger.Log($"Release port {port.Name}.");
    }

    // IMenu implementation
    public void Hide()
    {
        HidePortList();
        Reset();
        global::camera.SetLayerVisibility(WireLayer, false);

        m_state = State.Inactive;
    }

    public bool IsCameraZoomAllowed()
    {
        return true;
    }

    public string Name => MenuName;
    // IMenu end

    public void Show()
    {
        MenuManager.Manager.SetActive(this);
        global::camera.SetLayerVisibility(WireLayer, true);

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
        if (IsActive && m_mouseEventReady)
        {
            Vector2 mousePos = TileUtils.NormalizedMousePosition();
            GameObject obj = TileManagerScript.TileManager.GetGameObject(mousePos);
            DataPublisher publisher = obj?.GetComponent<DataPublisher>();

            if (m_state == State.WirePlacing)
            {
                DrawLine(mousePos);

                if (m_mouseEventReady && Input.GetMouseButtonUp(MouseUtils.PRIMARY_MOUSE_BUTTON) && publisher != null && m_publisherFrom != publisher)
                {
                    m_state = State.SelectPortTo;
                    m_selectedPortIndex = -1;
                    ShowPortList(mousePos, publisher.PortList);
                }
            }
            else if (m_state == State.Idle)
            {
                if (Input.GetMouseButtonUp(MouseUtils.PRIMARY_MOUSE_BUTTON) && publisher != null)
                {
                    m_fromPosition = mousePos;
                    m_state = State.SelectPortFrom;
                    m_publisherFrom = publisher;
                    ShowPortList(mousePos, publisher.PortList);
                }
            }
        }

        m_mouseEventReady = true;
    }

    private void WirePorts()
    {
        if (m_selectedPortFrom == null || m_selectedPortTo == null || m_line == null)
        {
            m_logger.Warn("Port wiring error. One of the ends or line renderer is null.");
            return;
        }

        m_logger.Log($"Wiring ports {m_selectedPortFrom.Name} and {m_selectedPortTo.Name}");
        DataPublisher.DataPort.WirePorts(m_selectedPortFrom, m_selectedPortTo, m_line);
    }

    private void ShowPortList(Vector2 pos, List<DataPublisher.DataPort> portList)
    {
        GetComponent<RectTransform>().SetPositionAndRotation(pos, Quaternion.identity);
        
        for (int i = 0; i < portList.Count; i++)
        {
            CreatePortListItem(portList[i], pos, i);
        }

        foreach (RectTransform t in GetComponent<RectTransform>())
        {
            if (t != gameObject)
                t.gameObject?.SetActive(true);
        }
    }

    private void CreatePortListItem(DataPublisher.DataPort port, Vector2 pos, int index)
    {
        var item = Instantiate(ListItemPrefab, pos, Quaternion.identity, PortList);
        item.localScale = new Vector3(1.0f, 1.0f, 1.0f); // prevent wrong scaling

        string buttonLabel = port.Name;
        var toggle = item.GetComponent<Toggle>();

        if (!port.IsConnected)
        {
            var i = index; // local var to make a closure
            toggle.onValueChanged.AddListener((val) =>
            {
                if (val)
                {
                    OnPortSelected(port);
                    m_selectedPortIndex = i;
                }
            });

            buttonLabel += " (connect)";
        }
        else
        {
            if (m_state == State.SelectPortFrom)
            {
                toggle.onValueChanged.AddListener((val) =>
                {
                    if (val)
                    {
                        OnPortReleased(port);
                    }
                });
            }
            else
            {
                toggle.enabled = false;
            }

            buttonLabel += " (release)";
        }

        var text = item.GetComponent<Text>();
        text.alignment = TextAnchor.MiddleCenter;
        text.text = buttonLabel;
    }

    private void HidePortList()
    {
        // destroy port list items
        foreach (RectTransform t in PortList.GetComponentInChildren<RectTransform>())
        {
            if (t != PortList)
                Destroy(t.gameObject);
        }
        
        GameObjectUtils.SetActiveForChildren(gameObject, false);
        gameObject.SetActive(true); // do not disable the menu object itself (to avoid update() disabling)
    }

    private void DrawLine(Vector2 to)
    {
        if (m_line == null && WirePrefab != null)
        {
            m_line = Instantiate(WirePrefab, new Vector2(), Quaternion.identity);
            if (m_line == null)
            {
                m_logger.Warn("Cannot instantiate line renderer.");
                return;
            }

            Color color = ColorUtils.GetRandomColor();
            m_line.startColor = color;
            m_line.endColor = color;
        }

        Vector3[] points = GetCurvePoints(m_fromPosition, to, GetCurveShift(m_selectedPortIndex));
        m_line.positionCount = points.Length;
        m_line.SetPositions(points);
    }

    private static float GetCurveShift(int index)
    {
        float curveShift = (index - 2) / 2.0f;
        if (curveShift >= 0)
        {
            curveShift += 0.5f;
        }
        return curveShift;
    }

    private static Vector3[] GetCurvePoints(Vector2 from, Vector2 to, float curveShift = 1.0f)
    {
        if (from == to)
        {
            return new Vector3[] { to } ;
        }

        Vector2 line = to - from;
        float angleSin = line.y / line.magnitude;
        float angleCos = line.x / line.magnitude;

        Vector2 shiftLine = new Vector2(-curveShift * angleSin, curveShift * angleCos);
        Vector2 middlePoint = new Vector3((from.x + to.x) / 2.0f, (from.y + to.y) / 2.0f);
        Vector2 referencePoint = middlePoint + shiftLine;

        // Bezier curve: P0(1 - t)^2 + 2tP1(1 - t) + P2t^2, where t in [0, 1]
        float t = 0.0f;
        const float curveStep = 0.1f;
        const int curveSize = (int) (1.0f / curveStep) + 1;
        Vector3[] points = new Vector3[curveSize];

        for (int i = 0; t <= 1.0f && i < curveSize; i++, t += curveStep)
        {
            float x = (float) (Math.Pow(1 - t, 2) * from.x + 2 * t * (1 - t) * referencePoint.x + Math.Pow(t, 2) * to.x);
            float y = (float) (Math.Pow(1 - t, 2) * from.y + 2 * t * (1 - t) * referencePoint.y + Math.Pow(t, 2) * to.y);
            points[i] = new Vector3(x, y);
        }

        points[curveSize - 1] = to;

        return points;
    }

    private void Reset()
    {
        m_state = State.Idle;
        m_selectedPortFrom = null;
        m_selectedPortTo = null;
        m_publisherFrom = null;
        m_mouseEventReady = true;
        m_selectedPortIndex = -1;

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

    private bool m_mouseEventReady;
    private int m_selectedPortIndex;
    private LineRenderer m_line;
    private Vector2 m_fromPosition;
    private DataPublisher.DataPort m_selectedPortFrom;
    private DataPublisher.DataPort m_selectedPortTo;
    private DataPublisher m_publisherFrom;

    private readonly LogUtils.DebugLogger m_logger = new LogUtils.DebugLogger("DataPipeMenu");

    private const string MenuName = "Wire Menu";
    private const string WireLayer = "wires";
}
