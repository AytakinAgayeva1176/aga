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
    public class ContactUsController : ControllerBase
    {
        #region ctor
        private readonly TokenSettings tokenSettings;
        private readonly string uri = "http://172.16.10.132:3574/nc/aga_project_tyds/api/v1/contact_us";
        public ContactUsController(TokenSettings tokenSettings)
        {
            this.tokenSettings = tokenSettings;
        }

        #endregion


        #region Create
        /// <summary>
        /// Create ContactUs
        /// </summary>
        /// <param name="model">ContactUs model </param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Create(ContactUsVM model)
        {
            var uniq_id = Guid.NewGuid().ToString();
            ContactUs ContactUs = new ContactUs()
            {
                uniq_id = uniq_id,
                email = model.email,
                fullname = model.fullname,
                created_at=DateTime.Now,
                message = model.message,    
                phone = model.phone,
                status="pending"
            };

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var data = JsonConvert.SerializeObject(ContactUs);
            StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");
            var json = await client.PostAsync(uri, queryString);

            if (json.IsSuccessStatusCode)
            {
                var EmpResponse = json.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<ContactUs>(EmpResponse);
                return Ok(new SystemMessaging(MesagesCode.Insert, "ContactUs insert succesfully", item));
            }

            return BadRequest();
        }


        #endregion


        #region GetByUnique_Id / FindOne
        /// <summary>
        /// Get ContactUs By UniqId
        /// </summary>
        /// <param name="uniq_id"> Unique id of ContactUs </param>
        /// <returns></returns>
        [HttpGet("GetByUniqueId/{uniq_id}")]
        public async Task<IActionResult> GetByUniqueId(string uniq_id)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var json = await client.GetAsync(uri + "/findOne?where=(uniq_id,like," + uniq_id + ")");
            if (json.IsSuccessStatusCode)
            {
                var EmpResponse = json.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<ContactUs>(EmpResponse);
                if (item == null)
                {
                    var result = new SystemMessaging(MesagesCode.Delete, "ContactUs doesn't exist");
                    return Ok(result);
                }
                return Ok(item);

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
                var items = JsonConvert.DeserializeObject<List<ContactUs>>(EmpResponse);

                return Ok(items);

            }
            return BadRequest();
        }

        #endregion


        #region Delete
        /// <summary>
        /// Delete ContactUs By UniqId
        /// </summary>
        /// <param name="uniq_id"> Unique id of ContactUs </param>
        /// <returns></returns>
        [HttpDelete("Delete/{uniq_id}")]
        public async Task<IActionResult> Delete(string uniq_id)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var ContactUs = await client.GetAsync(uri + "/findOne?where=(uniq_id,like," + uniq_id + ")");
            if (ContactUs.IsSuccessStatusCode)
            {
                var EmpResponse = ContactUs.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<ContactUs>(EmpResponse);
                if (item != null)
                {
                    var json = await client.DeleteAsync(uri + "/" + item.id);

                    if (json.IsSuccessStatusCode)
                    {
                        EmpResponse = json.Content.ReadAsStringAsync().Result;
                        if (EmpResponse == "1")
                        {
                            return Ok(new SystemMessaging(MesagesCode.Delete, "ContactUs deleted succesfully!"));
                        }
                        else return BadRequest(new SystemMessaging(MesagesCode.Delete, "ContactUs couldn't deleted!"));
                    }
                }
                else return BadRequest(new SystemMessaging(MesagesCode.Delete, "ContactUs doesn't exist"));
            }

            return BadRequest();
        }


        #endregion
    }
}
