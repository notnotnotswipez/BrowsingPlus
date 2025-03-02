using BrowsingPlus.PanelUI;
using Il2CppSLZ.Bonelab;
using Il2CppTMPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BrowsingPlus
{
    public class PanelOverrider
    {
        internal PanelView originalPanelView;

        internal TextMeshProUGUI title;
        internal TextMeshProUGUI pageIndicator;
        internal List<PanelButton> panelButtons = new List<PanelButton>();
        internal PanelButton returnButton;
        internal PanelButton nextButton;
        internal PanelButton prevButton;

        internal int lastPage = 0;

        internal int maxPanelView = 5;
        internal int page = 0;
        internal int maxPages = 0;

        internal PanelContainer currentContainer;
        internal List<PanelEntry> allPanelEntries;

        public bool initialized = false;

        public virtual void Populate(PanelView panelView) {
            initialized = false;

            panelButtons.Clear();

            originalPanelView = panelView;

            title = panelView.transform.Find("text_TITLE").GetComponent<TextMeshProUGUI>();
            pageIndicator = panelView.transform.Find("text_standing_val").GetComponent<TextMeshProUGUI>();

            for (int i = 1; i <= maxPanelView; i++)
            {
                panelButtons.Add(GetPanelButton(panelView.transform.Find($"button_Item 0{i}")));
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

        public void ForceUpdate() {
            OpenContainer(currentContainer, page);
        }

        public void TryInitialize(PanelView panelView) {
            if (panelView.GetInstanceID() != originalPanelView.GetInstanceID()) {
                return;
            }
            if (!initialized)
            {
                OnInitialized();
                initialized = true;
            }
            else {
                ForceUpdate();
            }
        }

        public virtual void OnInitialized() {
            
        }

        public void TryReturn() {
            if (currentContainer.parent != null)
            {
                OpenContainer(currentContainer.parent, lastPage);
            }
            else {
                originalPanelView.CloseMenu();
            }
        }

        public void PageChange(bool increment) {
            if (increment)
            {
                page++;
                if (page > maxPages)
                {
                    page--;
                    return;
                }

                OpenPage(page);
            }
            else {
                page--;
                if (page < 0)
                {
                    page++;
                    return;
                }

                OpenPage(page);
            }
        }

        public void OpenContainer(PanelContainer panelContainer, int pageOverride = 0) {
            currentContainer = panelContainer;
            allPanelEntries = currentContainer.children;

            title.text = panelContainer.title;

            lastPage = page;
            page = pageOverride;
            maxPages = Mathf.FloorToInt((float) allPanelEntries.Count / (float) maxPanelView);

            if (allPanelEntries.Count % maxPanelView == 0) {
                maxPages--;
            }

            if (page > maxPages) {
                page = 0;
            }

            OpenPage(page);
        }

        private void OpenPage(int page) {
            prevButton.SetActiveState(false);
            nextButton.SetActiveState(false);

            foreach (var panelButton in panelButtons) {
                panelButton.SetActiveState(false);
            }

            int start = page * maxPanelView;

            for (int i = 0; i < maxPanelView; i++) {
                int peekIndex = start + i;
                if (peekIndex < allPanelEntries.Count)
                {
                    PanelEntry panelEntry = allPanelEntries[peekIndex];
                    PanelButton panelButton = panelButtons[i + currentContainer.buttonOffset];

                    panelButton.SetActiveState(true);
                    panelButton.SetClickAction(panelEntry.onClick);
                    panelButton.TryChangeTitle(panelEntry.displayName);
                }
                else {
                    break;
                }
            }

            if (page > 0) {
                prevButton.SetActiveState(true);
            }

            if (page < maxPages) {
                nextButton.SetActiveState(true);
            }

            string targetPageText = $"{page+1} / {maxPages+1}";

            if (maxPages == 0) {
                targetPageText = " ";
            }

            pageIndicator.text = targetPageText;
        }

        internal virtual PanelButton GetPanelButton(Transform transform) {
            GameObject gameObject = transform.gameObject;
            Button button = gameObject.GetComponent<Button>();

            Transform attemptedText = transform.Find("text_standing_val");
            TextMeshProUGUI title = null;

            if (attemptedText) {
                title = attemptedText.GetComponent<TextMeshProUGUI>();
                title.richText = true;
            }
            

            RepairButton(button);

            PanelButton panelButton = new PanelButton() {
                buttonObject = gameObject,
                button = button,
                text = title
            };

            return panelButton;
        }

        internal void RepairButton(Button button) {
            Navigation noAutomaticNav = Navigation.defaultNavigation;
            noAutomaticNav.mode = Navigation.Mode.None;

            button.navigation = noAutomaticNav;
            button.onClick.m_PersistentCalls.Clear();
        }
    }

    public struct PanelButton {
        public GameObject buttonObject;
        public Button button;
        public TMP_Text text;

        public void SetActiveState(bool activeState) {
            buttonObject.SetActive(activeState);
        }

        public void SetClickAction(Action action) {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action);
        }

        public void TryChangeTitle(string newTitle) {
            if (text) {
                text.text = newTitle;
            }
        }
    }
}
