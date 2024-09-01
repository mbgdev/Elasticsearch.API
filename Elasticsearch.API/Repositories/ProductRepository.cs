using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elasticsearch.API.DTOs;
using Elasticsearch.API.Model;
using System.Collections.Immutable;

namespace Elasticsearch.API.Repositories
{
    public class ProductRepository
    {
        private readonly ElasticsearchClient _client;

        private const string indexName = "products";

        public ProductRepository(ElasticsearchClient client)
        {
            _client = client;
        }


        public async Task<Product?> SaveAsync(Product newProduct)
        {

            newProduct.Created = DateTime.Now;

            var response = await _client.IndexAsync(newProduct, x => x.Index(indexName).Id(Guid.NewGuid().ToString()));

            if (!response.IsSuccess()) return null;

            newProduct.Id = response.Id;

            return newProduct;





        }

        public async Task<ImmutableList<Product>> GetAllAsync()
        {
            // MatchAll sorgusu oluşturuluyor
            var result = await _client.SearchAsync<Product>(s => s.Index(indexName));

            // Döngüyle sonuçlar işleniyor
            foreach (var hit in result.Hits)
            {
                hit.Source.Id = hit.Id;
            }

            return result.Documents.ToImmutableList();
        }



        public async Task<Product> GetByIdAsync(string id)
        {
            var response=await _client.GetAsync<Product>(id,x=>x.Index(indexName));

            if (!response.IsSuccess()) return null;

            response.Source.Id=response.Id;

            return response.Source;

        }



        public async Task<bool> UpdateAync(ProductUpdateDto updateProduct)
        {
            var response = await _client.UpdateAsync<Product, ProductUpdateDto>(indexName, updateProduct.Id, x => x.Doc(updateProduct));

            return response.IsSuccess();

        }

        public async Task<bool> DeleteAsync(string id)
        {
            var response=await _client.DeleteAsync<Product>(id, x=>x.Index(indexName));
            return response.IsSuccess();
        }



        /// <summary>
        /// Hata yönetimi için bu method ele alınmıştır.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<DeleteResponse> HDeleteAsync(string id)
        {

            var response = await _client.DeleteAsync<Product>(id, x => x.Index(indexName));
            return response;
        }





    }
}
