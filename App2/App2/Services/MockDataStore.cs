using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using App2.Models;
using Newtonsoft.Json;

namespace App2.Services
{
    public class MockDataStore : IDataStore<Item>
    {
        List<Item> items;

        public MockDataStore()
        {
            items = new List<Item>();

            using (var webClient = new WebClient())
            {
                var response = webClient.DownloadString(
                    "https://education-erp.com/api/ClientApplication/News?schoolType=Football&cityId=4&count=15");
                var mockItems = JsonConvert.DeserializeObject<LinkedList<New>>(response);
                foreach (var item in mockItems)
                {
                    var t = Regex.Replace(item.Text, @"<img[^>]*>", String.Empty);
                    t = Regex.Replace(t, @"<br[^>]*>", "\n");
                    t = Regex.Replace(t, @"<p[^>]*>", String.Empty);
                    t = Regex.Replace(t, @"</p[^>]*>", "\n");
                    t = Regex.Replace(t, @"&nbsp;", " ");
                    t = Regex.Replace(t, @"&mdash;", "–");
                    t = Regex.Replace(t, @"&laquo;", "«");
                    t = Regex.Replace(t, @"&raquo;", "»");
                    t = Regex.Replace(t, @"<a href[*]*h", "h");
                    t = Regex.Replace(t, @"<span[^>]*>", String.Empty);
                    t = Regex.Replace(t, @"</span[^>]*>", String.Empty);
                    t = Regex.Replace(t, @"<[^>]*>", String.Empty);

                    items.Add(new Item()
                    {
                        Id = item.Id.ToString(),
                        Description = t,
                        Text = item.Title
                    }) ;
                }
            }
        }

        //public async Task<bool> AddItemAsync(Item item)
        //{
        //    items.Add(item);

        //    return await Task.FromResult(true);
        //}

        //public async Task<bool> UpdateItemAsync(Item item)
        //{
        //    var oldItem = items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
        //    items.Remove(oldItem);
        //    items.Add(item);

        //    return await Task.FromResult(true);
        //}

        //public async Task<bool> DeleteItemAsync(string id)
        //{
        //    var oldItem = items.Where((Item arg) => arg.Id == id).FirstOrDefault();
        //    items.Remove(oldItem);

        //    return await Task.FromResult(true);
        //}

        public async Task<Item> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}