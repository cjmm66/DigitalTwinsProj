using UnityEngine;

public class ClientOfferEvaluator : MonoBehaviour
{
    [Header("Client")]
    [SerializeField]
    private WarehouseClient client = new WarehouseClient();

    [Header("Current Contract")]
    [Tooltip("The storage price proposed by the warehouse.")]
    [Min(0f)]
    [SerializeField]
    private float offeredStoragePrice = 1000f;

    [Header("Willingness Configuration")]
    [Tooltip("Willingness when the offer equals the expected market price.")]
    [Range(0f, 100f)]
    [SerializeField]
    private float fairPriceWillingness = 75f;

    [Tooltip("Lowest possible willingness.")]
    [Range(0f, 100f)]
    [SerializeField]
    private float minimumWillingness = 5f;

    [Tooltip("Highest possible willingness.")]
    [Range(0f, 100f)]
    [SerializeField]
    private float maximumWillingness = 95f;

    [Header("Testing")]
    [Tooltip("Evaluate the offer automatically when Play Mode starts.")]
    [SerializeField]
    private bool evaluateOnStart = true;

    private void Start()
    {
        if (evaluateOnStart)
        {
            EvaluateCurrentOffer();
        }
    }

    [ContextMenu("Evaluate Current Offer")]
    public void EvaluateCurrentOffer()
    {
        OfferResult result = EvaluateOffer(
            client,
            offeredStoragePrice
        );

        PrintResult(result);
    }

    public OfferResult EvaluateOffer(
        WarehouseClient selectedClient,
        float offeredPrice)
    {
        if (selectedClient == null)
        {
            Debug.LogError("No client was provided.");
            return null;
        }

        if (selectedClient.expectedMarketPrice <= 0f)
        {
            Debug.LogError(
                "The expected market price must be greater than zero."
            );

            return null;
        }

        offeredPrice = Mathf.Max(0f, offeredPrice);

        float willingness = CalculateWillingness(
            selectedClient,
            offeredPrice
        );

        string category = EvaluateDecisionTree(willingness);

        float randomRoll = Random.Range(0f, 100f);
        bool accepted = randomRoll <= willingness;

        OfferResult result = new OfferResult
        {
            offeredPrice = offeredPrice,
            expectedMarketPrice =
                selectedClient.expectedMarketPrice,
            willingnessPercentage = willingness,
            randomRoll = randomRoll,
            willingnessCategory = category,
            accepted = accepted
        };

        return result;
    }

    private float CalculateWillingness(
        WarehouseClient selectedClient,
        float offeredPrice)
    {
        float priceDifferenceRatio =
            (
                selectedClient.expectedMarketPrice -
                offeredPrice
            )
            / selectedClient.expectedMarketPrice;

        float priceModifier =
            priceDifferenceRatio * 100f;

        float willingness =
            fairPriceWillingness +
            priceModifier;

        willingness +=
            selectedClient.reputationModifier;

        willingness +=
            selectedClient.containerPriorityModifier;

        willingness +=
            selectedClient.occupancyModifier;

        willingness +=
            selectedClient.relationshipModifier;

        willingness = Mathf.Clamp(
            willingness,
            minimumWillingness,
            maximumWillingness
        );

        return willingness;
    }

    private string EvaluateDecisionTree(float willingness)
    {
        if (willingness >= 80f)
        {
            return "High willingness";
        }

        if (willingness >= 50f)
        {
            return "Medium willingness";
        }

        if (willingness >= 25f)
        {
            return "Low willingness";
        }

        return "Very low willingness";
    }

    private void PrintResult(OfferResult result)
    {
        if (result == null)
        {
            return;
        }

        string finalDecision =
            result.accepted
                ? "ACCEPTED"
                : "REJECTED";

        Debug.Log(
            "===== STORAGE CONTRACT EVALUATION =====\n" +
            $"Client: {client.clientName}\n" +
            $"Expected market price: " +
            $"${result.expectedMarketPrice:F2}\n" +
            $"Offered storage price: " +
            $"${result.offeredPrice:F2}\n" +
            $"Willingness: " +
            $"{result.willingnessPercentage:F1}%\n" +
            $"Decision-tree category: " +
            $"{result.willingnessCategory}\n" +
            $"Random acceptance roll: " +
            $"{result.randomRoll:F1}\n" +
            $"Final decision: {finalDecision}\n" +
            "======================================="
        );
    }
}