﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuilderMenu : MonoBehaviour, IMenu
{
    public GameObject ButtonPrefab;
    public int ButtonsMargin = 10;


    public void Show()
    {
        GetComponent<Image>().enabled = true;
        GetComponent<ScrollRect>().enabled = true;
        setActiveForChildren(true);
    }

    public void Hide()
    {
        GetComponent<Image>().enabled = false;
        GetComponent<ScrollRect>().enabled = false;
        setActiveForChildren(false);
    }

    private void InitButton(BuildableObjectScript obj, ref Vector2 position)
    {
        Sprite img = (obj.Image != null) ? obj.Image : obj.Prefab.GetComponent<SpriteRenderer>().sprite;
        GameObject button = Instantiate(ButtonPrefab, m_builderPanelContent.transform);
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        rectTransform.localPosition = position;
        rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        button.GetComponent<RawImage>().texture = img.texture;
        button.GetComponent<Button>().onClick.AddListener(call: delegate { Pick(obj.Prefab); });
        position -= new Vector2(0, ButtonsMargin + button.GetComponent<RectTransform>().rect.height);
    }

    private void InitBuilderPanel()
    {
        Vector2 localPos = new Vector3(GetComponent<RectTransform>().rect.width / 2, 0.0f);
        GameObject viewPort = transform.Find("Viewport").gameObject;
        m_builderPanelContent = viewPort.transform.Find("Content").gameObject;

        // clear panel
        for (var i = 0; i < m_builderPanelContent.transform.childCount; i++)
        {
            Destroy(m_builderPanelContent.transform.GetChild(i).gameObject);
        }

        List<BuildableObjectScript> objects = objectBuilder.Builder.Objects;

        float buttonHeight = ButtonPrefab.GetComponent<RectTransform>().rect.height;
        float height = (ButtonsMargin + buttonHeight) * (objects.Count + 1) + ButtonsMargin;
        m_builderPanelContent.GetComponent<RectTransform>().sizeDelta = new Vector2(m_builderPanelContent.GetComponent<RectTransform>().sizeDelta.x, height);

        GameObject button = Instantiate(ButtonPrefab, m_builderPanelContent.transform);
        localPos -= new Vector2(0, ButtonsMargin + button.GetComponent<RectTransform>().rect.height / 2);
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        rectTransform.localPosition = localPos;
        rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        button.GetComponent<RawImage>().texture = objectBuilder.Builder.RemoverPrefab.GetComponent<SpriteRenderer>().sprite.texture;
        button.GetComponent<Button>().onClick.AddListener(call: PickRemover);

        localPos -= new Vector2(0, ButtonsMargin + buttonHeight);
        foreach (var obj in objects)
        {
            InitButton(obj, ref localPos);
        }
    }

    private void Pick(GameObject prefab)
    {
        objectBuilder.Builder.Pick(prefab);
    }

    private void PickRemover()
    {
        objectBuilder.Builder.PickRemover();
    }
    
    void Start()
    {
        InitBuilderPanel();  
        m_menuManager = MenuManager.Manager;
    }

    private void setActiveForChildren(bool isActive)
    {
        foreach (Transform t in GetComponent<Transform>())
        {
            if (t != gameObject)
                t.gameObject?.SetActive(isActive);
        }
    }

    private GameObject m_builderPanelContent;
    private MenuManager m_menuManager;
}
