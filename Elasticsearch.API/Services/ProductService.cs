using Elasticsearch.API.DTOs;
using Elasticsearch.API.Model;
using Elasticsearch.API.Repositories;
using Nest;
using System.Collections.Immutable;
using System.Linq;
using System.Net;

namespace Elasticsearch.API.Services;

public class ProductService
{
    private readonly ProductRepository _repository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(ProductRepository repository, ILogger<ProductService> logger)
    {
        _repository = repository;
        _logger = logger;
    }


    public async Task<ResponseDto<ProductDto>> SaveAsync(ProductCreateDto request)
    {
        var responseProduct = await _repository.SaveAsync(request.CreateProduct());

        if (responseProduct == null)
        { return ResponseDto<ProductDto>.Fail(new List<string> { "Kayıt sırasında hata meydana geldi" }, HttpStatusCode.InternalServerError); }

        return ResponseDto<ProductDto>.Success(responseProduct.CreateDto(),
            HttpStatusCode.Created);

    }



    public async Task<ResponseDto<List<ProductDto>>> GetAllAsync()
    {
        var products = await _repository.GetAllAsync();

        var productListDto = new List<ProductDto>();

        foreach (var item in products)
        {

            if (item.Feature is null)
            {
                productListDto.Add(new ProductDto(item.Id, item.Name, item.Price, item.Stock, null));
            }
            else
            {
                productListDto.Add(new ProductDto(item.Id, item.Name, item.Price, item.Stock, new ProductFeatureDto(item.Feature.Width, item.Feature.Height, item.Feature.Color.ToString())));
            }



        }


        return ResponseDto<List<ProductDto>>.Success(productListDto, HttpStatusCode
            .OK);
    }


    public async Task<ResponseDto<ProductDto>> GetByIdAsync(string id)
    {

        var hasProduct = await _repository.GetByIdAsync(id);

        if (hasProduct == null) return ResponseDto<ProductDto>.Fail("Veri bulunamadı.", HttpStatusCode.InternalServerError);

        return ResponseDto<ProductDto>.Success(hasProduct.CreateDto(), HttpStatusCode.OK);

    }



    public async Task<ResponseDto<bool>> UpdateAsync(ProductUpdateDto productUpdateDto)
    {
        var isSuccess=await _repository.UpdateAsync(productUpdateDto);

        if (!isSuccess) return ResponseDto<bool>.Fail("Veri güncellenemdi.", HttpStatusCode.InternalServerError);

        return ResponseDto<bool>.Success(isSuccess, HttpStatusCode.NoContent);

    }

    public async Task<ResponseDto<bool>> DeleteAsync(string id)
    {
        var isSuccess = await _repository.DeleteAsync(id);

        if (!isSuccess) return ResponseDto<bool>.Fail("Veri silinemedi.", HttpStatusCode.InternalServerError);

        return ResponseDto<bool>.Success(isSuccess, HttpStatusCode.NoContent);

    }



    public async Task<ResponseDto<bool>> HDeleteAsync(string id)
    {
        var deleteResponse = await _repository.HDeleteAsync(id);

        
        if (!deleteResponse.IsValid && deleteResponse.Result == Result.NotFound)
        {
            return ResponseDto<bool>.Fail(new List<string> { "Silmeye çalıştığınız ürün bulunamamıştır." }, System.Net.HttpStatusCode.NotFound);

        }


        if (!deleteResponse.IsValid)
        {
           

            _logger.LogError(deleteResponse.OriginalException, deleteResponse.ServerError.Error.ToString());


            return ResponseDto<bool>.Fail(new List<string> { "silme esnasında bir hata meydana geldi." },HttpStatusCode.NotFound);

        }



        return ResponseDto<bool>.Success(true, HttpStatusCode.NoContent);
    }




}
