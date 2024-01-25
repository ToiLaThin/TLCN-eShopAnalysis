using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;
using eShopAnalysis.CustomerLoyaltyProgramAPI.UnitOfWork;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.UnitTest.UnitOfWork
{
    public class UnitTestUnitOfWork: BaseUnitTestUnitOfWork
    {
        protected CustomerLoyaltyProgramAPI.UnitOfWork.UnitOfWork UnitOfWork { get; set; }
        public UnitTestUnitOfWork() {
            this.SeedDb();
            UnitOfWork = new CustomerLoyaltyProgramAPI.UnitOfWork.UnitOfWork(PostgresDbContext);
        }

        public override void SeedDb()
        {
            base.SeedDb();
            if (PostgresDbContext.Database.EnsureCreated() == false)
            {
                return;
            }
            PostgresDbContext.RewardTransactions.AddRange(DummyRewardTranData);
            PostgresDbContext.SaveChanges();
        }


        [Fact]
        public async Task WhenAddNonExistingRewardTransWithoutCommiting_ReturnTestWithRewardTransNotAdded()
        {
            //Arrange
            RewardTransaction nonExistingToAddRewardTrans = new RewardTransaction()
            {
                UserId = Guid.Parse("e5a4fa89-0c63-4efd-bd55-9cfd559926cf"),
                DateTransition = new DateTime(2028, 12, 30, 2, 2, 2, DateTimeKind.Utc),
                PointAfterTransaction = 200,
                PointBeforeTransaction = 250,
                PointTransition = -50,
                RewardTransactionId = Guid.Parse("2dd84318-e5b8-42b5-9971-d8b965a07d5f"),
                Origin = new OriginJson()
                {
                    Reason = Reason.ApplyCoupon,
                    DiscountType = CouponDiscountType.ByValue,
                    DiscountValue = 20000
                }
            };
            Guid addedRewardTransId = nonExistingToAddRewardTrans.RewardTransactionId;
            using var transaction = await UnitOfWork.BeginTransactionAsync();

            //Act
            var addedRewardTrans= await UnitOfWork.RewardTransactionRepository.AddAsync(nonExistingToAddRewardTrans);
            //really call to db not just get from change tracker, (use FindAsync will give the test failed result)
            var findAddedRewardTransExpectToBeNullBeforeCommit = PostgresDbContext.RewardTransactions.Where(rt => rt.RewardTransactionId.Equals(addedRewardTransId)).ToList().FirstOrDefault();

            //Assert
            addedRewardTrans.Should().NotBeNull();
            addedRewardTrans.Should().BeAssignableTo<RewardTransaction>();
            addedRewardTrans.Should().BeEquivalentTo(nonExistingToAddRewardTrans);
            findAddedRewardTransExpectToBeNullBeforeCommit.Should().BeNull();

            await UnitOfWork.CommitTransactionAsync(transaction);
            var findAddedRewardTransExpectNotNullAfterCommit = PostgresDbContext.RewardTransactions.Where(rt => rt.RewardTransactionId.Equals(addedRewardTransId)).ToList().FirstOrDefault();

            findAddedRewardTransExpectNotNullAfterCommit.Should().NotBeNull();
            findAddedRewardTransExpectNotNullAfterCommit.Should().BeAssignableTo<RewardTransaction>();
            findAddedRewardTransExpectNotNullAfterCommit.Should().BeEquivalentTo(addedRewardTrans);
            findAddedRewardTransExpectNotNullAfterCommit.Should().BeEquivalentTo(nonExistingToAddRewardTrans);
        }
    }
}
