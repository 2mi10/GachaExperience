using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GachaService : MonoBehaviour
{
    private const string DataPath = "gacha_data";

    public async UniTask<GachaDataList> GetGachaDataAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = Resources.LoadAsync<TextAsset>(DataPath);

            await request.ToUniTask(
                cancellationToken: cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            var jsonAsset = request.asset as TextAsset;

            if (jsonAsset == null)
            {
                Debug.LogError(
                    $"Resources/{DataPath}.json が見つかりません。");
                return null;
            }

            if (string.IsNullOrWhiteSpace(jsonAsset.text))
            {
                Debug.LogError("ガチャデータが空です。");
                return null;
            }

            var dataList =
                JsonUtility.FromJson<GachaDataList>(
                    jsonAsset.text);

            if (dataList == null ||
                dataList.items == null ||
                dataList.items.Length == 0)
            {
                Debug.LogError("ガチャデータがありません。");
                return null;
            }

            return dataList;
        }
        catch (OperationCanceledException)
        {
            Debug.Log("ガチャデータ取得をキャンセルしました。");
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError($"ガチャデータ取得エラー: {e}");
            return null;
        }
    }

    public async UniTask<GachaDataModel> GetGachaResultAsync(
        CancellationToken cancellationToken = default)
    {
        var dataList =
            await GetGachaDataAsync(cancellationToken);

        if (dataList == null)
        {
            return null;
        }

        var result = GetRandomResult(dataList.items);

        if (result == null)
        {
            Debug.LogError("ガチャ結果が取得できませんでした。");
            return null;
        }

        if (string.IsNullOrWhiteSpace(result.rarity))
        {
            Debug.LogError("rarityが設定されていません。");
            return null;
        }

        if (!result.TryGetRarity(out GachaRarity rarity))
        {
            Debug.LogError(
                $"不正なレアリティです: {result.rarity}");
            return null;
        }

        result.rarity = rarity.ToString();

        return result;
    }

    private GachaDataModel GetRandomResult(
        GachaDataModel[] items)
    {
        float totalWeight = 0f;

        foreach (var item in items)
        {
            if (item != null && item.weight > 0)
            {
                totalWeight += item.weight;
            }
        }

        if (totalWeight <= 0f)
        {
            Debug.LogError("ガチャ確率が設定されていません。");
            return null;
        }

        float randomValue =
            UnityEngine.Random.Range(0f, totalWeight);

        foreach (var item in items)
        {
            if (item == null || item.weight <= 0)
            {
                continue;
            }

            randomValue -= item.weight;

            if (randomValue <= 0f)
            {
                return item;
            }
        }

        return null;
    }

    private async void Start()
    {
        var result =
            await GetGachaResultAsync(destroyCancellationToken);

        if (result == null)
        {
            return;
        }

        Debug.Log(
            $"ガチャ結果: {result.rarity} / " +
            $"{result.itemName}");
    }
}
