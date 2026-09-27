using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GachaPresenter : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button gachaButton;
    [SerializeField] private TMP_Text rarityText;
    [SerializeField] private RectTransform gachaCard;
    [SerializeField] private Image gachaCardImage;
    [SerializeField] private TMP_Text rateText;

    [Header("Service")]
    [SerializeField] private GachaService gachaService;

    private Vector3 originalScale;
    private Vector2 originalPosition;
    private Quaternion originalRotation;

    private bool isGachaPlaying;

    private void Awake()
    {
        if (gachaButton == null)
        {
            Debug.LogError("GachaButtonが設定されていません。");
            return;
        }

        if (rarityText == null)
        {
            Debug.LogError("RarityTextが設定されていません。");
            return;
        }

        if (gachaCard == null)
        {
            Debug.LogError("GachaCardが設定されていません。");
            return;
        }

        if (gachaCardImage == null)
        {
            Debug.LogError("GachaCardのImageが設定されていません。");
            return;
        }

        if (rateText == null)
        {
            Debug.LogError("GachaRateTextが設定されていません。");
            return;
        }

        if (gachaService == null)
        {
            Debug.LogError("GachaServiceが設定されていません。");
            return;
        }

        originalScale = gachaCard.localScale;
        originalPosition = gachaCard.anchoredPosition;
        originalRotation = gachaCard.localRotation;

        gachaButton.onClick.AddListener(OnGachaButtonClicked);
    }

    private async void Start()
    {
        var dataList =
            await gachaService.GetGachaDataAsync(
                destroyCancellationToken);

        if (dataList == null)
        {
            return;
        }

        ShowRates(dataList.items);
    }

    private void OnDestroy()
    {
        if (gachaButton != null)
        {
            gachaButton.onClick.RemoveListener(OnGachaButtonClicked);
        }

        if (gachaCard != null)
        {
            gachaCard.DOKill();
        }

        if (gachaCardImage != null)
        {
            gachaCardImage.DOKill();
        }
    }

    private void OnGachaButtonClicked()
    {
        if (isGachaPlaying)
        {
            return;
        }

        PlayGachaAsync().Forget();
    }

    private async UniTaskVoid PlayGachaAsync()
    {
        isGachaPlaying = true;
        gachaButton.interactable = false;

        try
        {
            GachaDataModel result =
                await gachaService.GetGachaResultAsync(
                    destroyCancellationToken);

            if (result == null)
            {
                rarityText.text = "ERROR";
                return;
            }

            rarityText.text = result.rarity;

            ApplyRarityColor(result.rarity);

            Sequence sequence =
                PlayRevealAnimation(result.rarity);

            await sequence.AsyncWaitForCompletion();

            Debug.Log(
                $"ガチャ結果表示：{result.rarity}\n" +
                $"Item: {result.itemName}\n" +
                $"Effect: {result.effectType}");
        }
        finally
        {
            isGachaPlaying = false;
            gachaButton.interactable = true;
        }
    }

    private void ShowRates(GachaDataModel[] items)
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
            rateText.text = "排出率を取得できません。";
            return;
        }

        rateText.text = "排出率\n";

        foreach (var item in items)
        {
            if (item == null || item.weight <= 0)
            {
                continue;
            }

            float rate =
                item.weight / totalWeight * 100f;

            rateText.text +=
                $"{item.rarity} {rate:0.#}%\n";
        }
    }

    private void ApplyRarityColor(string rarity)
    {
        if (rarity == "SR")
        {
            gachaCardImage.color =
                new Color32(120, 180, 255, 255);

            rarityText.color = Color.white;
            return;
        }

        if (rarity == "SSR")
        {
            gachaCardImage.color =
                new Color32(255, 215, 80, 255);

            rarityText.color = Color.white;
            return;
        }

        if (rarity == "UR")
        {
            gachaCardImage.color =
                new Color32(180, 100, 255, 255);

            rarityText.color = Color.white;
            return;
        }

        Debug.LogWarning(
            $"未対応のレアリティです: {rarity}");

        gachaCardImage.color = Color.white;
        rarityText.color = Color.black;
    }

    private Sequence PlayRevealAnimation(string rarity)
    {
        gachaCard.DOKill();
        gachaCardImage.DOKill();

        gachaCard.localScale =
            originalScale * 0.8f;

        gachaCard.anchoredPosition =
            originalPosition;

        gachaCard.localRotation =
            originalRotation;

        if (rarity == "SR")
        {
            return PlaySrAnimation();
        }

        if (rarity == "SSR")
        {
            return PlaySsrAnimation();
        }

        if (rarity == "UR")
        {
            return PlayUrAnimation();
        }

        Sequence defaultSequence = DOTween.Sequence();

        defaultSequence.Append(
            gachaCard
                .DOScale(
                    originalScale,
                    0.25f)
                .SetEase(Ease.OutBack));

        return defaultSequence;
    }

    private Sequence PlaySrAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            gachaCard
                .DOScale(
                    originalScale * 1.05f,
                    0.25f)
                .SetEase(Ease.OutBack));

        sequence.Append(
            gachaCard
                .DORotate(
                    new Vector3(0f, 0f, -4f),
                    0.08f));

        sequence.Append(
            gachaCard
                .DORotate(
                    new Vector3(0f, 0f, 4f),
                    0.08f));

        sequence.Append(
            gachaCard
                .DORotate(
                    originalRotation.eulerAngles,
                    0.08f));

        sequence.Append(
            gachaCard
                .DOScale(
                    originalScale,
                    0.15f)
                .SetEase(Ease.OutQuad));

        return sequence;
    }

    private Sequence PlaySsrAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            gachaCard
                .DOScale(
                    originalScale * 1.15f,
                    0.3f)
                .SetEase(Ease.OutBack));

        sequence.Join(
            gachaCard
                .DOShakeRotation(
                    0.35f,
                    new Vector3(0f, 0f, 8f),
                    15,
                    90f));

        sequence.Append(
            gachaCard
                .DOScale(
                    originalScale * 0.95f,
                    0.1f)
                .SetEase(Ease.InOutQuad));

        sequence.Append(
            gachaCard
                .DOScale(
                    originalScale,
                    0.15f)
                .SetEase(Ease.OutQuad));

        return sequence;
    }

    private Sequence PlayUrAnimation()
    {
        Color urColor =
            new Color32(180, 100, 255, 255);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            gachaCard
                .DOScale(
                    originalScale * 1.25f,
                    0.35f)
                .SetEase(Ease.OutBack));

        sequence.Join(
            gachaCard
                .DOShakeRotation(
                    0.4f,
                    new Vector3(0f, 0f, 12f),
                    20,
                    90f));

        sequence.Join(
            gachaCardImage
                .DOColor(
                    Color.white,
                    0.08f));

        sequence.Append(
            gachaCardImage
                .DOColor(
                    urColor,
                    0.12f));

        sequence.Append(
            gachaCard
                .DOScale(
                    originalScale * 0.92f,
                    0.1f)
                .SetEase(Ease.InOutQuad));

        sequence.Append(
            gachaCard
                .DOScale(
                    originalScale * 1.08f,
                    0.12f)
                .SetEase(Ease.OutBack));

        sequence.Append(
            gachaCard
                .DOScale(
                    originalScale,
                    0.15f)
                .SetEase(Ease.OutQuad));

        return sequence;
    }
}
