using HarmonyLib;
using Il2CppSLZ.Marrow.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowsingPlus.Patches
{
    [HarmonyPatch(typeof(AssetWarehouse), nameof(AssetWarehouse.UnloadPallet), typeof(Barcode))]
    public class WarehouseUnLoadPatchBarcode
    {
        public static void Postfix(Barcode barcode) {
            Core.RemovePalletReference(barcode.ID);
        }
    }

    [HarmonyPatch(typeof(AssetWarehouse), nameof(AssetWarehouse.UnloadPallet), typeof(Pallet))]
    public class WarehouseUnLoadPatchPallet
    {
        public static void Postfix(Pallet pallet)
        {
            Core.RemovePalletReference(pallet.Barcode.ID);
        }
    }
}
