﻿using UnityEditor;
using UnityEngine;

using Core.Service;
using System.Collections.Generic;

public class NeuralEditorWindow : EditorWindow
{
    private InputController inputController;
    private NetworkConfiguration configuration;

    private GenericMenu createContextMenu;
    private GenericMenu nodeContextMenu;

    private GUISkin nodeSkin;
    private GUISkin nodeElipseSkin;

    private bool isBuildMode = false;
    
    private Vector3 cameraPosition;

    private List<Node> from_build_nodes;
    private List<Node> selected_nodes;
    private List<Link> selected_links;
    
    private const int grid_size = 17;

    
    public void OnGUI()
    {
        if (Selection.activeObject is NetworkConfiguration)
        {
            NetworkConfiguration configuration = Selection.activeObject as NetworkConfiguration;

            this.configuration = configuration;
        }

        InputHandler();
        DrawBackground();
        DrawGrid(cameraPosition.z * grid_size, 0.3F, new Color(0.15F, 0.15F, 0.15F));
        DrawGrid(cameraPosition.z * grid_size * 10, 0.66F, new Color(0.15F, 0.15F, 0.15F));
        DrawLinks();
        DrawNodes();
    }

    public void ShowWindow()
    {
        cameraPosition = Vector3.forward;

        if (nodeElipseSkin == null)
        {
            nodeElipseSkin = Resources.Load<GUISkin>("NodeEllipseSkin");
        }

        if (nodeSkin == null)
        {
            nodeSkin = Resources.Load<GUISkin>("NodeSkin");
        }

        Show();
    }

    #region Input
    private void InputHandler()
    {
        if (inputController == null)
        {
            inputController = new InputController();
        }

        if (inputController.OnMouseDown == null)
        {
            inputController.OnMouseDown = MouseClick;
        }

        if(inputController.OnMouseUp == null)
        {
            inputController.OnMouseUp = MouseUp;
        }

        if(inputController.OnKeyDown == null)
        {
            inputController.OnKeyDown = OnKeyDown;
        }

        inputController.Update();

        Event eventCurrent = Event.current;

        if (eventCurrent.type == EventType.MouseDrag)
        {
            if (inputController.isMiddleMouse)
            {
                cameraPosition += new Vector3(eventCurrent.delta.x, eventCurrent.delta.y);

                Repaint();
            }
        }

        if (configuration != null && eventCurrent.type == EventType.MouseDown && eventCurrent.button == 1)
        {
            if (createContextMenu == null)
            {
                createContextMenu = new GenericMenu();

                foreach (var value in NodeRules.GetNodeList())
                {
                    createContextMenu.AddItem(new GUIContent(value.Key), false, () => CreateNode(value.Value));
                }
            }

            createContextMenu.ShowAsContext();
        }

        ViewportService.cameraPosition = cameraPosition;
    }

    private void MouseClick()
    {
        bool isPointClick = false;
        bool isNodeClick = false;
        bool isLinkClick = false;

        if (inputController.isLeftMouse)
        {
            foreach (Node node in configuration.nodes)
            {
                if (ViewportService.ToSceneRect(ViewportService.GetConnectionEllipse(true, node.position, false)).Contains(inputController.mousePosition))
                {
                    isPointClick = true;

                    BeginBuild(node);

                    break;
                }
            }

            if (isPointClick == false)
            {
                foreach (Node node in configuration.nodes)
                {
                    if (ViewportService.ToSceneRect(node.position).Contains(inputController.mousePosition))
                    {
                        SelectOrDeselectNode(node);

                        isNodeClick = true;

                        break;
                    }
                }
            }

            if (isNodeClick == false)
            {
                //TODO: Link selection handler here...
            }

            if (isNodeClick == false && isLinkClick == false && isPointClick == false)
            {
                ClickOnBackground();
            }
        }

        Repaint();
    }

    private void MouseUp()
    {
        foreach (Node node in configuration.nodes)
        {
            if (ViewportService.ToSceneRect(ViewportService.GetConnectionEllipse(false, node.position, false)).Contains(inputController.mousePosition))
            {
                EndBuild(node);

                break;
            }
        }

        Repaint();
    }

    private void OnKeyDown(KeyCode code)
    {
        switch(code)
        {
            case KeyCode.Delete:
                {
                    if (selected_nodes != null)
                    {
                        RemoveNodes(selected_nodes.ToArray());
                    }
                }
                break;
            case KeyCode.A:
                {
                    if (inputController.isControl)
                    {
                        SelectAll();
                    }
                }
                break;

        }

        Repaint();
    }

    private void ClickOnBackground()
    {
        DeselectAll();
        EndBuild();
    }
    #endregion

    private void RemoveNodes(params Node[] nodes)
    {
        foreach(Node node in nodes)
        {
            if(node == null)
            {
                continue;
            }

            configuration?.RemoveNode(node.ID);
        }
    }

    private void CreateNode(int Type)
    {
        Node node = NodeRules.CreateNode(Type);

        node.position = new Rect(ViewportService.FromScreenVector(inputController.mousePosition), new Vector2(100, 100));

        configuration?.AddNode(node);
    }

    private void CreateLink(Link link)
    {
        configuration?.AddLink(link);
    }

    private void SelectAll()
    {
        if (configuration == null)
        {
            return;
        }

        if(selected_nodes == null)
        {
            selected_nodes = new List<Node>();
        }

        selected_nodes.Clear();

        selected_nodes.AddRange(configuration.nodes);
    }

    private void DeselectAll()
    {
        selected_nodes?.Clear();
        selected_links?.Clear();
    }

    private void SelectOrDeselectNode(Node node)
    {
        if (selected_nodes == null)
        {
            selected_nodes = new List<Node>();
        }

        if (!inputController.isShift)
        {
            selected_nodes.Clear();
        }

        if (!selected_nodes.Contains(node))
        {
            selected_nodes.Add(node);
        }
        else
        {
            selected_nodes.Remove(node);
        }

        if (selected_nodes.Count > 0)
        {
            Selection.activeObject = NodeInspectorContainerFactory.CreateContainer(selected_nodes[selected_nodes.Count - 1], configuration, this);
        }
        else
        {
            if (Selection.activeObject is NodeInspectorContainer)
            {
                Selection.activeObject = null;
            }
        }
    }

    private bool IsSelectedNode(Node node)
    {
        return selected_nodes.Contains(node);
    }

    #region Build handler
    private void BeginBuild(Node node)
    {
        if(from_build_nodes == null)
        {
            from_build_nodes = new List<Node>();
        }

        from_build_nodes.Clear();

        from_build_nodes.Add(node);

        isBuildMode = true;

        DeselectAll();

        Repaint();
    }

    private void EndBuild(Node node = null)
    {
        if (node != null)
        {
            from_build_nodes.ForEach(x =>
            {
                Link link = new Link();

                link.from = x.ID;
                link.to = node.ID;

                CreateLink(link);
            });
        }

        isBuildMode = false;

        from_build_nodes?.Clear();

        Repaint();
    }
    #endregion

    #region Drawing service
    private void DrawNodes()
    {
        float border = 2F;

        if (configuration == null)
        {
            return;
        }

        BeginWindows();

        foreach (Node node in configuration.nodes)
        {

            GUI.skin = nodeSkin;

            if (IsSelectedNode(node))
            {
                GUI.color = new Color(
                    ColorAtlas.orange.r,
                    ColorAtlas.orange.g,
                    ColorAtlas.orange.b,
                    0.55F);

                Rect selection_rect = ViewportService.ToSceneRect(node.position);

                selection_rect.x -= border;
                selection_rect.y -= border;
                selection_rect.width += border * 2;
                selection_rect.height += border * 2;

                GUI.Box(selection_rect, "");
            }

            GUI.color = node.color;

            //node.position = ViewportService.FromSceneRect(GUI.Window(node.ID, ViewportService.ToSceneRect(node.position), (int id) => OnWindow(id, node), node.ID.ToString()));

            GUI.Box(ViewportService.ToSceneRect(node.position), "");
            OnWindow(node.ID, node);
        }

        EndWindows();
    }

    private void DrawLinks()
    {
        if (configuration == null)
        {
            return;
        }

        foreach (var link in configuration.links)
        {
            var from = configuration.nodes.Find(x => x.ID == link.from);
            var to = configuration.nodes.Find(x => x.ID == link.to);

            if (from != null && to != null)
            {
                Rect from_rect = ViewportService.ToSceneRect(ViewportService.GetConnectionEllipse(true, from.position, false));
                Rect to_rect = ViewportService.ToSceneRect(ViewportService.GetConnectionEllipse(false, to.position, false));

                Vector2 vector_from = from_rect.position + from_rect.size * 0.5F;
                Vector2 vector_to = to_rect.position + to_rect.size * 0.5F;

                DrawCurve(2, vector_from, vector_to, new Color(0.55F, 0.55F, 0.55F, 0.6F), new Color(0, 1F, 1F, 0.6F));
            }
        }

        if (isBuildMode && from_build_nodes != null)
        {
            from_build_nodes.ForEach(x =>
            {
                Rect from_rect = ViewportService.ToSceneRect(ViewportService.GetConnectionEllipse(true, x.position, false));

                Vector2 from = from_rect.position + from_rect.size * 0.5F;

                DrawCurve(2, from, inputController.mousePosition, new Color(0.55F, 0.55F, 0.55F, 0.6F), new Color(0, 1F, 1F, 0.6F));
            });

            Repaint();
        }
    }

    private void DrawBackground()
    {
        GUI.color = new Color(0.55F, 0.55F, 0.55F);
        GUI.Box(rootVisualElement.contentRect, "");
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();

        float offset_x = cameraPosition.x % gridSpacing;
        float offset_y = cameraPosition.y % gridSpacing;

        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        for (int x = 0; x <= widthDivs; x++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * x + offset_x, -gridSpacing), new Vector3(gridSpacing * x + offset_x, position.height));
        }

        for (int y = 0; y <= heightDivs; y++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * y + offset_y), new Vector3(position.width, gridSpacing * y + offset_y));
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void DrawCurve(float width, Vector2 start, Vector2 end, Color first_color, Color second_color)
    {
        float steep = 0.05F;
        float delta = 0;

        for (float t = -steep; t < 1F; t += steep)
        {
            float t1 = t + delta;
            float t2 = t + steep + delta;

            float x = Mathf.Lerp(start.x, end.x, t1);
            float y = Mathf.Lerp(start.y, end.y, CurveFunction(t1));

            float x1 = Mathf.Lerp(start.x, end.x, t2);
            float y1 = Mathf.Lerp(start.y, end.y, CurveFunction(t2));

            float cross_x = x * y1 - x1 * y;
            float cross_y = x - x1;

            float length = Mathf.Sqrt(cross_x * cross_x + cross_y * cross_y);

            cross_x /= length;
            cross_y /= length;

            Handles.color = Mathf.Round(t / steep) % 2 == 0 ? first_color : second_color;

            for (float i = -width / 2F; i < width / 2F; i+=0.5F)
            {
                Handles.DrawLine(new Vector3(x + cross_x * i, y + cross_y * i), new Vector3(x1 + cross_x * i, y1 + cross_y * i));
            }
        }
    }

    private void OnWindow(int id, Node node)
    {
        GUI.color = new Color(0, 1F, 1F, 0.3F);
        GUI.skin = nodeElipseSkin;

        var right_rect = ViewportService.ToSceneRect(ViewportService.GetConnectionEllipse(true, node.position, false));
        var left_rect = ViewportService.ToSceneRect(ViewportService.GetConnectionEllipse(false, node.position, false));

        if (NodeRules.ContainsRightConnection(node))
        {
            GUI.Box(right_rect, "");
        }

        if (NodeRules.ContainsLeftConnection(node))
        {
            GUI.Box(left_rect, "");
        }

        if (IsSelectedNode(node) && inputController != null && inputController.isLeftMouse)
        {
            node.position.position += inputController.delta * 0.5F;

            Repaint();
        }
    }

    private float CurveFunction(float value)
    {
        return Mathf.Sin(value * Mathf.PI + 1.5F * Mathf.PI) / 2F + 0.5F;
    }
    #endregion
}