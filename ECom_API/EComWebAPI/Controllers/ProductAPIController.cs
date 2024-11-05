using AutoMapper;
using EComWebAPI.Data;
using EComWebAPI.Models;
using EComWebAPI.Models.Dto;
using EComWebAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EComWebAPI.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/ProductAPI")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IMapper _mapper;

        public ProductAPIController(IProductRepository productRepo, ICategoryRepository categoryRepo, IMapper mapper)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            IEnumerable<Product> productList = await _productRepo.GetAllAsync();
            return Ok(_mapper.Map<List<ProductDTO>>(productList));
        }

        [HttpGet("{id:int}",Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var product = await _productRepo.GetAsync(u => u.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ProductDTO>(product));

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDTO>> CreateProduct([FromBody] ProductCreateDTO createDTO)
        {
            //if(!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if (await _productRepo.GetAsync(u => u.Title.ToLower() == createDTO.Title.ToLower()) != null)
            {
                ModelState.AddModelError("CustomerError", "Product already Exists!");
                return BadRequest(ModelState);
            }
            if(await _categoryRepo.GetAsync(u => u.Id == createDTO.CategoryId) == null)
            {
                ModelState.AddModelError("InvalidError", "Category ID is Invalid!");
                return BadRequest(ModelState);
            }
            if (createDTO == null)
            {
                return BadRequest(createDTO);
            }
            //if(categoryDTO.Id > 0)
            //{
            //    return StatusCode(StatusCodes.Status500InternalServerError);
            //}

            Product product = _mapper.Map<Product>(createDTO);

            //Category model = new()
            //{
            //    //Id = categoryDTO.Id,
            //    Name = createDTO.Name,
            //    DisplayOrder = createDTO.DisplayOrder
            //};

            await _productRepo.CreateAsync(product);

            return CreatedAtRoute("GetProduct", new { id = product.Id }, product);

        }


        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:int}", Name = "DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var product = await _productRepo.GetAsync(u => u.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            await _productRepo.RemoveAsync(product);

            return NoContent();
        }

        [HttpPut("{id:int}",Name = "UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDTO updateDTO)
        {
            if(updateDTO == null || id != updateDTO.Id)
            {
                return BadRequest();
            }
            if (await _categoryRepo.GetAsync(u => u.Id == updateDTO.CategoryId) == null)
            {
                ModelState.AddModelError("InvalidError", "Category ID is Invalid!");
                return BadRequest(ModelState);
            }

            Product product = _mapper.Map<Product>(updateDTO);

            //Category model = new()
            //{   
            //    Id = updateDTO.Id,
            //    Name = updateDTO.Name,
            //    DisplayOrder = updateDTO.DisplayOrder
            //};

            await _productRepo.UpdateAsync(product);
            return NoContent();
        }

        [HttpPatch("{id:int}",Name = "UpdatePartialProduct")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialProduct(int id, JsonPatchDocument<ProductUpdateDTO> patchDTO)
        {
            if(patchDTO == null || id ==0)
            {
                return BadRequest();
            }
            var product = await _productRepo.GetAsync(u => u.Id == id, tracked: false);

            ProductUpdateDTO productDTO = _mapper.Map<ProductUpdateDTO>(product);

            //CategoryUpdateDTO categoryDTO = new()
            //{
            //    Id = category.Id,
            //    Name = category.Name,
            //    DisplayOrder = category.DisplayOrder
            //};

            if(product == null)
            {
                return BadRequest();
            }

            patchDTO.ApplyTo(productDTO, ModelState);

            Product model = _mapper.Map<Product>(productDTO);

            //Category model = new Category()
            //{
            //    Id = categoryDTO.Id,
            //    Name = categoryDTO.Name,
            //    DisplayOrder = categoryDTO.DisplayOrder
            //};
            
            await _productRepo.UpdateAsync(model);

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return NoContent();

        }

    }
}
