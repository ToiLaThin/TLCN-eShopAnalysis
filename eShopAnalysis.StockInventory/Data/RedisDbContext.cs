using eShopAnalysis.StockInventory.Utilities;
using Microsoft.Extensions.Options;
using Redis.OM;
using StackExchange.Redis;

namespace eShopAnalysis.StockInventory.Data
{
    using eShopAnalysis.StockInventory.Models;
    using Redis.OM.Searching;

    public class RedisDbContext
    {
        private readonly RedisConnectionProvider _provider;
        private readonly ConnectionMultiplexer _multiplexer;
        private readonly IOptions<RedisSettings> _redisSetting;
        private readonly IServer _server;
        public RedisDbContext(IOptions<RedisSettings> redisSetting)
        {
            _redisSetting = redisSetting;
            ConfigurationOptions redisConfigOptions = new ConfigurationOptions()
            {
                EndPoints = { { redisSetting.Value.Host, redisSetting.Value.Port} }
            };
            _multiplexer = ConnectionMultiplexer.Connect(redisConfigOptions);
            _server = _multiplexer.GetServer(redisSetting.Value.Host, redisSetting.Value.Port);
            _provider = new RedisConnectionProvider(_multiplexer);
            bool IndexCreated= _provider.Connection.CreateIndex(typeof(StockInventory)); //will create index if not
            var result = IndexCreated;
        }

        public IRedisCollection<StockInventory> StockInventoryCollection { get {
                var result = _provider.RedisCollection<StockInventory>();
                return result;
            }
        }

        public async void BackUp() //ensure high availability, this can be called every request or at a specific interval
        {
            await _server.SaveAsync(SaveType.BackgroundSave); //TODO check the effectiveness of this

        }


    }
}
