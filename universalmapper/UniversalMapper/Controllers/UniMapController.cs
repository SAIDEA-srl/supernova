using OrangeButton.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System;
using UniversalMapper.Models;

namespace UniversalMapper.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UniMapController : ControllerBase
{
    
    private readonly ILogger<UniMapController> _logger;
    private readonly UniversalMapperDbContext dbContext;

    public UniMapController(ILogger<UniMapController> logger,
        UniversalMapperDbContext dbContext)
    {
        _logger = logger;
        this.dbContext = dbContext;
    }


    private async Task<Guid?> FindCollectionUUID(string source, string? id)
    {        
        if (Guid.TryParse(source, out Guid uuid))
        {
            var identifier = await dbContext.Maps.AsNoTracking()
                .Where(t => t.UUID == uuid)
                .FirstOrDefaultAsync();

            return identifier?.UUID;
        }
        else
        {
            var identifier = await dbContext.Maps.AsNoTracking()
                .Where(t => t.AlternativeIdentifier.SourceName.Value == source)
                .Where(t => t.AlternativeIdentifier.Identifier.Value == id)
                .FirstOrDefaultAsync();

            return identifier?.UUID;
        }
    }

    /// <summary>
    /// Find all identifiers of a specific collection
    /// </summary>
    /// <param name="collectionUUID">Collection UUID</param>
    /// <returns>A list of all alternativeIdentifier registred</returns>
    [HttpGet("{collectionUUID}")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(List<AlternativeIdentifier>))]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    public async Task<Results<NotFound, Ok<List<AlternativeIdentifier>>>> GetIdentifiers(Guid collectionUUID)
    {
        var mappings = await dbContext.Maps.AsNoTracking()
            .Where(t => t.UUID == collectionUUID)
            .ToListAsync();

        return TypedResults.Ok(mappings.Select(t => t.AlternativeIdentifier).ToList());
    }


    /// <summary>
    /// Find all identifiers of a specific compoment
    /// </summary>
    /// <param name="source">Source of identifier</param>
    /// <param name="id">Name of the component in source</param>
    /// <returns>A list of all alternativeIdentifier registred</returns>
    [HttpGet("{source}/{id}")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(List<AlternativeIdentifier>))]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    public async Task<Results<NotFound, Ok<List<AlternativeIdentifier>>>> GetIdentifiers(string source, string? id = null)
    {

        Guid? collectionUUID = await FindCollectionUUID(source, id);

        if (collectionUUID == null) {
            return TypedResults.NotFound();
        }

        var mappings = await dbContext.Maps.AsNoTracking()
            .Where(t => t.UUID == collectionUUID)
            .ToListAsync();
        
        return TypedResults.Ok(mappings.Select(t => t.AlternativeIdentifier).ToList());
    }


    /// <summary>
    /// Add new identifier for a specific source
    /// </summary>
    /// <param name="source">source of identifier</param>
    /// <param name="alternativeIdentifier">AlternavieIdentifier definition</param>
    /// <returns>Collection UUID</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /UniMap/test
    ///     {
    ///         "SourceName": {
    ///             "Value": "test"
    ///         },
    ///         "Identifier": {
    ///             "Value": "INV01"
    ///         },
    ///         "Description": {
    ///             "Value": "inverter name"
    ///         },
    ///         "IdentifierType": {
    ///             "Value": "Other"
    ///         }
    ///     }
    /// </remarks>
    [HttpPost("{source}")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Guid))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    public async Task<Results<BadRequest, Ok<Guid>>> AddIdentifier(string source, [FromBody] AlternativeIdentifier alternativeIdentifier)
    {

        var model = new Models.UUIDMap()
        {
            UUID = Guid.NewGuid(),
            AlternativeIdentifier = alternativeIdentifier
        };

        //force source
        model.AlternativeIdentifier.SourceName.Value = source;

        dbContext.Maps.Add(model);
        await dbContext.SaveChangesAsync();


        return TypedResults.Ok(model.UUID);
    }

    /// <summary>
    /// Map a new identifier to an existing identifier
    /// </summary>
    /// <param name="source">Source of exisiting indentifier or Collection UUID</param>
    /// <param name="exsistingId">Identifier to search</param>
    /// <param name="alternativeIdentifier">AlternativeIdentifier to add in collection</param>
    /// <returns>Collection UUID</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /UniMap/test/INV01
    ///     {
    ///         "SourceName": {
    ///             "Value": "otherplatform"
    ///         },
    ///         "Identifier": {
    ///             "Value": "INV.0001"
    ///         },
    ///         "Description": {
    ///             "Value": "another description"
    ///         },
    ///         "IdentifierType": {
    ///             "Value": "Other"
    ///         }
    ///     }
    /// </remarks>
    [HttpPost("{source}/{exsistingId}")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Guid))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    public async Task<Results<BadRequest, Ok<Guid>>> AddIdentifierToExsisting(string source, string exsistingId, [FromBody] AlternativeIdentifier alternativeIdentifier)
    {
        Guid? collectionUUID = await FindCollectionUUID(source, exsistingId);

        if (collectionUUID == null)
        {
            return TypedResults.BadRequest();
        }

        var model = new Models.UUIDMap()
        {
            UUID = collectionUUID.Value,
            AlternativeIdentifier = alternativeIdentifier
        };

        dbContext.Maps.Add(model);

        await dbContext.SaveChangesAsync();

        return TypedResults.Ok(collectionUUID.Value);
    }

    /// <summary>
    /// Update an existing Identifier
    /// </summary>
    /// <param name="source">Source of exisiting indentifier</param>
    /// <param name="exsistingId">Identifier to search</param>
    /// <param name="alternativeIdentifier">AlternativeIdentifier to update</param>
    /// <returns>Collection UUID</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /UniMap/otherplatform/INV.0001
    ///     {
    ///         "SourceName": {
    ///             "Value": "otherplatform"
    ///         },
    ///         "Identifier": {
    ///             "Value": "INV.0001-A"
    ///         },
    ///         "Description": {
    ///             "Value": "another description"
    ///         },
    ///         "IdentifierType": {
    ///             "Value": "Other"
    ///         }
    ///     }
    /// </remarks>
    [HttpPut("{source}/{exsistingId}")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Guid))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    public async Task<Results<BadRequest, Ok<Guid>>> UpdateIdentifier(string source, string exsistingId, [FromBody] AlternativeIdentifier alternativeIdentifier)
    {
        var identifier = await dbContext.Maps
            .Where(t => t.AlternativeIdentifier.SourceName.Value == source)
            .Where(t => t.AlternativeIdentifier.Identifier.Value == exsistingId)
            .FirstOrDefaultAsync();

        if (identifier == null)
        {
            return TypedResults.BadRequest();
        }

        identifier.AlternativeIdentifier = alternativeIdentifier;
        //force source
        identifier.AlternativeIdentifier.SourceName.Value = source;
        await dbContext.SaveChangesAsync();

        return TypedResults.Ok(identifier.UUID);
    }

    /// <summary>
    /// Delete an identifier
    /// </summary>
    /// <param name="source">Source of exisiting indentifier</param>
    /// <param name="exsistingId">Identifier to search</param>
    /// <returns>No Content</returns>
    [HttpDelete("{source}/{exsistingId}")]
    [SwaggerResponse(StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    public async Task<Results<BadRequest, Ok>> DeleteIdentifiers(string source, string exsistingId)
    {
        var identifier = await dbContext.Maps.Where(t =>
            t.AlternativeIdentifier.SourceName.Value == source &&
            t.AlternativeIdentifier.Identifier.Value == exsistingId
        ).FirstOrDefaultAsync();

        if (identifier == null)
        {
            return TypedResults.BadRequest();
        }

        dbContext.Maps.Remove(identifier);
        await dbContext.SaveChangesAsync();
        
        return TypedResults.Ok();
    }
}
