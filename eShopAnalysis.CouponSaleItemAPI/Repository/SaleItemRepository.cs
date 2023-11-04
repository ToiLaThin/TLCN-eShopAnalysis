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

        #region Sync Methods
        public SaleItem Add(SaleItem nSaleItem)
        {
            try
            {
                _dbContext.Add(nSaleItem);
                _dbContext.SaveChanges();
                //throw new Exception("Just testing to see the sale item saved"); //wwork well the data not saved into the db even called SaveChanges since rollback in service
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
        #endregion


        #region Async Methods
        public async Task<SaleItem> AddAsync(SaleItem nSaleItem)
        {
            try
            {
                await _dbContext.AddAsync(nSaleItem);
                await _dbContext.SaveChangesAsync();
                //throw new Exception("Just testing to see the sale item saved"); //wwork well the data not saved into the db even called SaveChanges since rollback in service
                return nSaleItem; //luc nay da duoc set , ke ca guid
            }
            catch { 
                return null; 
            }
        }

        public async Task<SaleItem> DeleteAsync(Guid Id)
        {
            var saleItemnDel = await this.GetAsync(Id);
            if (saleItemnDel == null) {
                throw new Exception("coupon not exist");
                return null;
            }
            try {
                _dbContext.Remove(saleItemnDel);
                await _dbContext.SaveChangesAsync();
                return saleItemnDel;
            }
            catch { 
                return null; 
            }
        }

        public async Task<SaleItem> GetAsync(Guid Id)
        {
            SaleItem item = await _dbContext.SaleItems.FindAsync(Id);
            return item == null ? null : item;
        }

        public async Task<SaleItem> UpdateAsync(SaleItem uSaleItem)
        {
            try
            {
                _dbContext.Update(uSaleItem);
                await _dbContext.SaveChangesAsync();
                return uSaleItem;
            }
            catch { 
                return null; 
            }
        }
        #endregion

        public IQueryable<SaleItem> GetAsQueryable()
        {
            //add asNoTracking in the service layer if needed
            return _dbContext.SaleItems.AsQueryable();
        }
    }
}
