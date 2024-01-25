using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.UnitTest.Utils
{
    internal class DummyDataProvider
    {
        public static IEnumerable<RewardTransaction> GetRewardTransactionDummyData()
        {
            return new List<RewardTransaction>()
            {
                new RewardTransaction()
                {
                    UserId = Guid.Parse("93d3b6d6-0187-4a87-be75-57f16e9eb253"),
                    DateTransition = new DateTime(2024, 12, 30, 2, 2, 2, DateTimeKind.Utc),
                    PointAfterTransaction = 20,
                    PointBeforeTransaction = 25,
                    PointTransition = -5,
                    RewardTransactionId = Guid.Parse("c574afbc-3285-4de6-8230-3970605e176f"),
                    Origin = new OriginJson()
                    {
                        Reason = Reason.ApplyCoupon,
                        DiscountType = CouponDiscountType.ByValue,
                        DiscountValue = 2000
                    }
                },
                new RewardTransaction()
                {
                    UserId = Guid.Parse("6a5b5e2a-965e-4750-b7c8-55700178b141"),
                    DateTransition = new DateTime(2024, 12, 30, 2, 2, 2, DateTimeKind.Utc),
                    PointAfterTransaction = 20,
                    PointBeforeTransaction = 25,
                    PointTransition = -5,
                    RewardTransactionId = Guid.Parse("99543604-b0a1-4f9e-8638-154c785839cb"),
                    Origin = new OriginJson()
                    {
                        Reason = Reason.ApplyCoupon,
                        DiscountType = CouponDiscountType.ByValue,
                        DiscountValue = 2000
                    }
                },
                new RewardTransaction()
                {
                    UserId = Guid.Parse("10a16a4b-21ca-4f39-989a-dce02d9d1c23"),
                    DateTransition = new DateTime(2024, 12, 30, 2, 2, 2, DateTimeKind.Utc),
                    PointAfterTransaction = 20,
                    PointBeforeTransaction = 25,
                    PointTransition = -5,
                    RewardTransactionId = Guid.Parse("7e4c3808-5274-49f5-99cc-f5236a1aa6e9"),
                    Origin = new OriginJson()
                    {
                        Reason = Reason.ApplyCoupon,
                        DiscountType = CouponDiscountType.ByValue,
                        DiscountValue = 2000
                    }
                }
            };
        }

        public static IEnumerable<UserRewardPoint> GetUserRewardPointDummyData()
        {
            yield return new UserRewardPoint
            {
                UserId = Guid.Parse("f351236d-3dd3-4989-8be4-6fe2c03338e8"),
                RewardPoint = 50
            };
            yield return new UserRewardPoint
            {
                UserId = Guid.Parse("21c41384-b0fa-441f-a6ce-83d796a741c2"),
                RewardPoint = 20
            };
            yield return new UserRewardPoint
            {
                UserId = Guid.Parse("32ff121b-fc3f-4f2b-a3e9-77cfb347881c"),
                RewardPoint = 0
            };
            yield return new UserRewardPoint
            {
                UserId = Guid.Parse("e31600b9-2b3e-45f6-ac6f-349a9650fe02"),
                RewardPoint = 500
            };
            yield return new UserRewardPoint
            {
                UserId = Guid.Parse("e31600b9-2b3e-45f6-ac6f-349a9650fe02"),
                RewardPoint = 40
            };
        }
    }
}
