using AutoMapper;
using EagleRock.Gateway.Dtos;
using EagleRock.Services;
using EagleRock.Services.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace EagleRock.Gateway.Controllers
{
    [Route("api/[controller]/[action]")]
    public class TrafficDataController : Controller
    {
        private static readonly IMapper Mapper;
        private readonly ITrafficDataService _trafficDataService;
        private readonly ILogger<TrafficDataController> _logger;

        static TrafficDataController()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TrafficDataModel, TrafficDataDto>().ReverseMap();
            });
            Mapper = new Mapper(config);
        }

        public TrafficDataController(ITrafficDataService trafficDataService, ILogger<TrafficDataController> logger)
        {
            _trafficDataService = trafficDataService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TrafficDataDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> TrafficData()
        {
            var result = await _trafficDataService.GetAllTrafficData();

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SaveTrafficData([FromBody] TrafficDataDto trafficData)
        {
            var trafficDataModel = Mapper.Map<TrafficDataModel>(trafficData);

            var result = await _trafficDataService.SaveTrafficData(trafficDataModel);

            return Ok(result);
        }
    }
}