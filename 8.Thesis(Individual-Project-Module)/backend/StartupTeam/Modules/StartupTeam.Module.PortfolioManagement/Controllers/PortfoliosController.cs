using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StartupTeam.Module.PortfolioManagement.Dtos;
using StartupTeam.Module.PortfolioManagement.Services;
using StartupTeam.Module.UserManagement.Helpers;
using StartupTeam.Module.UserManagement.Models.Constants;
using StartupTeam.Shared.Models;

namespace StartupTeam.Module.PortfolioManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortfoliosController : ControllerBase
    {
        private readonly IPortfolioService _portfolioService;

        public PortfoliosController(IPortfolioService portfolioService)
        {
            _portfolioService = portfolioService;
        }

        [HttpGet]
        [Authorize(Roles = RoleConstants.SkilledIndividual)]
        public async Task<IActionResult> GetPortfolioItems()
        {
            var userId = ClaimsHelper.GetUserId(User);

            if (userId == null)
            {
                return BadRequest(new ApiResponse<object>()
                {
                    Success = false,
                    Message = "User ID is missing or invalid."
                });
            }

            var portfolioItems =
                await _portfolioService.GetPortfolioItemsByUserIdAsync(userId.Value);

            return Ok(new ApiResponse<object>
            {
                Data = portfolioItems
            });
        }

        [HttpGet("individual-portfolio/{userId}")]
        [Authorize(Roles = RoleConstants.SkilledIndividual + "," + RoleConstants.StartupFounder)]
        public async Task<IActionResult> GetPortfolioItemsByUserId(Guid userId)
        {
            var portfolioItems =
                await _portfolioService.GetPortfolioItemsByUserIdAsync(userId);

            if (!portfolioItems.Any())
            {
                return NotFound(new ApiResponse<object>()
                {
                    Success = false,
                    Message = "No portfolio items found for this user."
                });
            }

            return Ok(new ApiResponse<object>()
            {
                Data = portfolioItems
            });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = RoleConstants.SkilledIndividual)]
        public async Task<IActionResult> GetPortfolioItemForm(Guid id)
        {
            var portfolioItem = await _portfolioService.GetPortfolioItemFormByIdAsync(id);

            if (portfolioItem == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Portfolio item not found."
                });
            }

            return Ok(new ApiResponse<PortfolioItemFormDto>
            {
                Data = portfolioItem
            });
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.SkilledIndividual)]
        public async Task<IActionResult> CreatePortfolioItem(
            PortfolioItemFormDto portfolioItemFormDto)
        {
            var userId = ClaimsHelper.GetUserId(User);

            if (userId == null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User ID is missing or invalid."
                });
            }

            var result =
                await _portfolioService.CreatePortfolioItemAsync(portfolioItemFormDto, userId.Value);

            if (!result)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "An error occurred while creating the portfolio item."
                    });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Portfolio item created successfully."
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = RoleConstants.SkilledIndividual)]
        public async Task<IActionResult> UpdatePortfolioItem(
            Guid id, PortfolioItemFormDto portfolioItemFormDto)
        {
            if (id != portfolioItemFormDto.Id)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "ID mismatch."
                });
            }

            var userId = ClaimsHelper.GetUserId(User);

            if (userId == null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User ID is missing or invalid."
                });
            }

            if (userId != portfolioItemFormDto.UserId)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "You are not authorized to update this portfolio item."
                });
            }

            var result = await _portfolioService.UpdatePortfolioItemAsync(portfolioItemFormDto);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Portfolio item not found."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Portfolio item updated successfully."
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = RoleConstants.SkilledIndividual)]
        public async Task<IActionResult> DeletePortfolioItem(Guid id)
        {
            var result = await _portfolioService.DeletePortfolioItemAsync(id);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Portfolio item not found."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Portfolio item deleted successfully."
            });
        }

        [HttpGet("details/{id}")]
        [Authorize(Roles = RoleConstants.SkilledIndividual + "," + RoleConstants.StartupFounder)]
        public async Task<IActionResult> GetPortfolioItemDetail(Guid id)
        {
            var portfolioItem = await _portfolioService.GetPortfolioItemByIdAsync(id);

            if (portfolioItem == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Portfolio item not found."
                });
            }

            return Ok(new ApiResponse<PortfolioItemDetailDto>
            {
                Data = portfolioItem
            });
        }
    }
}
