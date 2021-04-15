using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuilderMenu : MonoBehaviour, IMenu
{
    public Texture RemoverIcon;
    public GameObject ButtonPrefab;

    // IMenu implementation
    public void Show()
    {
        GetComponent<Image>().enabled = true;
        GetComponent<ScrollRect>().enabled = true;
        GameObjectUtils.SetActiveForChildren(gameObject, true);
    }

    public void Hide()
    {
        GetComponent<Image>().enabled = false;
        GetComponent<ScrollRect>().enabled = false;
        GameObjectUtils.SetActiveForChildren(gameObject, false);
    }

    public bool IsCameraZoomAllowed()
    {
        return true;
    }

    public string Name => MenuName;
    // IMenu end

    private void InitButton(BuildableObjectScript obj)
    {
        Sprite img = (obj.Image != null) ? obj.Image : obj.Prefab.GetComponent<SpriteRenderer>().sprite;
        GameObject button = Instantiate(ButtonPrefab, m_builderPanelContent.transform);
        button.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        button.GetComponent<RawImage>().texture = img.texture;
        button.GetComponent<Button>().onClick.AddListener(call: delegate { Pick(obj.Prefab); });
        button.transform.Find("costLabel").GetComponent<TextMeshProUGUI>().text = obj.Cost.ToString() + '$';
        button.transform.Find("nameLabel").GetComponent<TextMeshProUGUI>().text = obj.Name;
    }

    private void InitRemoverButton()
    {
        GameObject button = Instantiate(ButtonPrefab, m_builderPanelContent.transform);
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        button.GetComponent<RawImage>().texture = RemoverIcon;
        button.GetComponent<Button>().onClick.AddListener(call: PickRemover);
        button.transform.Find("nameLabel").GetComponent<TextMeshProUGUI>().text = "Remover";
    }

    private void InitBuilderPanel()
    {
        GameObject viewPort = transform.Find("Viewport").gameObject;
        m_builderPanelContent = viewPort.transform.Find("Content").gameObject;

        // clear panel
        for (var i = 0; i < m_builderPanelContent.transform.childCount; i++)
        {
            Destroy(m_builderPanelContent.transform.GetChild(i).gameObject);
        }

        List<BuildableObjectScript> objects = objectBuilder.Builder.Objects;
        InitRemoverButton();
        foreach (var obj in objects)
        {
            InitButton(obj);
        }
    }

    private void Pick(GameObject prefab)
    {
        objectBuilder.Builder.Pick(prefab.GetComponent<BuildableObjectScript>());
    }

    private void PickRemover()
    {
        objectBuilder.Builder.PickRemover();
    }
    
    void Start()
    {
        InitBuilderPanel();
    }

    private GameObject m_builderPanelContent;

    private const string MenuName = "Ready";
}
