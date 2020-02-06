using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIControls
{
    public class MenuItemsTree
    {
        private int currentLevel;
        private IDictionary<int, List<MenuItem>> levels;

        public MenuItemsTree(MenuItem[] items)
        {
            BuilNodeTree(items);
        }

        public List<MenuItem> GetItems()
        {
            if (levels.ContainsKey(currentLevel))
            {
                return levels[currentLevel];
            }

            return new List<MenuItem>();
        }

        public MenuItem GetItem(int Index)
        {
            if (levels.ContainsKey(currentLevel))
            {
                return levels[currentLevel][Index];
            }

            return null;
        }

        public int ChildNodesCount()
        {
            if (levels.ContainsKey(currentLevel))
            {
                return levels[currentLevel].Count;
            }

            return -1;
        }

        private void BuilNodeTree(MenuItem[] items)
        {
            levels = new Dictionary<int, List<MenuItem>>();

            Dictionary<string, int> keys = new Dictionary<string, int>();

            foreach (MenuItem item in items)
            {
                string[] array = item.Name.Split('/');

                for (int level = 0; level < array.Length; level++)
                {
                    string part = array[level];

                    if (!levels.ContainsKey(level))
                    {
                        levels.Add(level, new List<MenuItem>());
                    }

                    if (levels[level].Find(x => x.Name == part) == null)
                    {
                        Action action = null;

                        if (level < array.Length - 1)
                        {
                            action = () => { currentLevel++; };
                        }
                        else
                        {
                            action = () => { GUIContextMenu.Close(); item.Invoke(); };
                        }

                        levels[level].Add(new MenuItem(part, action));
                    }
                }
            }
        }
    }
}
