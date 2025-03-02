using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowsingPlus.PanelUI
{
    public class PanelContainer
    {
        public PanelOverrider overrider;

        public PanelContainer parent;
        public string title;

        public List<PanelEntry> children = new List<PanelEntry>();

        public int buttonOffset = 0;

        public PanelContainer(string title, PanelOverrider overrider, int buttonOffset = 0)
        {
            this.title = title;
            this.overrider = overrider;
            this.buttonOffset = buttonOffset;
        }

        public void Clear() {
            children.Clear();
        }

        public void AddEntry(string title, Action onClick) {
            children.Add(new PanelEntry()
            {
                displayName = title,
                onClick = onClick
            });
        }

        public void AddEntry(int insert, string title, Action onClick)
        {
            children.Insert(insert, new PanelEntry()
            {
                displayName = title,
                onClick = onClick
            });
        }

        public PanelContainer MakeContainer(string title) {
            PanelContainer container = new PanelContainer(title, overrider);
            container.parent = this;

            children.Add(new PanelEntry() {
                displayName = title,
                redirect = container,
                onClick = () => {
                    overrider.OpenContainer(container);
                }
            });

            return container;
        }
    }

    public struct PanelEntry {
        public string displayName;
        public PanelContainer redirect;
        public Action onClick;
    }
}
