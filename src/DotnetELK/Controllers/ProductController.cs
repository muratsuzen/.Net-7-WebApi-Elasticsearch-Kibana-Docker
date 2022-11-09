using DotnetELK.Models;
using Microsoft.AspNetCore.Mvc;
using Nest;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly IElasticClient _elasticClient;
    private readonly ILogger<ProductController> _logger;

    public ProductController(
        IElasticClient elasticClient,
        ILogger<ProductController> logger
    )
    {
        _elasticClient = elasticClient;
        _logger = logger;
    }

    [HttpGet(Name = "GetProducts")]
    public async Task<IActionResult> Get(string keyword)
    {
        var result = await _elasticClient.SearchAsync<Product>(s => s.Query(
            q => q.QueryString(
                d => d.Query('*' + keyword + '*')
            )
        ).Size(1000)
        );

        return Ok(result.Documents.ToList());
    }

    [HttpPost(Name = "AddProduct")]
    public async Task<IActionResult> Post(Product product)
    {
        await _elasticClient.IndexDocumentAsync(product);

        return Ok();
    }
}