using System.ComponentModel.DataAnnotations;
using AddressBookService.API.DTO;
using AddressBookService.Application;
using AddressBookService.Domain;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AddressBookService.API;

[ApiController]
[Route("api/[controller]")]
public class AddressBookController(IAddressBookService addressBookService, IMapper mapper) : ControllerBase
{
    private readonly IAddressBookService _addressBookService = addressBookService;
    private readonly IMapper _mapper = mapper;

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAddress(AddressToCreate addressToCreate)
    {
        var address = _mapper.Map<Address>(addressToCreate);
        var createdAddress = await _addressBookService.CreateAddressAsync(address).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetAddress), new { id = createdAddress.Id }, createdAddress);
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllAddresses() =>
        Ok(await _addressBookService.GetAllAddressesAsync().ConfigureAwait(false));
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAddress([Required]Guid id)
    {
        var address = await _addressBookService.GetAddressByIdAsync(id).ConfigureAwait(false);
        return address is not null ? Ok(address) : NotFound();
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAddress(AddressToUpdate addressToUpdate)
    {
        var address = _mapper.Map<Address>(addressToUpdate);
        var addressFound = await _addressBookService.GetAddressByIdAsync(address.Id).ConfigureAwait(false);
        if (addressFound is null) return NotFound();
        var addressModified = await _addressBookService.UpdateAddressAsync(address).ConfigureAwait(false);
        return Ok(addressModified);
    }
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteAddress([Required]Guid id)
    {
        var address = await _addressBookService.GetAddressByIdAsync(id).ConfigureAwait(false);
        if (address is null) return  NotFound();
        await _addressBookService.DeleteAddressAsync(id).ConfigureAwait(false);
        return NoContent();
    }
}