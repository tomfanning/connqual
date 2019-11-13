using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using connqual_server.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using shared;

namespace connqual_server
{
    internal class UdpListener : IHostedService
    {
        public UdpListener(ILogger<UdpListener> logger, FrameService frameService)
        {
            this.logger = logger;
            this.frameService = frameService;
        }

        Task service;
        private readonly ILogger<UdpListener> logger;
        private readonly FrameService frameService;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        UdpClient server;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            service = Task.Run(BackgroundTask);

            return Task.CompletedTask;
        }

        public async Task BackgroundTask()
        {
            server = new UdpClient(new IPEndPoint(IPAddress.Any, 2000));

            while (!cancellationTokenSource.IsCancellationRequested)
            {
                UdpReceiveResult receiveResult;

                try
                {
                    receiveResult = await server.ReceiveAsync().WithCancellation(this.cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    server.Close();
                    return;
                }

                string json = Encoding.UTF8.GetString(receiveResult.Buffer);

                var frame = JsonConvert.DeserializeObject<Frame>(json);
                frameService.Frames.Add((frame, DateTime.UtcNow));

                logger.LogInformation($"{DateTime.UtcNow - frame.Sent}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            cancellationTokenSource.Cancel();
            return Task.CompletedTask;
        }
    }

    public static class AsyncExtensions
    {
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
            {
                if (task != await Task.WhenAny(task, tcs.Task))
                {
                    throw new OperationCanceledException(cancellationToken);
                }
            }

            return task.Result;
        }
    }
}