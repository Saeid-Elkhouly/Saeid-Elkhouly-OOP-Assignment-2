using SrpLab;

var basket = new CheckoutBasket();

basket.AddLine("A100", 50m, 2);
basket.AddLine("B200", 30m, 1);

basket.ApplyCouponText("SAVE10");
basket.EnableGiftWrap();

var couponParser = new CouponParser();
var pricingCalculator = new PricingCalculator(couponParser);
var giftMessageGenerator = new GiftMessageGenerator();
var paymentAuthorizer = new PaymentAuthorizer();

var subTotal = pricingCalculator.CalculateSubTotal(basket.Lines);

var discount = pricingCalculator.CalculateDiscount(
    basket.Lines,
    basket.CouponRaw);

var grandTotal = pricingCalculator.CalculateGrandTotal(
    basket.Lines,
    basket.CouponRaw,
    basket.GiftWrapEnabled);

var giftMessage = giftMessageGenerator.Generate(
    "Ali",
    basket.Lines,
    grandTotal);

var authorizationCode = paymentAuthorizer.Authorize(
    grandTotal,
    "1234",
    basket.Lines.Count);
