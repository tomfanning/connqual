using Newtonsoft.Json;
using RestSharp;
using shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace connqual_client
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new UdpClient();
            client.Connect("m0lte.uk", 2000);

            var restClient = new RestClient("http://m0lte.uk:5000");

            int i = 0;
            Console.WriteLine("Press enter to send");
            while (true)
            {
                Console.ReadLine();
                var json = new Frame { Sent = DateTime.UtcNow, Sequence = i++ };
                byte[] frame = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(json));
                client.Send(frame, frame.Length);
                Console.Write("sent");

                List<WrappedFrame> data = null;

                var sw = Stopwatch.StartNew();

                bool found = false;
                while (sw.Elapsed < TimeSpan.FromSeconds(10))
                {
                    var restResponse = restClient.Get(new RestRequest("/api/frames")).Content;

                    data = JsonConvert.DeserializeObject<List<WrappedFrame>>(restResponse);

                    if (data != null && data.Any(d => d.Frame.UniqueId == json.UniqueId))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Console.WriteLine("Lost that datagram");
                }
                else
                {
                    Console.WriteLine($"Latest delay: {data.OrderByDescending(d => d.Frame.Sent).First().DelayMs:0.00}ms");
                }

                if (data.Any())
                {
                    Console.WriteLine($"Recent average delay: {data.Average(d => d.DelayMs):0.00}ms");
                }
                else
                {
                    Debugger.Break();
                }
                
            }
        }
    }
}