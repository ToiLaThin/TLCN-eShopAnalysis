using eShopAnalysis.CouponSaleItemAPI.Data;
using eShopAnalysis.CouponSaleItemAPI.Models;

namespace eShopAnalysis.CouponSaleItemAPI.Repository
{
    public class SaleItemRepository : ISaleItemRepository
    {
        private readonly PostgresDbContext _dbContext;
        public SaleItemRepository(PostgresDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public SaleItem Add(SaleItem nSaleItem)
        {
            try
            {
                _dbContext.Add(nSaleItem);
                _dbContext.SaveChanges();
                //throw new Exception("Just testing to see the sale item saved"); wwork well the data not saved into the db even called SaveChanges since rollback in service
                return nSaleItem; //luc nay da duoc set , ke ca guid
            }
            catch { return null; }
        }

        public SaleItem Delete(Guid Id)
        {
            var saleItemnDel = this.Get(Id);
            if (saleItemnDel == null)
            {
                throw new Exception("coupon not exist");
                return null;
            }
            try
            {
                _dbContext.Remove(saleItemnDel);
                _dbContext.SaveChanges();
                return saleItemnDel;
            }
            catch { return null; }
        }

        public SaleItem Get(Guid Id)
        {
            SaleItem item = _dbContext.SaleItems.Find(Id);
            return item == null ? null : item;
        }

        public IEnumerable<SaleItem> GetAll()
        {
            return _dbContext.SaleItems.ToList();
        }

        public IQueryable<SaleItem> GetAllAsQueryable()
        {
            return _dbContext.SaleItems.AsQueryable();
        }

        public SaleItem Update(SaleItem uSaleItem)
        {
            try
            {
                _dbContext.Update(uSaleItem);
                _dbContext.SaveChanges();
                return uSaleItem;
            }
            catch { return null; }
        }
    }
}
