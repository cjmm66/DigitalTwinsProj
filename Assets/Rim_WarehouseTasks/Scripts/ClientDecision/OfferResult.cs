using System;

[Serializable]
public class OfferResult
{
    public float offeredPrice;
    public float expectedMarketPrice;
    public float willingnessPercentage;
    public float randomRoll;
    public string willingnessCategory;
    public bool accepted;
}