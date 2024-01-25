using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Repository;
using FluentAssertions;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.UnitTest.Repository
{
    public class UnitTestRewardTransactionRepository: BaseUnitTestRepository
    {
        protected CustomerLoyaltyProgramAPI.Repository.RewardTransactionRepository RewardTransactionRepository { get; set; }
        public UnitTestRewardTransactionRepository() {
            this.SeedDb();
            RewardTransactionRepository = new RewardTransactionRepository(PostgresDbContext);

        }

        public override void SeedDb()
        {
            base.SeedDb();
            if (PostgresDbContext.Database.EnsureCreated() == false) {
                return;
            }
            PostgresDbContext.RewardTransactions.AddRange(DummyRewardTranData);
            PostgresDbContext.SaveChanges();
        }


        [Fact]
        public void WhenGetAsQueryable_ReturnQueryableResult()
        {
            //Arrange
            IQueryable<RewardTransaction> expectedResult = DummyRewardTranData.AsQueryable();


            //Act
            var actualResult = RewardTransactionRepository.GetAsQueryable();

            //Assert
            actualResult.Should().NotBeNullOrEmpty();
            actualResult.Should().BeAssignableTo<object>();
            actualResult.Should().BeAssignableTo<IEnumerable<RewardTransaction>>();
            actualResult.Should().BeAssignableTo<IQueryable<RewardTransaction>>();
            actualResult.Should().BeEquivalentTo(expectedResult);
            actualResult.Select(rt => rt.Origin)
                        .Should()
                        .BeEquivalentTo(expectedResult.Select(rt => rt.Origin));
        }

        [Fact]
        public void WhenGetRewaredTrans_ReturnRightResult()
        {
            //Arrange
            RewardTransaction expectedResult = DummyRewardTranData.First();
            Guid expectedRewardTransId = expectedResult.RewardTransactionId;


            //Act
            var actualResult = RewardTransactionRepository.Get(expectedRewardTransId);

            //Assert
            actualResult.Should().NotBeNull();
            actualResult.Should().BeAssignableTo<object>();
            actualResult.Should().BeAssignableTo<RewardTransaction>();
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task WhenGetRewaredTransAsync_ReturnRightResult()
        {
            //Arrange
            RewardTransaction expectedResult = DummyRewardTranData.First();
            Guid expectedRewardTransId = expectedResult.RewardTransactionId;


            //Act
            var actualResult = await RewardTransactionRepository.GetAsync(expectedRewardTransId);

            //Assert
            actualResult.Should().NotBeNull();
            actualResult.Should().BeAssignableTo<object>();
            actualResult.Should().BeAssignableTo<RewardTransaction>();
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void GivenANonExistingRewardTrans_WhenAddRewaredTrans_ReturnAddedRewardTrans()
        {
            //Arrange toAddRewardTrans
            RewardTransaction toAddRewardTrans = new RewardTransaction()
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
            Guid addedRewardTransId = toAddRewardTrans.RewardTransactionId;


            //Act
            var addedRewardTrans = RewardTransactionRepository.Add(toAddRewardTrans);
            var actualRewardTrans = PostgresDbContext.RewardTransactions.Find(addedRewardTransId);

            //Assert
            actualRewardTrans.Should().NotBeNull();
            actualRewardTrans.Should().BeAssignableTo<RewardTransaction>();
            addedRewardTrans.Should().NotBeNull();
            addedRewardTrans.Should().BeAssignableTo<RewardTransaction>();
            addedRewardTrans.Should().BeEquivalentTo(actualRewardTrans);
            addedRewardTrans.Should().BeEquivalentTo(toAddRewardTrans);

            //After this test it will be clean up by base Dispose() so we do not have to delete the added trans
        }

        [Fact]
        public async Task GivenANonExistingRewardTrans_WhenAddRewaredTransAsync_ReturnAddedRewardTrans()
        {
            //Arrange
            RewardTransaction toAddRewardTrans = new RewardTransaction()
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
            Guid addedRewardTransId = toAddRewardTrans.RewardTransactionId;


            //Act
            var addedRewardTrans = await RewardTransactionRepository.AddAsync(toAddRewardTrans);
            var actualRewardTrans = await PostgresDbContext.RewardTransactions.FindAsync(addedRewardTransId);

            //Assert
            actualRewardTrans.Should().NotBeNull();
            actualRewardTrans.Should().BeAssignableTo<RewardTransaction>();
            addedRewardTrans.Should().NotBeNull();
            addedRewardTrans.Should().BeAssignableTo<RewardTransaction>();
            addedRewardTrans.Should().BeEquivalentTo(actualRewardTrans);
            addedRewardTrans.Should().BeEquivalentTo(toAddRewardTrans);
        }


        [Fact]
        public async Task GivenAnAlreadyExistingRewardTrans_WhenAddRewaredTransAsync_ReturnTestWithException()
        {
            //Arrange
            RewardTransaction existingToAddRewardTrans = new RewardTransaction()
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
            };

            //Act
            //Assert
            await Assert.ThrowsAnyAsync<Exception>(() => RewardTransactionRepository.AddAsync(existingToAddRewardTrans));  //please see the exception, if there is, then return null

        }

        [Fact]
        public void GivenAnAlreadyExistingRewardTrans_WhenDeleteRewaredTrans_ReturnDeletedRewardTransAndDeletedVerified()
        {
            //Arrange, the dummy data already in the db
            RewardTransaction existingToDeleteRewardTrans = DummyRewardTranData.First();
            Guid deletedRewardTransId = existingToDeleteRewardTrans.RewardTransactionId;


            //Act
            var deletedRewardTrans = RewardTransactionRepository.Delete(existingToDeleteRewardTrans);
            PostgresDbContext.SaveChanges(); //must save to write to db
            var nullRewardTransDueToDeleted = PostgresDbContext.RewardTransactions.Find(deletedRewardTransId);

            //Assert
            deletedRewardTrans.Should().NotBeNull(); //because the to delete is already existed
            deletedRewardTrans.Should().BeAssignableTo<RewardTransaction>();
            deletedRewardTrans.Should().BeEquivalentTo(existingToDeleteRewardTrans); //make sure they're loopback
            nullRewardTransDueToDeleted.Should().BeNull();
        }

        //TODO: THIS TEST FAILED => DELETE NOT RETURN NULL, SO BEFORE DELETE WE MUST MAKE SURE THE TO DELETE REWARD TRANS ALREADY EXIST
        [Fact]
        public void GivenANonExistingRewardTrans_WhenDeleteRewaredTrans_ReturnDeletedRewardTransAndDeletedVerified()
        {
            //Arrange
            RewardTransaction nonExistingToDeleteRewardTrans = new RewardTransaction()
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


            //Act
            var deletedRewardTrans = RewardTransactionRepository.Delete(nonExistingToDeleteRewardTrans);

            //Assert
            nonExistingToDeleteRewardTrans.Should().Match<RewardTransaction>(rt => !DummyRewardTranData.Contains(rt));
            Assert.Null(deletedRewardTrans);
        }


        [Fact]
        public void GivenAnExistingRewardTrans_WhenUpdateRewaredTrans_ReturnUpdatedRewardTransAndUpdatedVerified()
        {
            //Arrange
            RewardTransaction existingToUpdateRewardTrans = DummyRewardTranData.First();
            Guid updatedRewardTransId = existingToUpdateRewardTrans.RewardTransactionId;
            RewardTransaction expectedUpdatedRewardTrans = existingToUpdateRewardTrans;
            expectedUpdatedRewardTrans.PointAfterTransaction -= 5;

            //Act
            var updatedRewardTrans = RewardTransactionRepository.Update(existingToUpdateRewardTrans);
            PostgresDbContext.SaveChanges();
            var actualUpdatedRewardTrans = PostgresDbContext.RewardTransactions.Find(updatedRewardTransId);

            //Assert
            existingToUpdateRewardTrans.Should().NotBeNull();
            updatedRewardTrans.Should().NotBeNull();
            actualUpdatedRewardTrans.Should().NotBeNull();

            existingToUpdateRewardTrans.Should().BeAssignableTo<RewardTransaction>();
            updatedRewardTrans.Should().BeAssignableTo<RewardTransaction>();
            actualUpdatedRewardTrans.Should().BeAssignableTo<RewardTransaction>();

            actualUpdatedRewardTrans.Should().BeEquivalentTo(updatedRewardTrans);
            actualUpdatedRewardTrans.Should().BeEquivalentTo(expectedUpdatedRewardTrans);
            expectedUpdatedRewardTrans.Should().BeEquivalentTo(updatedRewardTrans);
        }
    }
}
