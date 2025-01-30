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

    //For scrolling
    private int selectionTop = 0;
    private bool showingMenu = true;

    public GameObject menuItem;
    public GameObject goBackText;
    private void Start()
    {
        EventBus.Subscribe<MenuItemDataEvent>(SetMenu);
    }

    private void Update()
    {
        if(showingMenu)
        {
            gameObject.GetComponent<Canvas>().enabled = true;
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                menuItems[selection].GetComponent<MenuItemController>().setSelected(false);
                selection -= 1;
                if (selection < 0)
                {
                    selection = menuItems.Count - 1;
                }
                EventBus.Publish(new MenuItemHoverEvent(menuItems[selection].GetComponent<MenuItemController>().getMessage()));
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                menuItems[selection].GetComponent<MenuItemController>().setSelected(false);
                selection += 1;
                if (selection > menuItems.Count - 1)
                {
                    selection = 0;
                }
                EventBus.Publish(new MenuItemHoverEvent(menuItems[selection].GetComponent<MenuItemController>().getMessage()));
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                EventBus.Publish(new MenuItemSelectEvent(menuItems[selection].GetComponent<MenuItemController>().getMessage()));
            }

            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
            {
                EventBus.Publish(new MenuItemSelectEvent("BACK"));
            }

            menuItems[selection].GetComponent<MenuItemController>().setSelected(true);
        }
        else
        {
            gameObject.GetComponent<Canvas>().enabled = false;
        }
    }

    public void SetMenu(MenuItemDataEvent e)
    {
        if (e.showMenu)
        {
            showingMenu = true;
            bool showBack = e.showBackText;
            MenuItemData[] menuItemData = e.items;
            selection = 0;
            if (menuItems != null)
            {
                for (int i = 0; i < menuItems.Count; i++)
                {
                    if (menuItems[i] != null)
                    {
                        Destroy(menuItems[i]);
                    }
                }
            }
            menuItems = new List<GameObject>();
            for (int i = 0; i < menuItemData.Length; i++)
            {
                GameObject newItem = Instantiate(menuItem);
                newItem.transform.SetParent(gameObject.transform);
                newItem.transform.position = new Vector3(startX, startY - offsetY * i, newItem.transform.position.z);
                newItem.GetComponent<MenuItemController>().SetState(menuItemData[i].name, menuItemData[i].cost, menuItemData[i].message);
                menuItems.Add(newItem);
            }
            EventBus.Publish(new MenuItemHoverEvent(menuItems[selection].GetComponent<MenuItemController>().getMessage()));

            goBackText.SetActive(showBack);
        }
        else
        {
            showingMenu = false;
            EventBus.Publish(new MenuItemHoverEvent(""));
        }
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

public class MenuItemDataEvent
{
    public MenuItemData[] items;
    public bool showBackText;
    public bool showMenu;

    public MenuItemDataEvent(MenuItemData[] items, bool showBackText)
    {
        this.items = items;
        this.showBackText = showBackText;
        this.showMenu = true;
    }

    public MenuItemDataEvent(bool showMenu)
    {
        this.showMenu =showMenu;
    }
}

public class MenuItemSelectEvent
{
    public string message;

    public MenuItemSelectEvent(string message)
    {
        this.message = message;
    }
}
public class MenuItemHoverEvent
{
    public string message;

    public MenuItemHoverEvent(string message)
    {
        this.message = message;
    }
}