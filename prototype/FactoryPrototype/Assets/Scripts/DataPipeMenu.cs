using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
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


    public void SelectPort(string port)
    {
        m_selectedPort = port;
        m_state = State.WirePlacing;
        HidePortList();
    }

    // IMenu implementation
    public void Hide()
    {
        //GetComponent<Image>().enabled = false;
        //SetActiveForChildren(false);
        m_state = State.Inactive;
        HidePortList();
    }

    public void Show()
    {
        //GetComponent<Image>().enabled = true;
       // SetActiveForChildren(true);

        MenuManager.Manager.SetActive(this);

        m_state = State.Idle;
    }

    public bool IsActive => m_state != State.Inactive;

    // MonoBehavior implementation
    void Start()
    {
        Hide();
        HidePortList();
    }

    void Update()
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
                    m_state = State.Idle;
                    m_line = null;
                    // TODO wire 2 publishers
                }
            }
            else if (m_state == State.Idle)
            {
                if (Input.GetMouseButtonUp(MouseUtils.PRIMARY_MOUSE_BUTTON) && publisher != null)
                {
                    m_fromPosition = mousePos;
                    m_state = State.PortListMenu;
                    ShowPortList(mousePos, publisher.GetPortList());
                }
            }
        }
    }

    private void ShowPortList(Vector2 pos, List<string> portList)
    {
        PortList.transform.position = pos;
        foreach (var port in portList)
        {
            var item = Instantiate(ListItemPrefab, pos, Quaternion.identity);
            item.SetParent(PortList);
            item.GetComponent<Text>().text = port;
            var button = item.gameObject.AddComponent<Button>();
            button.onClick.AddListener(() =>
            {
                SelectPort(port);
            });
        }

        PortList.gameObject.SetActive(true);
    }

    private void HidePortList()
    {
        foreach (Transform t in PortList.GetComponent<Transform>())
        {
            if (t != gameObject)
                Destroy(t);
        }
        PortList.gameObject.SetActive(false);
    }

    private void DrawLine(Vector2 to)
    {
        if (m_line == null && WirePrefab != null)
        {
            m_line = Instantiate(WirePrefab, new Vector2(), Quaternion.identity);
        }
        m_line.SetPositions(new[] { (Vector3)m_fromPosition, (Vector3)to });
    }

    private void SetActiveForChildren(bool isActive)
    {
        foreach (Transform t in GetComponent<Transform>())
        {
            if (t != gameObject)
                t.gameObject?.SetActive(isActive);
        }
    }

    private enum State
    {
        Inactive, 
        Idle,
        PortListMenu,
        WirePlacing
    }
    private State m_state = State.Inactive;

    private LineRenderer m_line;
    private Vector2 m_fromPosition;
    private DataPublisher m_fromPublisher;
    private string m_selectedPort;
}
