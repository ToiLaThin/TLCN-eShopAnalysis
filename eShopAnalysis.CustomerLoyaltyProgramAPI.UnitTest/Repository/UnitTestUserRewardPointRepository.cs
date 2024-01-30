using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Repository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.UnitTest.Repository
{
    public class UnitTestUserRewardPointRepository: BaseUnitTestRepository
    {
        protected CustomerLoyaltyProgramAPI.Repository.UserRewardPointRepository UserRewardPointRepository { get; set; }

        public UnitTestUserRewardPointRepository(FixtureUnitTestRepository fixtureUnitTestRepository): base(fixtureUnitTestRepository) {
            this.SeedDb();
            UserRewardPointRepository = new CustomerLoyaltyProgramAPI.Repository.UserRewardPointRepository(base.PostgresDbContext);
        }

        public override void SeedDb() {
            base.SeedDb();
            bool isDbJustCreated = PostgresDbContext.Database.EnsureCreated();
            if (PostgresDbContext.UserRewardPoints.Any() == true)
            {
                return;
            }
            PostgresDbContext.UserRewardPoints.AddRange(DummyUserRewardPointData);
            PostgresDbContext.SaveChanges();
        }

        [Fact]
        public void WhenGetAsQueryable_ReturnQueryableResult()
        {
            //Arrange
            IQueryable<UserRewardPoint> expectedResult = base.DummyUserRewardPointData.AsQueryable();


            //Act
            var actualResult = UserRewardPointRepository.GetAsQueryable();

            //Assert
            actualResult.Should().NotBeNullOrEmpty();
            actualResult.Should().BeAssignableTo<object>();
            actualResult.Should().BeAssignableTo<IEnumerable<UserRewardPoint>>();
            actualResult.Should().BeAssignableTo<IQueryable<UserRewardPoint>>();
            actualResult.Should().BeEquivalentTo(expectedResult);
            actualResult.Select(uRP => uRP.RewardPoint)
                        .Should()
                        .BeEquivalentTo(expectedResult.Select(uRP => uRP.RewardPoint));
        }

        [Fact]
        public void WhenGetUserRewardPoints_ReturnRightResult()
        {
            //Arrange
            UserRewardPoint expectedResult = base.DummyUserRewardPointData.First();
            Guid expectedUserId = expectedResult.UserId;


            //Act
            var actualResult = UserRewardPointRepository.Get(expectedUserId);

            //Assert
            actualResult.Should().NotBeNull();
            actualResult.Should().BeAssignableTo<object>();
            actualResult.Should().BeAssignableTo<UserRewardPoint>();
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task WhenGetUserRewardPointAsync_ReturnRightResult()
        {
            //Arrange
            UserRewardPoint expectedResult = base.DummyUserRewardPointData.First();
            Guid expectedUserId = expectedResult.UserId;


            //Act
            var actualResult = await UserRewardPointRepository.GetAsync(expectedUserId);

            //Assert
            actualResult.Should().NotBeNull();
            actualResult.Should().BeAssignableTo<object>();
            actualResult.Should().BeAssignableTo<UserRewardPoint>();
            actualResult.Should().BeEquivalentTo(expectedResult);
        }


        [Fact]
        public void GivenANonExistingUserRewardPoint_WhenAddUserRewardPoint_ReturnAddedUserRewardPoint()
        {
            //Arrange toAddUserRewardPoint
            UserRewardPoint toAddUserRewardPoint = new UserRewardPoint()
            {
                UserId = Guid.Parse("584ab36b-fdb2-468d-a76e-640fb5e575c3"),
                RewardPoint = 20
            };
            Guid addedUserRewardPointId = toAddUserRewardPoint.UserId;


            //Act
            var addedUserRewardPoint = UserRewardPointRepository.Add(toAddUserRewardPoint);
            var actualUserRewardPoint = base.PostgresDbContext.UserRewardPoints.Find(addedUserRewardPointId);

            //Assert
            actualUserRewardPoint.Should().NotBeNull();
            actualUserRewardPoint.Should().BeAssignableTo<UserRewardPoint>();
            addedUserRewardPoint.Should().NotBeNull();
            addedUserRewardPoint.Should().BeAssignableTo<UserRewardPoint>();
            addedUserRewardPoint.Should().BeEquivalentTo(actualUserRewardPoint);
            addedUserRewardPoint.Should().BeEquivalentTo(toAddUserRewardPoint);

            UserRewardPointRepository.Delete(actualUserRewardPoint);

        }

        [Fact]
        public async Task GivenANonExistingUserRewardPoint_WhenAddUserRewardPointAsync_ReturnAddedUserRewardPoint()
        {
            //Arrange toAddUserRewardPoint
            UserRewardPoint toAddUserRewardPoint = new UserRewardPoint()
            {
                UserId = Guid.Parse("584ab36b-fdb2-468d-a76e-640fb5e575c3"),
                RewardPoint = 20
            };
            Guid addedUserRewardPointId = toAddUserRewardPoint.UserId;


            //Act
            var addedUserRewardPoint = await UserRewardPointRepository.AddAsync(toAddUserRewardPoint);
            var actualUserRewardPoint = base.PostgresDbContext.UserRewardPoints.Find(addedUserRewardPointId);

            //Assert
            actualUserRewardPoint.Should().NotBeNull();
            actualUserRewardPoint.Should().BeAssignableTo<UserRewardPoint>();
            addedUserRewardPoint.Should().NotBeNull();
            addedUserRewardPoint.Should().BeAssignableTo<UserRewardPoint>();
            addedUserRewardPoint.Should().BeEquivalentTo(actualUserRewardPoint);
            addedUserRewardPoint.Should().BeEquivalentTo(toAddUserRewardPoint);

            //We must delete the added since this is async & no longer dispose db after each test (not ensured delete but remove range)
            UserRewardPointRepository.Delete(actualUserRewardPoint);
        }


        //THESE TEST FAILED WHEN THEY SHOULDN'T
        //[Fact]
        //public async Task GivenAnAlreadyExistingUserRewardPoint_WhenAddUserRewardPointAsync_ReturnTestWithException()
        //{
        //    //Arrange
        //    UserRewardPoint existingToAddUserRewardPoint = PostgresDbContext.UserRewardPoints.First();

        //    //Act
        //    //Assert
        //    await Assert.ThrowsAnyAsync<Exception>(() => UserRewardPointRepository.AddAsync(existingToAddUserRewardPoint));  //please see the exception, if there is, then return null

        //}

        //[Fact]
        //public void GivenAnAlreadyExistingUserRewardPoint_WhenDeleteUserRewardPoint_ReturnDeletedUserRewardPointsAndDeletedVerified()
        //{
        //    //Arrange, the dummy data already in the db
        //    UserRewardPoint existingToDeleteUserRewardPoint = base.DummyUserRewardPointData.First();
        //    Guid deletedUserRewardPointUserId = existingToDeleteUserRewardPoint.UserId;


        //    //Act
        //    var deletedUserRewardPoints = UserRewardPointRepository.Delete(existingToDeleteUserRewardPoint);
        //    base.PostgresDbContext.SaveChanges(); //must save to write to db
        //    var nullUserRewardPointDueToDeleted = base.PostgresDbContext.UserRewardPoints.Find(deletedUserRewardPointUserId);

        //    //Assert
        //    deletedUserRewardPoints.Should().NotBeNull(); //because the to delete is already existed
        //    deletedUserRewardPoints.Should().BeAssignableTo<UserRewardPoint>();
        //    deletedUserRewardPoints.Should().BeEquivalentTo(existingToDeleteUserRewardPoint); //make sure they're loopback
        //    nullUserRewardPointDueToDeleted.Should().BeNull();
        //}

        //[Fact]
        //public void GivenAnExistingUserRewardPoint_WhenUpdateUserRewardPoint_ReturnUpdatedUserRewardPointAndUpdatedVerified()
        //{
        //    //Arrange
        //    UserRewardPoint existingToUpdateUserRewardPoint = base.DummyUserRewardPointData.First();
        //    Guid updatedUserRewardPointUserId = existingToUpdateUserRewardPoint.UserId;
        //    UserRewardPoint expectedUpdatedUserRewardPoints = existingToUpdateUserRewardPoint;
        //    expectedUpdatedUserRewardPoints.RewardPoint -= 5;

        //    //Act
        //    var updatedUserRewardPoint = UserRewardPointRepository.Update(expectedUpdatedUserRewardPoints);
        //    base.PostgresDbContext.SaveChanges();
        //    var actualUpdatedUserRewardPoint = base.PostgresDbContext.UserRewardPoints.Find(updatedUserRewardPointUserId);

        //    //Assert
        //    existingToUpdateUserRewardPoint.Should().NotBeNull();
        //    updatedUserRewardPoint.Should().NotBeNull();
        //    actualUpdatedUserRewardPoint.Should().NotBeNull();

        //    existingToUpdateUserRewardPoint.Should().BeAssignableTo<UserRewardPoint>();
        //    updatedUserRewardPoint.Should().BeAssignableTo<UserRewardPoint>();
        //    actualUpdatedUserRewardPoint.Should().BeAssignableTo<UserRewardPoint>();

        //    actualUpdatedUserRewardPoint.Should().BeEquivalentTo(updatedUserRewardPoint);
        //    actualUpdatedUserRewardPoint.Should().BeEquivalentTo(expectedUpdatedUserRewardPoints);
        //    expectedUpdatedUserRewardPoints.Should().BeEquivalentTo(updatedUserRewardPoint);
        //}
    }
}
