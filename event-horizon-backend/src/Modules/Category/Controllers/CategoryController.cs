using AutoMapper;
using event_horizon_backend.Core.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
}