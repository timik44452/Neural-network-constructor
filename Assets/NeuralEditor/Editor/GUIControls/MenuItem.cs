using System;

namespace GUIControls
{
    public class MenuItem
    {
        public string Name { get; }

        private Action callback;

        public MenuItem(string Name, Action callback)
        {
            this.Name = Name;
            this.callback = callback;
        }

        public void Invoke()
        {
            callback?.Invoke();
        }
    }
}
