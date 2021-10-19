using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using APIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System;
using Microsoft.AspNetCore.Http;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace APIConsume.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            List<Product> ProductList = new List<Product>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:44324/api/Product"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    ProductList = JsonConvert.DeserializeObject<List<Product>>(apiResponse);
                }
            }
            return View(ProductList);
        }

        public ViewResult GetProduct() => View();

        [HttpPost]
        public async Task<IActionResult> GetProduct(int id)
        {
            Product product = new Product();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:44324/api/Product/" + id))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        product = JsonConvert.DeserializeObject<Product>(apiResponse);
                    }
                    else
                        ViewBag.StatusCode = response.StatusCode;
                }
            }
            return View(product);
        }

        public ViewResult AddProduct() => View();

        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product)
        {
            Product receivedProduct = new Product();
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://localhost:44324/api/Product", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    receivedProduct = JsonConvert.DeserializeObject<Product>(apiResponse);
                }
            }
            return View(receivedProduct);
        }

       public async Task<IActionResult> UpdateProduct(int id)
        {
            Product product = new Product();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:44324/api/Product/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    product = JsonConvert.DeserializeObject<Product>(apiResponse);
                }
            }
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            Product receivedProduct = new Product();
            using (var httpClient = new HttpClient())
            {
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(product.Id.ToString()), "Id");
                content.Add(new StringContent(product.Name), "Name");
                content.Add(new StringContent(product.Description), "Description");
                content.Add(new StringContent(product.ExpirationDate.ToShortDateString()), "ExpirationDate");
                content.Add(new StringContent(product.DateOfProduction.ToShortDateString()), "DateOfProduction");
                content.Add(new StringContent(product.Price.ToString()), "Price");
                content.Add(new StringContent(product.Quantity.ToString()), "Quantity");

                using (var response = await httpClient.PutAsync("https://localhost:44324/api/Product", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    ViewBag.Result = "Success";
                    receivedProduct = JsonConvert.DeserializeObject<Product>(apiResponse);
                }
            }
            return View(receivedProduct);
        }

        public async Task<IActionResult> UpdateProductPatch(int id)
        {
            Product product = new Product();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:44324/api/Product/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    product = JsonConvert.DeserializeObject<Product>(apiResponse);
                }
            }
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProductPatch(int id, Product product)
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri("https://localhost:44324/api/Product/" + id),
                    Method = new HttpMethod("Patch"),
                    Content = new StringContent("[" +
                            "{ \"op\":\"replace\", \"path\":\"Name\", \"value\":\"" + product.Name + "\"}" +
                            ",{ \"op\":\"replace\", \"path\":\"Description\", \"value\":\"" + product.Description + "\"}" +
                            //",{ \"op\":\"replace\", \"path\":\"ExpirationDate\", \"value\":\"" + product.ExpirationDate.ToShortDateString() + "\"}" +
                            //",{ \"op\":\"replace\", \"path\":\"DateOfProduction\", \"value\":\"" + product.DateOfProduction.ToShortDateString() + "\"}" +
                            ",{ \"op\":\"replace\", \"path\":\"Price\", \"value\":\"" + product.Price.ToString() + "\"}" +
                            ",{ \"op\":\"replace\", \"path\":\"Quantity\", \"value\":\"" + product.Quantity.ToString()
                            + "\"}" +
                            "]", Encoding.UTF8, "application/json")
                };

                var response = await httpClient.SendAsync(request);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int ProductId)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync("https://localhost:44324/api/Product/" + ProductId))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                }
            }

            return RedirectToAction("Index");
        }

        public ViewResult AddFile() => View();

        [HttpPost]
        public async Task<IActionResult> AddFile(IFormFile file)
        {
            string apiResponse = "";
            using (var httpClient = new HttpClient())
            {
                var form = new MultipartFormDataContent();
                using (var fileStream = file.OpenReadStream())
                {
                    form.Add(new StreamContent(fileStream), "file", file.FileName);
                    using (var response = await httpClient.PostAsync("https://localhost:44324/api/Product/UploadFile", form))
                    {
                        apiResponse = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            return View((object)apiResponse);
        }

        public ViewResult AddProductByXml() => View();

        [HttpPost]
        public async Task<IActionResult> AddProductByXml(Product product)
        {
            Product receivedProduct = new Product();

            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(ConvertObjectToXMLString(product), Encoding.UTF8, "application/xml");

                using (var response = await httpClient.PostAsync("https://localhost:44324/api/Product/PostXml", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    receivedProduct = JsonConvert.DeserializeObject<Product>(apiResponse);
                }
            }
            return View(receivedProduct);
        }

        string ConvertObjectToXMLString(object classObject)
        {
            string xmlString = null;
            XmlSerializer xmlSerializer = new XmlSerializer(classObject.GetType());
            using (MemoryStream memoryStream = new MemoryStream())
            {
                xmlSerializer.Serialize(memoryStream, classObject);
                memoryStream.Position = 0;
                xmlString = new StreamReader(memoryStream).ReadToEnd();
            }
            return xmlString;
        }
    }
}