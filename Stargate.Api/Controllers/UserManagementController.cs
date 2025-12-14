using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stargate.Application.Interfaces;
using Stargate.Domain.Dtos;

namespace Stargate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public partial class UserManagementController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(IUserService userService, ILogger<UserManagementController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
        {
            try
            {
                var response = await _userService.GetAllUsersAsync(cancellationToken);
                return this.GetResponse(response);
            }
            catch (Exception ex)
            {
                LogErrorRetrievingUsers(ex);
                return this.GetResponse(new UserListResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving users",
                    ResponseCode = 500
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _userService.GetUserByIdAsync(id, cancellationToken);
                return this.GetResponse(response);
            }
            catch (Exception ex)
            {
                LogErrorRetrievingUser(ex, id);
                return this.GetResponse(new UserResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving the user",
                    ResponseCode = 500
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _userService.CreateUserAsync(request, cancellationToken);
                return this.GetResponse(response);
            }
            catch (Exception ex)
            {
                LogErrorCreatingUser(ex, request.Username);
                return this.GetResponse(new UserResponse
                {
                    Success = false,
                    Message = "An error occurred while creating the user",
                    ResponseCode = 500
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _userService.UpdateUserAsync(id, request, cancellationToken);
                return this.GetResponse(response);
            }
            catch (Exception ex)
            {
                LogErrorUpdatingUser(ex, id);
                return this.GetResponse(new UserResponse
                {
                    Success = false,
                    Message = "An error occurred while updating the user",
                    ResponseCode = 500
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _userService.DeleteUserAsync(id, cancellationToken);
                return this.GetResponse(response);
            }
            catch (Exception ex)
            {
                LogErrorDeletingUser(ex, id);
                return this.GetResponse(new BaseResponse
                {
                    Success = false,
                    Message = "An error occurred while deleting the user",
                    ResponseCode = 500
                });
            }
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles(CancellationToken cancellationToken)
        {
            try
            {
                var response = await _userService.GetAllRolesAsync(cancellationToken);
                return this.GetResponse(response);
            }
            catch (Exception ex)
            {
                LogErrorRetrievingRoles(ex);
                return this.GetResponse(new RoleListResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving roles",
                    ResponseCode = 500
                });
            }
        }
    }
}
