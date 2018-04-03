using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Microsoft.Extensions.Logging;
using MosListAPI.Models;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace MosListAPI.DataAccess{
    public class AdRepository : IRepository<Ad>{
        #region " Members "
        private IElasticSearchClient _client;
        private ILogger _logger;
        #endregion

        #region " Constructors "
        public AdRepository(IElasticSearchClient elasticSearchClient, ILogger<AdRepository> logger){
            _client = elasticSearchClient;
            _logger = logger;
        }
        #endregion 
        #region " Methods "
        public async Task<Ad> CreateAsync(Ad item){
            await _client.PutItemAsync(item);
            return item;
        }

        public async Task<IEnumerable<Ad>> GetAllAsync(){
            // TODO: Fill this in
            return await Task.FromResult<IEnumerable<Ad>>(null);
        }
        public async Task<Ad> GetAsync(string id){
            var ad = await _client.GetItemAsync<Ad>(id);
            return ad;
        }
       #endregion
    }
}