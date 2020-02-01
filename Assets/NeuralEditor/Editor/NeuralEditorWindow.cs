using Core.Service;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NeuralEditorWindow : EditorWindow
{
    private InputController inputController;
    private NetworkConfiguration configuration;

    private GenericMenu createContextMenu;
    private GenericMenu nodeContextMenu;

    private GUISkin nodeSkin;
    private GUISkin nodeElipseSkin;

    private List<Node> from_build_nodes;
    private List<Node> selected_nodes;
    private List<Link> selected_links;

    private const float collision_radius = 0.65F;
    private const float border_width = 12F;
    private const float grid_size = 17;

    public void OnGUI()
    {
        if (Selection.activeObject is NetworkConfiguration)
        {
            NetworkConfiguration configuration = Selection.activeObject as NetworkConfiguration;

            this.configuration = configuration;
        }

        InitializeIfNull();
        InitializeStyleIfNot();

        InputHandler();

        DrawBackground();
        DrawGrid(grid_size, 0.3F, new Color(0.15F, 0.15F, 0.15F));
        DrawGrid(grid_size * 10, 0.66F, new Color(0.15F, 0.15F, 0.15F));
        DrawLinks();
        DrawNodes();
    }

    #region Input

    private void InputHandler()
    {
        if (inputController.OnMouseDown == null)
        {
            inputController.OnMouseDown = MouseDown;
        }

        if (inputController.OnMouseUp == null)
        {
            inputController.OnMouseUp = MouseUp;
        }

        if (inputController.OnKeyDown == null)
        {
            inputController.OnKeyDown = OnKeyDown;
        }

        inputController.Update();

        if (inputController.isMiddleMouse)
        {
            ViewportService.cameraPosition += inputController.delta;

            Repaint();
        }

        if (inputController.isScroll)
        {
            //Zoom normalno ne rabotaet
            //ViewportService.Zoom += inputController.delta.y * 0.01F;

            Repaint();
        }
    }

    private void MouseDown()
    {
        if (configuration == null)
        {
            return;
        }

        if (inputController.isLeftMouse)
        {
            bool isPointClick = false;
            bool isNodeClick = false;
            bool isLinkClick = false;

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

        if (inputController.isRightMouse)
        {
            if (createContextMenu == null)
            {
                createContextMenu = new GenericMenu();

                foreach (var value in NodeRules.GetNodeList())
                {
                    createContextMenu.AddItem(new GUIContent($"Create/{value.Key}"), false, () => CreateNode(value.Value));
                }
            }

            inputController.ResetMouseButtons();

            createContextMenu.ShowAsContext();
        }

        Repaint();
    }

    private void MouseUp()
    {
        if (configuration == null)
        {
            return;
        }

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
        switch (code)
        {
            case KeyCode.Escape:
                {
                    EndBuild();
                }
                break;

            case KeyCode.Delete:
                {
                    RemoveNodes(selected_nodes.ToArray());
                    RemoveLinks(selected_links.ToArray());

                    EndBuild();
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

            case KeyCode.D:
                {
                    if (inputController.isControl)
                    {
                        Dublicate();
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

    #endregion Input

    #region Node service

    private void CreateNode(int Type)
    {
        Node node = NodeRules.CreateNode(Type);

        node.position = new Rect(ViewportService.FromScreenPoint(inputController.mousePosition), new Vector2(100, 100));

        configuration?.AddNode(node);
    }

    private void RemoveNodes(params Node[] nodes)
    {
        if (configuration == null)
        {
            return;
        }

        foreach (Node node in nodes)
        {
            configuration.RemoveNode(node);
        }
    }

    private void CreateLink(Link link)
    {
        configuration?.AddLink(link);
    }

    private void RemoveLinks(params Link[] links)
    {
        if (configuration == null)
        {
            return;
        }

        foreach (Link link in links)
        {
            configuration.RemoveLink(link);
        }
    }

    private void Dublicate()
    {
        if (configuration == null)
        {
            return;
        }

        configuration.Dublicate(selected_nodes.ToArray());

        selected_nodes.ForEach(node =>
        {
            CalculateCollision(Vector2.one * node.position.size.magnitude * 0.5F, node);
        });

        Repaint();
    }

    private void SelectAll()
    {
        if (configuration == null)
        {
            return;
        }

        selected_nodes.Clear();

        selected_nodes.AddRange(configuration.nodes);
    }

    private void DeselectAll()
    {
        selected_nodes.Clear();
        selected_links.Clear();
    }

    private void BeginMultiselect()
    {
    }

    private void EndMultiselect()
    {
    }

    private void SelectOrDeselectNode(Node node)
    {
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
            Selection.objects = NodeInspectorContainerFactory.CreateContainer(selected_nodes, configuration, this);
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

    #endregion Node service

    #region Build handler

    private void BeginBuild(Node node)
    {
        from_build_nodes.Clear();

        from_build_nodes.Add(node);

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

        from_build_nodes.Clear();

        Repaint();
    }

    private bool IsBuildMode()
    {
        return from_build_nodes.Count > 0;
    }

    #endregion Build handler

    #region Drawing service

    private void DrawNodes()
    {
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

                selection_rect.x -= border_width * 0.5F;
                selection_rect.y -= border_width * 0.5F;
                selection_rect.width += border_width;
                selection_rect.height += border_width;

                GUI.Box(selection_rect, "");
            }

            var right_rect = ViewportService.ToSceneRect(ViewportService.GetConnectionEllipse(true, node.position, false));
            var left_rect = ViewportService.ToSceneRect(ViewportService.GetConnectionEllipse(false, node.position, false));

            GUI.color = node.color;

            GUI.Box(ViewportService.ToSceneRect(node.position), "");

            GUI.color = new Color(0, 1F, 1F, 0.3F);
            GUI.skin = nodeElipseSkin;

            if (NodeRules.ContainsRightConnection(node))
            {
                GUI.Box(right_rect, "");
            }

            if (NodeRules.ContainsLeftConnection(node))
            {
                GUI.Box(left_rect, "");
            }

            if (IsSelectedNode(node) && inputController.isLeftMouse)
            {
                node.position.position += inputController.delta * 0.5F;

                CalculateCollision(inputController.delta, node);

                Repaint();
            }
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

                Vector2 from_point = from_rect.position + new Vector2(to_rect.size.x, to_rect.size.y * 0.5F);
                Vector2 to_point = to_rect.position + new Vector2(0, to_rect.size.y * 0.5F);

                DrawCurve(2, from_point, to_point, new Color(0.55F, 0.55F, 0.55F, 0.6F), new Color(0.55F, 0.55F, 0.55F, 0.6F));
            }
        }

        if (IsBuildMode())
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

        float offset_x = ViewportService.cameraPosition.x % gridSpacing;
        float offset_y = ViewportService.cameraPosition.y % gridSpacing;

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

            for (float i = -width / 2F; i < width / 2F; i += 0.5F)
            {
                Handles.DrawLine(new Vector3(x + cross_x * i, y + cross_y * i), new Vector3(x1 + cross_x * i, y1 + cross_y * i));
            }
        }
    }

    private float CurveFunction(float value)
    {
        return Mathf.Sin(value * Mathf.PI + 1.5F * Mathf.PI) / 2F + 0.5F;
    }

    #endregion Drawing service

    #region Collision

    private void CalculateCollision(Vector2 velocity, Node body)
    {
        if (configuration == null)
        {
            return;
        }

        var collidedNodes = configuration.nodes.FindAll(x => x != body && Extensions.IsCollidedCircle(body.position, x.position, collision_radius));

        if (collidedNodes.Count > 0)
        {
            collidedNodes.ForEach(node =>
            {
                Vector2 crossVelocity = (node.position.center - body.position.center).normalized;

                node.position.position += velocity + crossVelocity;

                CalculateCollision(velocity, node);
            });
        }
    }

    #endregion Collision

    private void InitializeIfNull()
    {
        if (selected_links == null)
        {
            selected_links = new List<Link>();
        }

        if (selected_nodes == null)
        {
            selected_nodes = new List<Node>();
        }

        if (inputController == null)
        {
            inputController = new InputController();
        }

        if (from_build_nodes == null)
        {
            from_build_nodes = new List<Node>();
        }
    }

    private void InitializeStyleIfNot()
    {
        if (nodeElipseSkin == null)
        {
            nodeElipseSkin = Resources.Load<GUISkin>("Skins/NodeEllipseSkin");
        }

        if (nodeSkin == null)
        {
            nodeSkin = Resources.Load<GUISkin>("Skins/NodeSkin");
        }
    }
}