using BrowsingPlus.PanelUI;
using Il2CppSLZ.Marrow.SceneStreaming;
using Il2CppSLZ.Marrow.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowsingPlus.OverrideImplements
{
    public class LevelPanelOverride : PanelOverrider
    {
        public PanelContainer palletsContainer;
        public PanelContainer allContainer;
        public PanelContainer mainContainer;

        public LevelPanelOverride() {
            mainContainer = new PanelContainer($"<color={Core.categoryColor}>Levels", this, 1);

            palletsContainer = mainContainer.MakeContainer($"<color={Core.categoryColor}>Pallets");
            allContainer = mainContainer.MakeContainer($"<color={Core.categoryColor}>All");
        }

        public override void OnInitialized()
        {
            PopulateMenus();
            OpenContainer(mainContainer);
        }

        public void PopulateMenus() {
            palletsContainer.Clear();
            allContainer.Clear();

            // SLZ comes first!
            PanelContainer slzContainer = palletsContainer.MakeContainer($"<color={Core.categoryColor}>SLZ");

            foreach (var quickCrateRef in Core.palletToLevel["SLZ"])
            {
                slzContainer.AddEntry(quickCrateRef.title, () =>
                {
                    SceneStreamer.Load(new Barcode(quickCrateRef.barcode));
                });

                allContainer.AddEntry(quickCrateRef.title, () =>
                {
                    SceneStreamer.Load(new Barcode(quickCrateRef.barcode));
                });
            }
           

            foreach (var entry in Core.recentlyInstalledPallets)
            {
                if (entry == "SLZ") {
                    continue;
                }
                // Theres a level in here
                if (Core.palletToLevel.ContainsKey(entry)) {
                    string palletName = Core.GetProperTitleForPallet(entry);

                    PanelContainer container = palletsContainer.MakeContainer($"<color={Core.categoryColor}>" + palletName);

                    foreach (var quickCrateRef in Core.palletToLevel[entry])
                    {
                        container.AddEntry(quickCrateRef.title, () =>
                        {
                            SceneStreamer.Load(new Barcode(quickCrateRef.barcode), new Barcode("SLZ.BONELAB.CORE.Level.LevelModLevelLoad"));
                        });
                    }
                }
            }

            List<QuickCrateRef> additionalAdd = new List<QuickCrateRef>();

            foreach (var entry in Core.recentlyInstalledPallets)
            {
                if (entry == "SLZ")
                {
                    continue;
                }

                // Theres a level in here
                if (Core.palletToLevel.ContainsKey(entry))
                {
                    foreach (var quickCrateRef in Core.palletToLevel[entry])
                    {
                        additionalAdd.Add(quickCrateRef);
                    }
                }
            }

            additionalAdd.Sort((x, y) => x.title.CompareTo(y.title));

            foreach (var added in additionalAdd) {
                allContainer.AddEntry(added.title, () =>
                {
                    SceneStreamer.Load(new Barcode(added.barcode), new Barcode("SLZ.BONELAB.CORE.Level.LevelModLevelLoad"));
                });
            }

            
        }
    }
}
