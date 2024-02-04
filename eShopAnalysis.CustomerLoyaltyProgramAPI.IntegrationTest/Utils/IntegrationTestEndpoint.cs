using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.IntegrationTest.Utils
{
    public static class IntegrationTestEndpoint
    {
        public const string BaseUrl = "api/CustomerLoyaltyProgramAPI/";
        public static class RewardTransaction
        {
            public const string GetUserRewardTransaction = BaseUrl + "RewardTransactionAPI/GetRewardTransactionsOfUser";
            public const string AddRewardTransactionForApplyCoupon = BaseUrl + "RewardTransactionAPI/BackChannel/AddRewardTransactionForApplyCoupon";
            public const string AddRewardTransactionForCompleteOrdering = BaseUrl + "RewardTransactionAPI/BackChannel/AddRewardTransactionForCompleteOrdering";
        }

        public static class UserRewardPoint
        {
            public const string GetRewardPointOfUser = BaseUrl + "UserRewardPointAPI/GetRewardPointOfUser";
            public const string AddUserRewardInstance = BaseUrl + "UserRewardPointAPI/AddUserRewardInstance";
            public const string AddRewardTransactionForCompleteOrdering = BaseUrl + "UserRewardPointAPI/DeleteUserRewardInstance";
        }
    }
}
