using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    private List<GameObject> menuItems;
    private const float startX = 480f;
    private const float startY = 810f;
    private const float offsetY = 90f;

    private int selection;


    public GameObject menuItem;
    private void Start()
    {
        EventBus.Subscribe<MenuItemData[]>(SetMenu);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            menuItems[selection].GetComponent<MenuItemController>().setSelected(false);
            selection -= 1;
            if (selection < 0)
            {
                selection = menuItems.Count - 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            menuItems[selection].GetComponent<MenuItemController>().setSelected(false);
            selection += 1;
            if (selection > menuItems.Count - 1)
            {
                selection = 0;
            }
        }

        menuItems[selection].GetComponent<MenuItemController>().setSelected(true);
    }

    public void SetMenu(MenuItemData[] menuItemData)
    {
        menuItems = new List<GameObject>();
        for(int i = 0; i < menuItemData.Length; i++)
        {
            GameObject newItem = Instantiate(menuItem);
            newItem.transform.SetParent(gameObject.transform);
            newItem.transform.position = new Vector3(startX, startY - offsetY * i, newItem.transform.position.z);
            newItem.GetComponent<MenuItemController>().SetState(menuItemData[i].name, menuItemData[i].cost, menuItemData[i].message);
            menuItems.Add(newItem);
        }
        selection = 0;
    }
}


public class MenuItemData
{
    public string name;
    public string cost;
    public string message;

    public MenuItemData(string name, string cost, string message)
    {
        this.name = name;
        this.cost = cost;
        this.message = message;
    }
}