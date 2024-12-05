using System.Net.Mime;
using HomeStation.Application.CQRS;
using HomeStation.Application.CQRS.ReadingsQuery;
using HomeStation.Domain.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeStation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AirController : ControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher;

    public AirController(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }
        
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReadingsWebModel))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces(MediaTypeNames.Application.Json)]
    [HttpGet(template: "{DeviceId:required}")]
    public async Task<ReadingsWebModel> GetLatestReading([FromRoute] GetLatestReadingQuery readingsQuery)
    {
        return await _queryDispatcher.Dispatch<GetLatestReadingQuery, ReadingsWebModel>(readingsQuery,
            new CancellationToken());
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ReadingsWebModel>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces(MediaTypeNames.Application.Json)]
    [HttpGet(template: "{ReadingType:alpha}/{DeviceId:required}/{StartDate:datetime}/{EndDate:datetime}/{DetailLevel:alpha}", 
        Name = "GetReadings")]
    public async Task<IEnumerable<ReadingsWebModel>> GetReadings([FromRoute] GetReadingsQuery readingsQuery)
    {
        return await _queryDispatcher.Dispatch<GetReadingsQuery, IEnumerable<ReadingsWebModel>>(readingsQuery,
            new CancellationToken());
    }
}