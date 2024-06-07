CREATE DATABASE eShopAnalysisDW
USE eShopAnalysisDW


CREATE TABLE SaleItemDim (
	SaleItemId uniqueidentifier,
	DiscountType int,
	DiscountValue float,
	MinimumItemQuantityToApply int,
	Duration int,
	RewardPointRequire int
)

CREATE TABLE DateDim (
	DateKey varchar(14),
	Date date,
	Year int,
	Quarter int,
	Month int,
	Day int,
	Weekday int,
	WeekdayName varchar(20),
)

CREATE TABLE CartItemFact (
	CartId uniqueidentifier,
	ProductModelId uniqueidentifier,
	ProductId uniqueidentifier,
	BusinessKey uniqueidentifier,
	SaleItemKey uniqueidentifier,
	IsOnSale bit,
	SaleValue float,
	Quantity int,
	UnitPrice float,
	FinalPrice float,
	UnitAfterSalePrice float,
	FinalAfterSalePrice float,
	DateStockConfirmedKey varchar(14),
)

CREATE TABLE ProductModelDim (
	ProductModelId uniqueidentifier,
	ProductKey uniqueidentifier,
	BusinessKey uniqueidentifier,
	CublicTypeKey int,
	ProductModelName nvarchar(500)
)

CREATE TABLE CublicTypeDim (
	CublicTypeId int,
	CublicTypeName nvarchar(250)
)

CREATE TABLE ProductsDim (
	ProductId uniqueidentifier,
	ProductName nvarchar(500),
	BrandKey int,
	UsageInstructionTypeKey int,
	SubCatalogKey uniqueidentifier,
	PreserveInstructionTypeKey int,
	HaveModels bit,
	HavePricePerCublic bit,
	BusinessKey uniqueidentifier,
	Revision int
)

CREATE TABLE PreserveInstructionTypeDim (
	PreserveInstructionTypeId int,
	PreserveInstructionTypeName nvarchar(500)
)

CREATE TABLE UsageInstructionTypeDim (
	UsageInstructionTypeId int,
	UsageInstructionTypeName nvarchar(1000)
)

CREATE TABLE BrandDim (
	BrandId int,
	BrandName nvarchar(250)
)

CREATE TABLE SubCatalogDim (
	SubCatalogId uniqueidentifier,
	SubCatalogName nvarchar(250),
	CatalogKey uniqueidentifier
)

CREATE TABLE CatalogDim (
	CatalogId uniqueidentifier,
	CatalogName nvarchar(250)
)

CREATE TABLE AddressDim (
	AddressId int,
	DistrictOrLocality nvarchar(250)
)

CREATE TABLE PaymentMethodDim (
	PaymentMethodId int,
	MethodName nvarchar(250)
)

CREATE TABLE CouponDim (
	CouponId uniqueidentifier,
	DiscountType int,
	DiscountValue float,
	MinimumOrderValueToApply float,
	RewardPointRequire int,
	CouponStatus int
)

CREATE TABLE OrderStatusDim (
	OrderStatusId int,
	OrderStatusName nvarchar(250)
)

CREATE TABLE AbandonStepDim (
	AbandonStepId int,
	StepName nvarchar(500)
)

CREATE TABLE ProcessDim (
	ProcessId int,
	StepName nvarchar(500)
)

CREATE TABLE UserDim (
	UserId uniqueidentifier,
	EmailConfirmed bit,
	PhoneNumber nvarchar(15)
)

CREATE TABLE DiscountTypeDim (
	DiscountTypeId int,
	DiscountTypeName varchar(200)
)

--CREATE TABLE OrderFact (
--	--Dimension keys
--	OrderId uniqueidentifier,
--	AddressKey int,
--	PaymentMethodKey int,
--	OrderStatusKey int,
--	UserKey uniqueidentifier,
--	CouponKey uniqueidentifier,
--	AbandonStepKey int,
--	ProcessKey int,
--	DateCreated datetime,
--	DateConfirmed datetime,
--	DateRefunded datetime,
--	DateCompleted datetime,
--	--Facts
--	HaveCouponApplied bit,
--	HaveAnySaleItem bit,
--	CartStatus int,
--	CouponDiscountType int,
--	CouponDiscountValue float,
--	TotalSaleDiscountAmount float,
--	CouponDiscountAmount float,
--	TotalPriceOriginal float,
--	TotalPriceFinal float,
--	TimeToCompleteOrCanceled float, --(hours)
--	TimeLagToApprove float
--)

CREATE TABLE CartOrderFact (
	--Dimension keys
	CartOrderId uniqueidentifier,
	AddressKey int,
	PaymentMethodKey int,
	OrderStatusKey int,
	AbandonStepKey int,
	DiscountTypeKey	int,
	ProcessKey int,
	DateCreatedDraftKey varchar(20),
	DateStockConfirmedKey varchar(20),

	--Facts
	HaveCouponApplied bit,
	HaveAnySaleItem bit,
	CouponDiscountAmount float,
	CouponDiscountValue float,
	TotalSaleDiscountAmount float,
	TotalPriceOriginal float,
	TotalPriceAfterSale float,
	TotalPriceAfterCouponApplied float,
	TotalPriceFinal float,
	TimeLagToApprove float,
)