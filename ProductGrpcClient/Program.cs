using Google.Protobuf.WellKnownTypes;
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


            await GetAllProducts(client);
            await AddProductAsync(client);
            await GetProductAsync(client, 4);
            await UpdateProductAsync(client);
            await GetProductAsync(client, 1);
            await DeleteProductAsync(client);
            await GetAllProducts(client);

            await InsertBulkProduct(client);
            Console.ReadLine();
        }

        private static async Task InsertBulkProduct(ProductProtoService.ProductProtoServiceClient client)
        {
            Console.WriteLine("InsertBulkProdcut started.");
            using var clientBulk = client.InsertBulkProduct();

            for (int i = 0; i < 3; i++)
            {
                var productModel = new ProductModel
                {
                    Name = $"Product{i}",
                    Description = "Bulk inserted product",
                    Price = 300,
                    Status = ProductStatus.Instock,
                    CreatedTime = Timestamp.FromDateTime(DateTime.UtcNow),
                };

                await clientBulk.RequestStream.WriteAsync(productModel);
            }

            await clientBulk.RequestStream.CompleteAsync();

            var responseBulk = await clientBulk;
            Console.WriteLine($"Status: {responseBulk.Success}. Insert Count: {responseBulk.InsertCount}");
        }

        private static async Task DeleteProductAsync(ProductProtoService.ProductProtoServiceClient client)
        {
            Console.WriteLine("DeleteProductAsync started.");
            var response = await client.DeleteProductAsync(new DeleteProductRequest
            {
                ProductId = 4
            });
            Console.WriteLine("DeleteProductAsync response : " + response.Success.ToString());
        }

        private static async Task UpdateProductAsync(ProductProtoService.ProductProtoServiceClient client)
        {
            Console.WriteLine("UpdateProductAsync started.");
            var response = await client.UpdateProductAsync(new UpdateProductRequest
            {
                Product = new ProductModel
                {
                    ProductId = 1,
                    Name = "Mi10T",
                    Description = "New Iphone 14 Pro",
                    Price = 3800,
                    Status = ProductStatus.Instock,
                    CreatedTime = Timestamp.FromDateTime(DateTime.UtcNow),
                }
            });
            Console.WriteLine("UpdateProductAsync response : " + response.ToString());
        }

        public static async Task GetProductAsync(ProductProtoService.ProductProtoServiceClient client, int productId)
        {
            Console.WriteLine("GetProductAsync started.");
            var response = await client.GetProductAsync(new GetProductRequest { ProductId = productId });
            Console.WriteLine("response : " + response);

        }

        public static async Task GetAllProducts(ProductProtoService.ProductProtoServiceClient client)
        {
            Console.WriteLine("GetAllProducts started.");
            using (var clientData = client.GetAllProducts(new GetAllProductsRequest()))
            {
                while (await clientData.ResponseStream.MoveNext(new System.Threading.CancellationToken()))
                {
                    var currentProduct = clientData.ResponseStream.Current;
                    Console.WriteLine("response : " + currentProduct);
                }
            }   
        }
        private static async Task AddProductAsync(ProductProtoService.ProductProtoServiceClient client)
        {
            Console.WriteLine("AddProductAsync started.");

            var addProductResponse = await client.AddProductAsync(new AddProductRequest
            {
                Product = new ProductModel
                {
                    Name = "Red",
                    Description = "New Red Iphone",
                    Price = 699,
                    Status = ProductStatus.Instock,
                    CreatedTime = Timestamp.FromDateTime(DateTime.UtcNow),
                }
            });

            Console.WriteLine("response : " + addProductResponse.ToString());
        }
    }
}
