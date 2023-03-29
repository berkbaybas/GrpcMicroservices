using Grpc.Net.Client;
using ProductGrpc.Protos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProductGrpcClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Thread.Sleep(5000);
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new ProductProtoService.ProductProtoServiceClient(channel);

            await GetProductAsync(client);
            await GetAllProducts(client);


            Console.ReadLine();
        }

        public static async Task GetProductAsync(ProductProtoService.ProductProtoServiceClient client)
        {
            Console.WriteLine("GetProductAsync started.");
            var response = await client.GetProductAsync(new GetProductRequest { ProductId = 1 });
            Console.WriteLine("response : " + response);

        }

        public static async Task GetAllProducts(ProductProtoService.ProductProtoServiceClient client)
        {
            Console.WriteLine("Get All products started.");
            using (var clientData = client.GetAllProducts(new GetAllProductsRequest()))
            {
                while (await clientData.ResponseStream.MoveNext(new System.Threading.CancellationToken()))
                {
                    var currentProduct = clientData.ResponseStream.Current;
                    Console.WriteLine("response : " + currentProduct);
                }
            }   
        }


    }
}
