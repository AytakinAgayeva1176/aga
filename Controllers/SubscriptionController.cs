using AGA.Helpers;
using AGA.Models;
using AGA.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AGA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        #region ctor
        private readonly TokenSettings tokenSettings;
        private readonly string uri = "http://172.16.10.132:3574/nc/aga_project_tyds/api/v1/subscriptions";
        public SubscriptionController(TokenSettings tokenSettings)
        {
            this.tokenSettings = tokenSettings;
        }

        #endregion


        #region Create
        /// <summary>
        /// Create Subscription
        /// </summary>
        /// <param name="model">Subscription model </param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Create(SubscriptionVM model)
        {
            var uniq_id = Guid.NewGuid().ToString();
            Subscription Subscription = new Subscription()
            {
                uniq_id = uniq_id,
                email = model.email,
                source = model.source
            };

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var data = JsonConvert.SerializeObject(Subscription);
            StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");
            var json = await client.PostAsync(uri, queryString);

            if (json.IsSuccessStatusCode)
            {
                var EmpResponse = json.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<Subscription>(EmpResponse);
                return Ok(new SystemMessaging(MesagesCode.Insert, "Subscription insert succesfully", item));
            }

            return BadRequest();
        }


        #endregion


        #region GetAll

        /// <summary>
        /// GetAll
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var json = await client.GetAsync(uri);
            if (json.IsSuccessStatusCode)
            {
                var EmpResponse = json.Content.ReadAsStringAsync().Result;
                var items = JsonConvert.DeserializeObject<List<Subscription>>(EmpResponse);

                return Ok(items);

            }
            return BadRequest();
        }

        #endregion
    }
}
