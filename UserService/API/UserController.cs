using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserService.API.DTO;
using UserService.Application;
using UserService.Domain;

namespace UserService.API;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService, IMapper mapper) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly IMapper _mapper = mapper;

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser(UserToCreate userToCreate)
    {
        var user = _mapper.Map<User>(userToCreate);
        var createdUser = await _userService.CreateUserAsync(user).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllUsers() =>
        Ok(await _userService.GetAllUsersAsync().ConfigureAwait(false));
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUser([Required]Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id).ConfigureAwait(false);
        return user is not null ? Ok(user) : NotFound();
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUser(UserToUpdate userToUpdate)
    {
        var user = _mapper.Map<User>(userToUpdate);
        var userFound = await _userService.GetUserByIdAsync(user.Id).ConfigureAwait(false);
        if (userFound is null) return NotFound();
        var userModified = await _userService.UpdateUserAsync(user).ConfigureAwait(false);
        return Ok(userModified);
    }
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteUser([Required]Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id).ConfigureAwait(false);
        if (user is null) return  NotFound();
        await _userService.DeleteUserAsync(id).ConfigureAwait(false);
        return NoContent();
    }
}