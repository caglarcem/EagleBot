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
        }

        /// <summary>
        ///     Returns all the inspection data of all EagleBots.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TrafficDataDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> TrafficData()
        {
            var result = await _trafficDataService.GetAllTrafficData();

            return Ok(result);
        }

        /// <summary>
        ///     Returns the latest inspection data of all the active EagleBots.
        ///     Active EagleBot are bots that have not been sending data for more than 1 hour.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TrafficDataDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> LatestTrafficDataOfActiveBots()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Returns the latest data of the specified EagleBot.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TrafficDataDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> LatestTrafficDataByEagleBot(string eagleBotId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Saves the traffic inspection data sent by an EagleBot.
        /// </summary>
        /// <param name="trafficData"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SaveTrafficData([FromBody] TrafficDataDto trafficData)
        {
            if(string.IsNullOrEmpty(trafficData?.EagleBotId))
            {
                return BadRequest();
            }

            var trafficDataModel = Mapper.Map<TrafficDataModel>(trafficData);

            var result = await _trafficDataService.SaveTrafficData(trafficDataModel);

            return Ok(result);
        }
    }
}