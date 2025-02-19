using FoodMenuApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FoodMenuAPI.Data;
using Microsoft.EntityFrameworkCore;
using FoodMenuApi.DTOS;
using System.ComponentModel.DataAnnotations;

namespace FoodMenuApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly MenuContext _context;
        private readonly ILogger<MenuController> _logger;

        public MenuController(MenuContext context, ILogger<MenuController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DishDTO>>> GetDishes()
        {
            try
            {
                var dishes = await _context.Dishes
                    .Include(di => di.DishIngredients)
                    .ThenInclude(i => i.Ingredient)
                    .ToListAsync();

                if (!dishes.Any())
                {
                    return NotFound("No dishes found in the database.");
                }

                var dishDTOs = dishes.Select(MapToDTO).ToList();
                return Ok(dishDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching dishes");
                return StatusCode(500, "An error occurred while retrieving dishes");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DishDTO>> GetDish(int id)
        {
            try
            {
                var dish = await _context.Dishes
                    .Include(di => di.DishIngredients)
                    .ThenInclude(i => i.Ingredient)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (dish == null)
                {
                    return NotFound($"Dish with ID {id} not found.");
                }

                return Ok(MapToDTO(dish));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching dish with id {DishId}", id);
                return StatusCode(500, "An error occurred while retrieving the dish");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DishDTO>> CreateDish([FromBody] CreateDishDTO createDishDTO)
        {
            try
            {
                if (createDishDTO == null)
                    return BadRequest("Dish data is required");

                if (string.IsNullOrEmpty(createDishDTO.Name))
                    return BadRequest("Dish name is required");

                if (createDishDTO.Price <= 0)
                    return BadRequest("Price must be greater than zero");

                var newDish = new Dish
                {
                    Name = createDishDTO.Name,
                    Description = createDishDTO.Description,
                    Price = (double)createDishDTO.Price,
                    ImageUrl = createDishDTO.ImageUrl,
                    DishIngredients = new List<DishIngredient>()
                };

                foreach (var ingredientId in createDishDTO.IngredientIds)
                {
                    var ingredient = await _context.Ingredients.FindAsync(ingredientId);
                    if (ingredient == null)
                    {
                        return BadRequest($"Ingredient with ID {ingredientId} not found.");
                    }

                    newDish.DishIngredients.Add(new DishIngredient
                    {
                        Dish = newDish,
                        IngredientId = ingredientId
                    });
                }

                _context.Dishes.Add(newDish);
                await _context.SaveChangesAsync();

                var dishDTO = MapToDTO(newDish);
                return CreatedAtAction(nameof(GetDish), new { id = newDish.Id }, dishDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating new dish");
                return StatusCode(500, "An error occurred while creating the dish");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DishDTO>> UpdateDish(int id, [FromBody] UpdateDishDTO updateDishDTO)
        {
            try
            {
                if (updateDishDTO == null)
                    return BadRequest("Dish data is required");

                var dish = await _context.Dishes
                    .Include(d => d.DishIngredients)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (dish == null)
                    return NotFound($"Dish with ID {id} not found.");

                // Update basic properties
                dish.Name = updateDishDTO.Name ?? dish.Name;
                dish.Description = updateDishDTO.Description ?? dish.Description;
                dish.Price = updateDishDTO.Price != null ? (double)updateDishDTO.Price : dish.Price;
                dish.ImageUrl = updateDishDTO.ImageUrl ?? dish.ImageUrl;

                // Update ingredients if provided
                if (updateDishDTO.IngredientIds != null && updateDishDTO.IngredientIds.Any())
                {
                    // Remove existing ingredients
                    dish.DishIngredients.Clear();

                    // Add new ingredients
                    foreach (var ingredientId in updateDishDTO.IngredientIds)
                    {
                        var ingredient = await _context.Ingredients.FindAsync(ingredientId);
                        if (ingredient == null)
                        {
                            return BadRequest($"Ingredient with ID {ingredientId} not found.");
                        }

                        dish.DishIngredients.Add(new DishIngredient
                        {
                            DishId = dish.Id,
                            IngredientId = ingredientId
                        });
                    }
                }

                await _context.SaveChangesAsync();
                return Ok(MapToDTO(dish));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating dish with id {DishId}", id);
                return StatusCode(500, "An error occurred while updating the dish");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDish(int id)
        {
            try
            {
                var dish = await _context.Dishes.FindAsync(id);
                if (dish == null)
                {
                    return NotFound($"Dish with ID {id} not found.");
                }

                _context.Dishes.Remove(dish);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting dish with id {DishId}", id);
                return StatusCode(500, "An error occurred while deleting the dish");
            }
        }

        private static DishDTO MapToDTO(Dish dish)
        {
            if (dish == null) return null;

            return new DishDTO
            {
                Id = dish.Id,
                Name = dish.Name,
                Description = dish.Description,
                Price = (decimal)dish.Price,
                ImageUrl = dish.ImageUrl,
                Ingredients = dish.DishIngredients?
                    .Select(di => new IngredientDTO
                    {
                        Id = di.Ingredient.Id,
                        Name = di.Ingredient.Name
                    }).ToList() ?? new List<IngredientDTO>()
            };
        }
    }
}