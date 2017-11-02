// ******************************************************************

// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

// ******************************************************************

using Microsoft.Knowzy.Common.Contracts;
using Microsoft.Knowzy.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Windows.Storage;

namespace Microsoft.Knowzy.DataProvider
{
    public class JsonDataProvider
    {
        private string _jsonFilePath = "ms-appx:///Products.json";

        private StorageFile _file;

        public JsonDataProvider()
        {
        }

        public async Task<Product[]> GetDataAsync()
        {
            var jsonFilePath = _jsonFilePath;

            _file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(_jsonFilePath));

            return JsonConvert.DeserializeObject<Product[]>(await FileIO.ReadTextAsync(_file));
        }

        public async Task<Product> GetDataByIdAsync(string id)
        {
            _file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(_jsonFilePath));

            var products = JsonConvert.DeserializeObject<Product[]>(await FileIO.ReadTextAsync(_file));

            var product = (from p in products where p.Id == id select p).FirstOrDefault();

            return product;
        }

        public async void SetDataAsync(Product[] products)
        {
            _file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(_jsonFilePath));

            await FileIO.WriteTextAsync(_file, JsonConvert.SerializeObject(products));
        }
    }
}
