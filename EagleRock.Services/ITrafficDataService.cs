﻿using EagleRock.Services.Models;

namespace EagleRock.Services
{
    public interface ITrafficDataService
    {
        Task<string> SaveTrafficData(TrafficDataModel trafficData);

        Task<IEnumerable<TrafficDataModel>> GetAllTrafficData();

        Task<IEnumerable<TrafficDataModel>> GetLatestTrafficDataOfActiveBots();

        Task<TrafficDataModel> GetLatestTrafficDataByEagleBot(string eagleBotId);
    }
}
