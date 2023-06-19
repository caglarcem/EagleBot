namespace EagleRock.Gateway.Dtos
{
    public class TrafficDataReadDto : BaseTrafficDataDto
    {
        /// <summary>
        ///     Date Time the data is saved
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
