using AutoMapper;
using event_horizon_backend.Common.Extensions;
using event_horizon_backend.Core.Context;
using event_horizon_backend.Core.Models;
using event_horizon_backend.Modules.Category.DTO.AdminDTO;
using event_horizon_backend.Modules.Category.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static event_horizon_backend.Modules.Category.DTO.AdminDTO.CategoryAdminDTO;

namespace event_horizon_backend.Modules.Category.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CategoryController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<PagedResponse<CategoryModel>>> GetCategories(
        [FromQuery] PaginationParameters parameters)
    {
        IQueryable<CategoryModel> categories = _context.Categories.AsQueryable();

        PagedResponse<CategoryModel> pagedResult = await categories.ToPagedListAsync(
            parameters.PageNumber,
            parameters.PageSize
        );

        return Ok(pagedResult);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryModel>> GetCategory(Guid id)
    {
        var categoryModel = await _context.Categories.FindAsync(id);
        return categoryModel == null ? NotFound() : categoryModel;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<CategoryModel>> CreateCategory(CategoryAdminCreateDTO categoryDto)
    {
        CategoryModel categoryModel = _mapper.Map<CategoryModel>(categoryDto);
        _context.Categories.Add(categoryModel);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCategory), new { id = categoryModel.Id }, categoryModel);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(Guid id, CategoryModel categoryModel)
    {
        if (!id.Equals(categoryModel.Id))
        {
            return BadRequest();
        }

        _context.Entry(categoryModel).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CategoryExists(id))
            {
                return NotFound();
            }

            throw;
        }

        return NoContent();
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var categoryModel = await _context.Categories.FindAsync(id);

        if (categoryModel == null)
        {
            return NotFound();
        }

        categoryModel.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CategoryExists(Guid id)
    {
        return _context.Categories.Any(e => e.Id == id);
    }
}