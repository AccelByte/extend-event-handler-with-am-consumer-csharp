// Copyright (c) 2026 AccelByte Inc. All Rights Reserved.
// This is licensed software from AccelByte Inc, for limitations
// and restrictions contact your company contract manager.

using System;
using System.Text.Json;
using System.Threading.Tasks;
using AccelByte.Extend.AsyncMessaging;
using AccelByte.Sdk.Api;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace AccelByte.PluginArch.EventHandler.Demo.Server.Services
{
    public class ConsumerService : AsyncMessagingConsumerService.AsyncMessagingConsumerServiceBase
    {
        private readonly IAccelByteServiceProvider _ABProvider;
        private readonly ILogger<ConsumerService> _Logger;

        public ConsumerService(
            ILogger<ConsumerService> logger,
            IAccelByteServiceProvider abProvider)
        {
            _ABProvider = abProvider;
            _Logger = logger;
        }

        public override async Task<Empty> onMessage(ReceivedMessage request, ServerCallContext context)
        {
            _Logger.LogInformation($"Received message with topic: {request.Topic}, body: {request.Body}");

            string playerId = "";
            if (request.Metadata.ContainsKey("PlayerId"))
                playerId = request.Metadata["PlayerId"].Trim();

            if (playerId != "")
            {
                if (_ABProvider.IsStoreEnabled)
                {
                    PlayerJoinedEvent? eventData = JsonSerializer.Deserialize<PlayerJoinedEvent>(request.Body);
                    if (eventData == null)
                    {
                        _Logger.LogError("Failed to deserialized message body to PlayerJoinedEvent object.");
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Cannot deserialized json."));
                    }

                    string storeKey = $"player_joined_event_{playerId}";

                    _ = await _ABProvider.Sdk.Cloudsave.AdminGameRecord.AdminPostGameRecordHandlerV1Op
                        .ExecuteAsync(eventData, storeKey, _ABProvider.Sdk.Namespace);
                }
            }
            else
            {
                _Logger.LogInformation("PlayerId not found in metadata");
            }

            return await Task.FromResult(new Empty());
        }
    }
}
