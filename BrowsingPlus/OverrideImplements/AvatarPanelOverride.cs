using BrowsingPlus.PanelUI;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow.SceneStreaming;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;
using Il2CppSLZ.Marrow.Utilities;

namespace BrowsingPlus.OverrideImplements
{
    public class AvatarPanelOverride : PanelOverrider
    {
        public PanelContainer palletsContainer;
        public PanelContainer allContainer;
        public PanelContainer mainContainer;

        public AvatarPanelOverride() {
            mainContainer = new PanelContainer($"<color={Core.categoryColor}>Avatars", this, 1);

            palletsContainer = mainContainer.MakeContainer($"<color={Core.categoryColor}>Pallets");
            allContainer = mainContainer.MakeContainer($"<color={Core.categoryColor}>All");
        }

        public override void Populate(PanelView panelView)
        {
            initialized = false;
            panelButtons.Clear();

            originalPanelView = panelView;

            title = panelView.transform.Find("text_TITLE").GetComponent<TextMeshProUGUI>();
            pageIndicator = panelView.transform.Find("text_standing_val").GetComponent<TextMeshProUGUI>();

            for (int i = 1; i <= maxPanelView; i++)
            {
                panelButtons.Add(GetPanelButton(panelView.transform.Find($"button_Item_0{i}")));
            }

            returnButton = GetPanelButton(panelView.transform.Find("button_return"));
            nextButton = GetPanelButton(panelView.transform.Find("button_Forward"));
            prevButton = GetPanelButton(panelView.transform.Find("button_Back"));

            nextButton.SetClickAction(() =>
            {
                PageChange(true);
            });

            prevButton.SetClickAction(() =>
            {
                PageChange(false);
            });

            returnButton.SetClickAction(() =>
            {
                TryReturn();
            });
        }

        internal override PanelButton GetPanelButton(Transform transform)
        {
            GameObject gameObject = transform.gameObject;
            Button button = gameObject.GetComponent<Button>();

            Transform attemptedText = transform.Find("text_avatar_val");
            TextMeshPro title = null;

            if (attemptedText)
            {
                title = attemptedText.GetComponent<TextMeshPro>();
                title.richText = true;
            }


            RepairButton(button);

            PanelButton panelButton = new PanelButton()
            {
                buttonObject = gameObject,
                button = button,
                text = title
            };

            return panelButton;
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

            foreach (var quickCrateRef in Core.palletToAvatar["SLZ"])
            {
                slzContainer.AddEntry(quickCrateRef.title, () =>
                {
                    PlayerRefs.Instance.PlayerRigManager.SwapAvatarCrate(new Barcode(quickCrateRef.barcode));
                });

                allContainer.AddEntry(quickCrateRef.title, () =>
                {
                    PlayerRefs.Instance.PlayerRigManager.SwapAvatarCrate(new Barcode(quickCrateRef.barcode));
                });
            }
           

            foreach (var entry in Core.recentlyInstalledPallets)
            {
                if (entry == "SLZ") {
                    continue;
                }
                // Theres a avatar in here
                if (Core.palletToAvatar.ContainsKey(entry)) {
                    string palletName = Core.GetProperTitleForPallet(entry);

                    PanelContainer container = palletsContainer.MakeContainer($"<color={Core.categoryColor}>" + palletName);

                    foreach (var quickCrateRef in Core.palletToAvatar[entry])
                    {
                        container.AddEntry(quickCrateRef.title, () =>
                        {
                            PlayerRefs.Instance.PlayerRigManager.SwapAvatarCrate(new Barcode(quickCrateRef.barcode));
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

                // Theres a avatar in here
                if (Core.palletToAvatar.ContainsKey(entry))
                {
                    foreach (var quickCrateRef in Core.palletToAvatar[entry])
                    {
                        additionalAdd.Add(quickCrateRef);
                    }
                }
            }

            additionalAdd.Sort((x, y) => x.title.CompareTo(y.title));

            foreach (var added in additionalAdd)
            {
                allContainer.AddEntry(added.title, () =>
                {
                    PlayerRefs.Instance.PlayerRigManager.SwapAvatarCrate(new Barcode(added.barcode));
                });
            }
        }
    }
}
