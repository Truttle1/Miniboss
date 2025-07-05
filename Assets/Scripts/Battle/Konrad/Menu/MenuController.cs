using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    private List<GameObject> menuItems;
    private const float startX = 760f;
    private const float startY = 800f;
    private const float offsetY = 90f;

    private int selection;

    //For scrolling
    private const int maxVisibleItems = 4;
    private int selectionTop = 0;
    private bool showingMenu = true;

    public GameObject menuItem;
    public GameObject goBackText;

    public GameObject upArrow;
    public GameObject downArrow;


    public TMP_Text flavorText;
    public GameObject flavorTextSection;

    public StatusBarController statusBar;

    
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
                    selectionTop = Mathf.Max(0, menuItems.Count - maxVisibleItems);
                }

                if (selection < selectionTop)
                {
                    selectionTop = selection;
                }

                EventBus.Publish(new MenuItemHoverEvent(menuItems[selection].GetComponent<MenuItemController>().getMessage()));
                
                SetFlavorText(menuItems[selection].GetComponent<MenuItemController>().getFlavorText());
                RefreshMenuDisplay();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                menuItems[selection].GetComponent<MenuItemController>().setSelected(false);
                selection += 1;
                if (selection > menuItems.Count - 1)
                {
                    selection = 0;
                    selectionTop = 0;
                }

                if (selection >= selectionTop + maxVisibleItems)
                {
                    selectionTop = selection - maxVisibleItems + 1;
                }
                EventBus.Publish(new MenuItemHoverEvent(menuItems[selection].GetComponent<MenuItemController>().getMessage()));
                
                SetFlavorText(menuItems[selection].GetComponent<MenuItemController>().getFlavorText());
                RefreshMenuDisplay();
            }

            if(!StatusBarController.instance.isRunning())
            {
                if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
                {
                    EventBus.Publish(new MenuItemSelectEvent(menuItems[selection].GetComponent<MenuItemController>().getMessage()));
                }

                if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
                {
                    EventBus.Publish(new MenuItemSelectEvent("BACK"));
                }
            }

            if(menuItems[selection] != null && menuItems[selection].GetComponent<MenuItemController>() != null)
            {
                menuItems[selection].GetComponent<MenuItemController>().setSelected(true);
            }
        }
        else
        {
            gameObject.GetComponent<Canvas>().enabled = false;
        }
    }


    private IEnumerator DestroyOldMenuItems(List<GameObject> oldItems)
    {
        yield return null;
        foreach (var item in oldItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
    }
    public void SetMenu(MenuItemDataEvent e)
    {
        if (e.showMenu)
        {
            
            if (menuItems != null)
            {
                StartCoroutine(DestroyOldMenuItems(menuItems));
            }

            showingMenu = true;
            bool showBack = e.showBackText;
            MenuItemData[] menuItemData = e.items;
            selection = 0;
            selectionTop = 0;
            List<GameObject> newItems = new List<GameObject>();
            for (int i = 0; i < menuItemData.Length; i++)
            {
                GameObject newItem = Instantiate(menuItem);
                newItem.transform.SetParent(gameObject.transform);
                newItem.transform.position = new Vector3(startX, startY - offsetY * i, newItem.transform.position.z);
                newItem.GetComponent<MenuItemController>().SetState(
                    menuItemData[i].name,
                    menuItemData[i].cost,
                    menuItemData[i].message,
                    menuItemData[i].flavorText,
                    menuItemData[i].overrideColor
                );
                newItem.GetComponent<MenuItemController>().setItemEnabled(menuItemData[i].enabled);
                newItems.Add(newItem);
            }
            menuItems = newItems;
            EventBus.Publish(new MenuItemHoverEvent(menuItems[selection].GetComponent<MenuItemController>().getMessage()));

            goBackText.SetActive(showBack);
            
            SetFlavorText(menuItems[selection].GetComponent<MenuItemController>().getFlavorText());
            RefreshMenuDisplay();
        }
        else
        {
            showingMenu = false;
            EventBus.Publish(new MenuItemHoverEvent(""));
        }
    }

    private void RefreshMenuDisplay()
    {
        for (int i = 0; i < menuItems.Count; i++)
        {
            GameObject itemObj = menuItems[i];
            if (i >= selectionTop && i < selectionTop + maxVisibleItems)
            {
                itemObj.SetActive(true);
                itemObj.transform.position = new Vector3(startX, startY - offsetY * (i - selectionTop), itemObj.transform.position.z);
            }
            else
            {
                itemObj.SetActive(false);
            }

            itemObj.GetComponent<MenuItemController>().setSelected(i == selection);

            if(selectionTop > 0)
            {
                upArrow.SetActive(true);
            }
            else
            {
                upArrow.SetActive(false);
            }

            if(selectionTop + maxVisibleItems < menuItems.Count)
            {
                downArrow.SetActive(true);
            }
            else
            {
                downArrow.SetActive(false);
            }
        }
    }

    private void SetFlavorText(string text)
    {
        if (text != null && text != "")
        {
            // Replace \ with new line
            string newText = text.Replace("\\", "\n");
            flavorText.text = newText;
            flavorTextSection.SetActive(true);
        }
        else
        {
            flavorTextSection.SetActive(false);
        }
    }
}


public class MenuItemData
{
    public string name;
    public string cost;
    public string message;
    public string flavorText;
    public bool enabled;

    public Color? overrideColor = null;

    public MenuItemData(string name, string cost, string message, string flavorText = "", bool enabled = true)
    {
        this.name = name;
        this.cost = cost;
        this.message = message;
        this.flavorText = flavorText;
        this.enabled = enabled;
    }

    public MenuItemData(string name, string cost, string message, bool enabled = true, Color? overrideColor = null)
    {
        this.name = name;
        this.cost = cost;
        this.message = message;
        this.enabled = enabled;
        this.overrideColor = overrideColor;
    }
    public MenuItemData(string name, string cost, string message)
    {
        this.name = name;
        this.cost = cost;
        this.message = message;
        this.enabled = true;
    }
}

public class MenuItemDataEvent
{
    public MenuItemData[] items;
    public bool showBackText;
    public bool showMenu;

    public MenuItemDataEvent(MenuItemData[] items, bool showBackText)
    {
        if(items != null && items.Length != 0) 
        {
            this.items = items;
            this.showBackText = showBackText;
            this.showMenu = true;
        }
    }

    public MenuItemDataEvent(bool showMenu)
    {
        this.showMenu = showMenu;
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