﻿using System;
using System.Threading.Tasks;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Client.Models;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Delay(10000).Wait();
            var client = new Lykke.Service.BlockchainCashoutPreconditionsCheck.Client.BlockchainCashoutPreconditionsCheckClient("http://localhost:5000",null);
            var (isAllowed, listOfErrors) = client.ValidateCashoutAsync(new Client.Models.CashoutValidateModel()
            {
                AssetId = "62c04960-4015-4945-bb7e-8e4a193b3653",
                DestinationAddress = "0x406561f72e25ab41200fa3d52badc5a21",
                Amount = 0
            }).Result;

            client.CreateBlackListAsync(new BlackListModel("EthereumClassic", "0x81b7E08F65Bdf5648606c89998A9CC8164397647", false)).Wait();
            client.CreateBlackListAsync(new BlackListModel("EthereumClassic", "0x81b7E08F65Bdf5648606c89998A", false)).Wait();

            var (isAllowed1, listOfErrors1) = client.ValidateCashoutAsync(new Client.Models.CashoutValidateModel()
            {
                AssetId = "62c04960-4015-4945-bb7e-8e4a193b3653",
                DestinationAddress = "0x81b7E08F65Bdf5648606c89998A9CC8164397647",
                Amount = 0
            }).Result;

            var list = client.GetAllBlackListsAsync("EthereumClassic", 20).Result;
            var x = client.GetBlackListAsync("EthereumClassic", "0x81b7E08F65Bdf5648606c89998A9CC8164397647").Result;
            client.DeleteBlackListAsync("EthereumClassic", "0x81b7E08F65Bdf5648606c89998A9CC8164397647").Wait();
            var y = client.GetBlackListAsync("EthereumClassic", "0x81b7E08F65Bdf5648606c89998A9CC8164397647").Result;

            Console.ReadLine();
        }
    }
}
