using UnityEngine;
using System.Collections.Generic;

using GUIControls;
using System;

public class GUIContextMenu
{
    public static bool IsOpened { get => s_isOpened; }

    public static Vector2 Position { get => s_contextRect.position; }

    private static bool s_isOpened;

    private static Rect s_contextRect;
    private static Rect s_findBoxRect;
    private static Rect s_buttonRect;

    private static  MenuItemsTree s_menuItemsTree;

    private static GUISkin s_skin;
    private static string searchText;

    public static void Show(Vector2 position, params MenuItem[] menuItems)
    {
        s_isOpened = true;
        searchText = string.Empty;

        s_contextRect = new Rect(position, new Vector2(200, 270));
        s_skin = Resources.Load<GUISkin>("Skins/ContextMenuSkin");

        s_menuItemsTree = new MenuItemsTree(menuItems);
        
        CalculateBounds();
    }

    public static void Draw()
    {
        if (s_isOpened)
        {
            if (Event.current.type == EventType.MouseDown && !s_contextRect.Contains(Event.current.mousePosition))
                s_isOpened = false;

            GUI.skin = s_skin;
            GUI.color = new Color(0.85F, 0.85F, 0.85F, 1.0F);

            GUI.Box(s_contextRect, "");
            searchText = GUI.TextField(s_findBoxRect, searchText);

            for (int i = 0; i < s_menuItemsTree.ChildNodesCount(); i++)
            {
                Rect rect = new Rect(s_buttonRect.x, s_buttonRect.y + s_buttonRect.height * i, s_buttonRect.width, s_buttonRect.height);

                if (GUI.Button(rect, s_menuItemsTree.GetItem(i).Name))
                {
                    s_menuItemsTree.GetItem(i).Invoke();
                }
            }
        }
    }

    public static void Close()
    {
        s_isOpened = false;
    }

    private static void CreteLevelTree(MenuItem[] menuItems)
    {

    }

    private static void CalculateBounds()
    {
        float space = s_contextRect.width * 0.025F;
        float half_space = space * 0.5F;
        float text_height = 20F;

        s_findBoxRect = new Rect(
            s_contextRect.x + half_space,
            s_contextRect.y + half_space,
            s_contextRect.width - space,
            text_height);

        s_buttonRect = new Rect(
            s_contextRect.x + half_space,
            s_findBoxRect.y + s_findBoxRect.height + space,
            s_contextRect.width - space,
            text_height);
    }
}
