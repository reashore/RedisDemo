using System;
using System.Collections.Generic;
using System.Text;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;

namespace RedisDemo
{
    public class Program
    {
        public static void Main()
        {
            DemoRedisNativeClient();
            DemoRedisClient();
            DemoRedisTypedClient();

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private static void DemoRedisNativeClient()
        {
            const string key = "urn:messages:1";

            using (IRedisNativeClient redisNativeClient = new RedisClient())
            {
                redisNativeClient.Set(key, Encoding.UTF8.GetBytes("Hello World"));
            }

            using (IRedisNativeClient redisNativeClient = new RedisClient())
            {
                byte[] bytes = redisNativeClient.Get(key);
                string result = Encoding.UTF8.GetString(bytes);
                Console.WriteLine($"Message = {result}");
            }
        }

        private static void DemoRedisClient()
        {
            const string customerListKey = "urn:customerList";

            using (IRedisClient redisClient = new RedisClient())
            {
                IRedisList customerList = redisClient.Lists[customerListKey];
                customerList.Clear();
                customerList.Add("Joe");
                customerList.Add("Mary");
                customerList.Add("Bob");
                customerList.Add("Frank");
            }

            using (IRedisClient redisClient = new RedisClient())
            {
                IRedisList customerList = redisClient.Lists[customerListKey];

                foreach (string customerName in customerList)
                {
                    Console.WriteLine($"Customer: {customerName}");
                }
            }
        }

        private static void DemoRedisTypedClient()
        {
            long lastCustomerId = 0;

            using (IRedisClient redisClient = new RedisClient())
            {
                IRedisTypedClient<Customer> customerClient = redisClient.GetTypedClient<Customer>();
                Customer customer = new Customer
                {
                    Id = customerClient.GetNextSequence(),
                    Name = "Frank",
                    Address = "123 Main Street",
                    Orders = new List<Order>
                    {
                        new Order {OrderNumber = "123", OrderTotal = 100.00m},
                        new Order {OrderNumber = "123", OrderTotal = 100.00m}
                    }
                };
                Customer savedCustomer = customerClient.Store(customer);
                lastCustomerId = savedCustomer.Id;
            }

            using (IRedisClient redisClient = new RedisClient())
            {
                IRedisTypedClient<Customer> customerClient = redisClient.GetTypedClient<Customer>();
                Customer customer = customerClient.GetById(lastCustomerId);
                Console.WriteLine($"Customer = {customer.Id}, {customer.Name}");
            }
        }
    }

    public class Customer
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public List<Order> Orders { get; set; }
    }

    public class Order
    {
        public string OrderNumber { get; set; }
        public decimal OrderTotal { get; set; }
    }
}
