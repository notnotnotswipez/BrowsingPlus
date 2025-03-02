using BrowsingPlus.OverrideImplements;
using Harmony;
using Il2CppSLZ.Bonelab.SaveData;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Warehouse;
using MelonLoader;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine.UI;
using static Il2CppSLZ.Bonelab.CrateFilters;

[assembly: MelonInfo(typeof(BrowsingPlus.Core), "BrowsingPlus", "1.0.0", "notnotnotswipez", null)]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]

namespace BrowsingPlus
{
    public struct QuickCrateRef {
        public string barcode;
        public string title;
    }

    public class Core : MelonMod
    {
        public static List<string> recentlyInstalledPallets = new List<string>();
        public static Dictionary<string, List<QuickCrateRef>> palletToLevel = new Dictionary<string, List<QuickCrateRef>>();
        public static Dictionary<string, List<QuickCrateRef>> palletToAvatar = new Dictionary<string, List<QuickCrateRef>>();

        private static Dictionary<string, string> palletTitleCache = new Dictionary<string, string>();

        public static LevelPanelOverride levelsPanelOverride;
        public static AvatarPanelOverride avatarPanelOverride;

        public const string categoryColor = "#CAB3F9";

        public override void OnInitializeMelon()
        {
            AssetWarehouse.OnReady(new Action(() => {
                PopulateRecentlyInstalledPallets(MarrowSDK.RuntimeModsPath);

                foreach (var crate in AssetWarehouse.Instance._crateRegistry.Values) {
                    HandleCrate(crate);
                }


                // MODS ADDED DURING RUNTIME HANDLING.
                AssetWarehouse.Instance.OnCrateAdded += new Action<Barcode>((br) =>
                {
                    Crate crateInQuestion = AssetWarehouse.Instance._crateRegistry[br];

                    if (!recentlyInstalledPallets.Contains(crateInQuestion.Pallet.Barcode.ID))
                    {
                        recentlyInstalledPallets.Insert(0, crateInQuestion.Pallet.Barcode.ID);
                    }

                    HandleCrate(crateInQuestion, true);
                });
            }));

            levelsPanelOverride = new LevelPanelOverride();
            avatarPanelOverride = new AvatarPanelOverride();
        }

        public static string GetProperTitleForPallet(string palletBarcode) {
            string title;

            if (palletTitleCache.ContainsKey(palletBarcode))
            {
                title = palletTitleCache[palletBarcode];
            }
            else {
                title = AssetWarehouse.Instance._palletRegistry[new Barcode(palletBarcode)].Title;
                palletTitleCache.Add(palletBarcode, title);
            }

            return title;
        }

        public static void RemovePalletReference(string palletBarcode) {
            recentlyInstalledPallets.Remove(palletBarcode);
            palletToAvatar.Remove(palletBarcode);
            palletToLevel.Remove(palletBarcode);

            levelsPanelOverride.PopulateMenus();
            avatarPanelOverride.PopulateMenus();
        }

        private void HandleCrate(Crate crate, bool updateIfPossible = false) {
            var avatarCrate = crate.TryCast<AvatarCrate>();
            var levelCrate = crate.TryCast<LevelCrate>();
            string palletBarcode = crate.Pallet.Barcode.ID;

            if (palletBarcode.StartsWith("SLZ.")) {
                palletBarcode = "SLZ";
            }

            if (crate.Redacted) {
                return;
            }

            if (crate.Unlockable) {
                if (DataManager.ActiveSave.Unlocks.UnlockCountForBarcode(crate.Barcode) <= 0)
                {
                    return;
                }
            }
            

            if (avatarCrate) {
                if (!palletToAvatar.ContainsKey(palletBarcode)) {
                    palletToAvatar.Add(palletBarcode, new List<QuickCrateRef>());
                }

                palletToAvatar[palletBarcode].Add(new QuickCrateRef() {
                    barcode = crate.Barcode.ID,
                    title = crate.Title
                });

                palletToAvatar[palletBarcode].Sort((x, y) => x.title.CompareTo(y.title));

                if (updateIfPossible)
                {
                    avatarPanelOverride.PopulateMenus();
                }
            }

            if (levelCrate)
            {
                if (!palletToLevel.ContainsKey(palletBarcode))
                {
                    palletToLevel.Add(palletBarcode, new List<QuickCrateRef>());
                }

                palletToLevel[palletBarcode].Add(new QuickCrateRef()
                {
                    barcode = crate.Barcode.ID,
                    title = crate.Title
                });

                palletToLevel[palletBarcode].Sort((x, y) => x.title.CompareTo(y.title));

                if (updateIfPossible) {
                    levelsPanelOverride.PopulateMenus();
                }
            }
        }

        private void PopulateRecentlyInstalledPallets(string directory) {
            recentlyInstalledPallets.Clear();
            palletToLevel.Clear();
            palletToAvatar.Clear();

            Thread thread = new Thread(() =>
            {
                var filesInOrder = new DirectoryInfo(directory).GetFiles()
                        .OrderByDescending(f => f.LastWriteTime)
                        .Select(f => f.FullName)
                        .ToList();

                foreach (var manifestTest in filesInOrder) {
                    if (!manifestTest.EndsWith(".manifest")) {
                        continue;
                    }

                    dynamic manifest = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(manifestTest));
                    string palletBarcode = manifest["objects"]["1"]["palletBarcode"];

                    if (recentlyInstalledPallets.Contains(palletBarcode)) {
                        continue;
                    }

                    if (palletBarcode.StartsWith("SLZ."))
                    {
                        continue;
                    }

                    recentlyInstalledPallets.Add(palletBarcode);
                }
            });

            thread.Start();
        }
    }
}