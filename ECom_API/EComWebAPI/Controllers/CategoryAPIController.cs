using AutoMapper;
using EComWebAPI.Data;
using EComWebAPI.Models;
using EComWebAPI.Models.Dto;
using EComWebAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EComWebAPI.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/CategoryAPI")]
    [ApiController]
    public class CategoryAPIController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IMapper _mapper;

        public CategoryAPIController(ICategoryRepository categoryRepo, IMapper mapper)
        {
            _categoryRepo = categoryRepo;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            IEnumerable<Category> categoryList = await _categoryRepo.GetAllAsync();
            return Ok(_mapper.Map<List<CategoryDTO>>(categoryList));
        }

        [HttpGet("{id:int}",Name = "GetCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var category = await _categoryRepo.GetAsync(u => u.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CategoryDTO>(category));

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CategoryDTO>> CreateCategory([FromBody]CategoryCreateDTO createDTO)
        {
            //if(!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if (await _categoryRepo.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomerError", "Category already Exists!");
                return BadRequest(ModelState);
            }
            if (await _categoryRepo.GetAsync(u => u.DisplayOrder.ToString() == createDTO.DisplayOrder.ToString()) != null)
            {
                ModelState.AddModelError("DuplicateError", "Display Order already Exists!");
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

            Category model = _mapper.Map<Category>(createDTO);

            //Category model = new()
            //{
            //    //Id = categoryDTO.Id,
            //    Name = createDTO.Name,
            //    DisplayOrder = createDTO.DisplayOrder
            //};

            await _categoryRepo.CreateAsync(model);

            return CreatedAtRoute("GetCategory", new { id = model.Id }, model);

        }


        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:int}", Name = "DeleteCategory")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var category = await _categoryRepo.GetAsync(u => u.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            await _categoryRepo.RemoveAsync(category);

            return NoContent();
        }

        [HttpPut("{id:int}",Name = "UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCategory (int id, [FromBody]CategoryUpdateDTO updateDTO)
        {
            if(updateDTO == null || id != updateDTO.Id)
            {
                return BadRequest();
            }

            Category model = _mapper.Map<Category>(updateDTO);

            //Category model = new()
            //{   
            //    Id = updateDTO.Id,
            //    Name = updateDTO.Name,
            //    DisplayOrder = updateDTO.DisplayOrder
            //};

            await _categoryRepo.UpdateAsync(model);
            return NoContent();
        }

        [HttpPatch("{id:int}",Name ="UpdatePartialCategory")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialCategory(int id, JsonPatchDocument<CategoryUpdateDTO> patchDTO)
        {
            if(patchDTO == null || id ==0)
            {
                return BadRequest();
            }
            var category = await _categoryRepo.GetAsync(u => u.Id == id, tracked: false);

            CategoryUpdateDTO categoryDTO = _mapper.Map<CategoryUpdateDTO>(category);

            //CategoryUpdateDTO categoryDTO = new()
            //{
            //    Id = category.Id,
            //    Name = category.Name,
            //    DisplayOrder = category.DisplayOrder
            //};

            if(category == null)
            {
                return BadRequest();
            }

            patchDTO.ApplyTo(categoryDTO, ModelState);

            Category model = _mapper.Map<Category>(categoryDTO);

            //Category model = new Category()
            //{
            //    Id = categoryDTO.Id,
            //    Name = categoryDTO.Name,
            //    DisplayOrder = categoryDTO.DisplayOrder
            //};

            await _categoryRepo.UpdateAsync(model);

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return NoContent();

        }

    }
}
