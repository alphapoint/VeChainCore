﻿using System;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using VeChainCore.Models;

namespace VeChainCore
{
    public class VeChainClient
    {
        private string _blockchainAddress = "http://localhost:8669";

        private readonly HttpClient _client = new HttpClient();

        // Config methods
        /// <summary>
        /// Sets the address of the blockchain that the client is interacting with.
        /// </summary>
        /// <param name="address">The address of the blockchain, by default "http://localhost:8669"</param>
        public void SetBlockchainAddress(string address)
        {
            _blockchainAddress = address.TrimEnd('/');
        }

        /// <summary>
        /// Gets the address of the blockchain that the client is interacting with.
        /// </summary>
        /// <returns></returns>
        public string GetBlockchainAddress()
        {
            return _blockchainAddress;
        }



        // Logic methods
        /// <summary>
        /// Gets an <see cref="Account"/> object that contains all Account information for
        /// the given address.
        /// </summary>
        /// <param name="address">The address id in 0x notation</param>
        /// <param name="revision">The block number or ID to be able to look at past balances</param>
        /// <returns></returns>
        public async Task<Account> GetAccount(string address, string revision = "best")
        {
            if (revision != "best")
                address += $"?revision={revision}";

            var streamTask = _client.GetStreamAsync($"{_blockchainAddress}/accounts/{address}");
            Console.WriteLine(streamTask.ToString());

            var serializer = new DataContractJsonSerializer(typeof(Account));
            return serializer.ReadObject(await streamTask) as Account;
        }

        /// <summary>
        /// Gets the <see cref="Block"/> object that contains all Block information for
        /// the given block number
        /// </summary>
        /// <param name="blockNumber">The block number</param>
        /// <returns></returns>
        public async Task<Block> GetBlock(uint blockNumber)
        {
            var streamTask = _client.GetStreamAsync($"{_blockchainAddress}/blocks/{blockNumber}");
            Console.WriteLine(streamTask.ToString());

            var serializer = new DataContractJsonSerializer(typeof(Block));
            return serializer.ReadObject(await streamTask) as Block;
        }


        /// <summary>
        /// Gets the <see cref="Transaction"/> object that contains all Transaction information for
        /// the given transaction id
        /// </summary>
        /// <param name="blockNumber">The block number</param>
        /// <returns></returns>
        public async Task<Transaction> GetTransaction(string id)
        {
            var streamTask = _client.GetStreamAsync($"{_blockchainAddress}/transactions/{id}");
            Console.WriteLine(streamTask.ToString());

            var serializer = new DataContractJsonSerializer(typeof(Transaction));
            return serializer.ReadObject(await streamTask) as Transaction;
        }
    }
}
