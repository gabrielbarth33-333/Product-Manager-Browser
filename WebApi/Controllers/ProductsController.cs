using Application.Commands.Products;
using Application.UseCases.Products;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly CreateProductUseCase _createProductUseCase;
        private readonly UpdateProductUseCase _updateProductUseCase;
        private readonly DeleteProductUseCase _deleteProductUseCase;
        private readonly GetProductByIdUseCase _getProductByIdUseCase;
        private readonly ListProductsUseCase _listProductsUseCase;

        public ProductsController(
            CreateProductUseCase createProductUseCase,
            UpdateProductUseCase updateProductUseCase,
            DeleteProductUseCase deleteProductUseCase,
            GetProductByIdUseCase getProductByIdUseCase,
            ListProductsUseCase listProductsUseCase)
        {
            _createProductUseCase = createProductUseCase;
            _updateProductUseCase = updateProductUseCase;
            _deleteProductUseCase = deleteProductUseCase;
            _getProductByIdUseCase = getProductByIdUseCase;
            _listProductsUseCase = listProductsUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateProductCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _createProductUseCase.ExecuteAsync(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(CreateProblemDetails("Erro de validação", result.Error));

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Value!.Id },
                result.Value);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateProductCommand command,
            CancellationToken cancellationToken)
        {
            command.Id = id;

            var result = await _updateProductUseCase.ExecuteAsync(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error.Contains("não encontrado"))
                    return NotFound(CreateProblemDetails("Recurso não encontrado", result.Error));

                return BadRequest(CreateProblemDetails("Erro de validação", result.Error));
            }

            return Ok(result.Value);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _deleteProductUseCase.ExecuteAsync(id, cancellationToken);

            if (result.IsFailure)
                return NotFound(CreateProblemDetails("Recurso não encontrado", result.Error));

            return NoContent();
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _getProductByIdUseCase.ExecuteAsync(id, cancellationToken);

            if (result.IsFailure)
                return NotFound(CreateProblemDetails("Recurso não encontrado", result.Error));

            return Ok(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> List(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _listProductsUseCase.ExecuteAsync(pageNumber, pageSize, cancellationToken);
            return Ok(result);
        }

        private static ProblemDetails CreateProblemDetails(string title, string detail)
        {
            return new ProblemDetails
            {
                Title = title,
                Detail = detail,
                Status = StatusCodes.Status400BadRequest
            };
        }
    }
}
