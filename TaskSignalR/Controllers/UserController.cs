using Microsoft.AspNetCore.Mvc;
using System;
using TaskSignalR.BLL.DTO;
using TaskSignalR.BLL.IService;
using TaskSignalR.Contracts;
using TaskSignalR.DAL.Models;
using TaskSignalR.Requests;

namespace TaskSignalR.Controllers
{
    /// <summary>
    /// Controller for handling user-related operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userService">The user service dependency.</param>
        /// <param name="logger">The logger dependency.</param>
        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Get All Users.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/User/GetAll
        /// 
        /// </remarks>
        /// <returns>A list of existed users.</returns>
        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserResponse>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<UserResponse>>> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsers();
                var response = users.Select(u => new UserResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                }).ToList();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get a User by ID.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/User/GetById?id=1
        /// 
        /// </remarks>
        /// <param name="id">The ID of the user.</param>
        /// <returns>The user with the specified ID.</returns>
        [HttpGet("GetById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserResponse>> GetUserById(int id)
        {
            try
            {
                var user = await _userService.GetUserById(id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found");
                }

                var response = new UserResponse
                {
                    Id = user.Id,
                    Name = user.Name,
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by ID");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Create a new User.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/User/Create?name=NewUserName
        /// 
        /// </remarks>
        /// <param name="name">The name of the user.</param>
        /// <returns>A boolean indicating if the user was created successfully.</returns>
        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> CreateUser(string name)
        {
            try
            {
                var addSuccessfully = await _userService.CreateUser(name);
                if (addSuccessfully)
                {
                    return CreatedAtAction(nameof(GetUserById), new { id = addSuccessfully }, true);
                }
                else
                {
                    return BadRequest("Failed to create user");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update an existing User.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT api/User/Update?id=1&amp;name=NewUserUpdated
        /// 
        /// </remarks>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="name">The new name of the user.</param>
        /// <returns>A boolean indicating if the user was updated successfully.</returns>
        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> UpdateUser(int id, string name)
        {
            try
            {
                var userDto = new UserDto
                {
                    Id = id,
                    Name = name
                };

                var updateSuccessfully = await _userService.UpdateUser(userDto);
                if (!updateSuccessfully)
                {
                    return BadRequest("User not updated");
                }
                return Ok(updateSuccessfully);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete a User by ID.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE api/User/Delete?id=1
        /// 
        /// </remarks>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>A boolean indicating if the user was deleted successfully.</returns>
        [HttpDelete("Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> DeleteUser(int id)
        {
            try
            {
                var deleteSuccessfully = await _userService.DeleteUser(id);
                if (!deleteSuccessfully)
                {
                    return BadRequest("User not deleted");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
