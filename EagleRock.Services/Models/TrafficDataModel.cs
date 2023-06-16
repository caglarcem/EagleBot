namespace EagleRock.Services.Models
{
    public class TrafficDataModel
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

        /// <summary>
        ///     Time the data is saved in total milliseconds since DateTime.MinValue
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
