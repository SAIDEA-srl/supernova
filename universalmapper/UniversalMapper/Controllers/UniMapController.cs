using OrangeButton.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System;
using UniversalMapper.Models;
using System.Collections.ObjectModel;

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
            var query = dbContext.Maps.AsNoTracking()
                .Where(t => t.AlternativeIdentifier.SourceName.Value == source);

            if (id != null)
            {
                query = query.Where(t => t.AlternativeIdentifier.Identifier.Value == id);
            }

            var identifier = await query.FirstOrDefaultAsync();

            return identifier?.UUID;
        }
    }

    /// <summary>
    /// Find all identifiers of a specific source
    /// </summary>
    /// <param name="source">source name</param>
    /// <returns>A list of all alternativeIdentifier registred for a specific source</returns>
    [HttpGet("{source}")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(List<AlternativeIdentifier>))]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    public async Task<Results<NotFound, Ok<List<AlternativeIdentifier>>>> GetAllIdentifiersOfSource(string source)
    {
        var mappings = await dbContext.Maps.AsNoTracking()
            .Where(t => t.AlternativeIdentifier.SourceName.Value == source)
            .ToListAsync();

        return TypedResults.Ok(mappings.Select(t => t.AlternativeIdentifier).ToList());
    }

    /// <summary>
    /// Find all identifiers of a specific compoment
    /// </summary>
    /// <param name="source">Source of identifier</param>
    /// <param name="id">Name of the component in source</param>
    /// <returns>A list of all alternativeIdentifier registred</returns>
    [HttpGet("{source}/{id?}")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(List<AlternativeIdentifier>))]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    public async Task<Results<NotFound, Ok<List<AlternativeIdentifier>>>> GetIdentifiers(string source, string id)
    {

        Guid? collectionUUID = await FindCollectionUUID(source, id);

        if (collectionUUID == null)
        {
            return TypedResults.NotFound();
        }

        var mappings = await dbContext.Maps.AsNoTracking()
            .Where(t => t.UUID == collectionUUID)
            .ToListAsync();

        return TypedResults.Ok(mappings.Select(t => t.AlternativeIdentifier).ToList());
    }


    /// <summary>
    /// Find all identifiers of a specific collection
    /// </summary>
    /// <param name="collectionUUID">Collection UUID</param>
    /// <returns>A list of all alternativeIdentifier registred</returns>
    [HttpGet("collection/{collectionUUID}")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Collection))]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    public async Task<Results<NotFound, Ok<Collection>>> GetCollectionIdentifiers(Guid collectionUUID)
    {
        var mappings = await dbContext.Maps.AsNoTracking()
            .Where(t => t.UUID == collectionUUID)
            .ToListAsync();

        return TypedResults.Ok(new Collection()
        {
            CollectionId = collectionUUID,
            Identifiers = mappings.Select(t => t.AlternativeIdentifier).ToList()
        });
    }

    /// <summary>
    /// Search an identifier
    /// </summary>
    /// <param name="identifier">identifier to search</param>
    /// <returns>A list of all alternativeIdentifier registred</returns>
    [HttpGet("search/{identifier}")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(List<Collection>))]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    public async Task<Results<NotFound, Ok<List<Collection>>>> SearchIdentifiers(string identifier)
    {
        var identifiers = await dbContext.Maps.Where(t =>
            t.AlternativeIdentifier.Identifier.Value == identifier
        ).ToListAsync();

        var collectionIds = identifiers.Select(t => t.UUID).ToList();

        var collections = await dbContext.Maps.Where(t => collectionIds.Contains(t.UUID))
            .GroupBy(t => t.UUID)
            .Select(t => new Collection()
            {
                CollectionId = t.Key,
                Identifiers = t.Select(t => t.AlternativeIdentifier).ToList()
            })
            .ToListAsync();

        return TypedResults.Ok(collections);
    }


    /// <summary>
    /// Add new identifier for a specific source
    /// </summary>
    /// <param name="source">source of identifier</param>
    /// <param name="alternativeIdentifier">AlternativeIdentifier definition</param>
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
    /// <param name="existingId">Identifier to search</param>
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
    [HttpPost("{source}/{existingId}")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Guid))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    public async Task<Results<BadRequest, Ok<Guid>>> AddIdentifierToExisting(string source, string existingId, [FromBody] AlternativeIdentifier alternativeIdentifier)
    {
        Guid? collectionUUID = await FindCollectionUUID(source, existingId);

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
    /// <param name="existingId">Identifier to search</param>
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
    [HttpPut("{source}/{existingId}")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Guid))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    public async Task<Results<BadRequest, Ok<Guid>>> UpdateIdentifier(string source, string existingId, [FromBody] AlternativeIdentifier alternativeIdentifier)
    {
        var identifier = await dbContext.Maps
            .Where(t => t.AlternativeIdentifier.SourceName.Value == source)
            .Where(t => t.AlternativeIdentifier.Identifier.Value == existingId)
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
    /// <param name="existingId">Identifier to search</param>
    /// <param name="all">Optional query parameter for delete all collection</param>
    /// <returns>No Content</returns>
    [HttpDelete("{source}/{existingId}")]
    [SwaggerResponse(StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    public async Task<Results<BadRequest, Ok>> DeleteIdentifiers(string source, string existingId, [FromQuery] bool all = false)
    {
        var identifier = await dbContext.Maps.Where(t =>
            t.AlternativeIdentifier.SourceName.Value == source &&
            t.AlternativeIdentifier.Identifier.Value == existingId
        ).FirstOrDefaultAsync();

        if (identifier == null)
        {
            return TypedResults.BadRequest();
        }

        if (all)
        {
            var allIdentfiers = await dbContext.Maps.Where(t => t.UUID == identifier.UUID).ToListAsync();
            dbContext.RemoveRange(allIdentfiers);
        }
        else
        {
            dbContext.Maps.Remove(identifier);
        }

        await dbContext.SaveChangesAsync();

        return TypedResults.Ok();
    }
}
