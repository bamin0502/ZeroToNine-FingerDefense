using UnityEngine;

public static class EffectFactory
{
    private static string EffectFile = "Effects/{0}";

    public static EffectController CreateEffect(int effectId)
    {
        var assetTable = DataTableManager.Get<AssetListTable>(DataTableIds.Asset);
        var effectFilePath = string.Format(EffectFile, assetTable.Get(effectId));
        var effectResource = Resources.Load<EffectController>(effectFilePath);
        if (effectResource == null)
            return null;
        var effect = GameObject.Instantiate(effectResource);

        return effect;
    }
}