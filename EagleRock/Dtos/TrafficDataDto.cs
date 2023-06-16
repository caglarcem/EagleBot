using EagleRock.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagleRock.Gateway.Dtos
{
    public class TrafficDataDto
    {
        /// <summary>
        ///     Unique identifier of the Eagle bot
        /// </summary>
        public string EagleBotId { get; set; }

        /// <summary>
        ///     Road under inspection
        /// </summary>
        public string RoadName { get; set; }

        /// <summary>
        ///     Direction of traffic flow
        /// </summary>
        public Direction TrafficDirection { get; set; }

        /// <summary>
        ///     Rate of traffic flow
        /// </summary>
        public int VehiclesPerMinute { get; set; }

        /// <summary>
        ///     Average speed of vehicles at the time of inspection
        /// </summary>
        public double AverageVehicleSpeed { get; set; }

        /// <summary>
        ///     Current location of the bot
        /// </summary>
        public Location BotLocation { get; set; }
    }
}
