using AGA.Helpers;
using AGA.Models;
using AGA.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
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
    public class ScholarshipController : ControllerBase
    {
        
        #region ctor
        private readonly TokenSettings tokenSettings;
        private readonly IMapper mapper;
        private readonly string uri = "http://172.16.10.132:3574/nc/aga_project_tyds/api/v1/scholarships";
        public ScholarshipController(TokenSettings tokenSettings, IMapper mapper)
        {
            this.tokenSettings = tokenSettings;
            this.mapper = mapper;   
        }

        #endregion


        #region Create
        /// <summary>
        /// Create Scholarship
        /// </summary>
        /// <param name="model">Scholarship model </param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Create(ScholarshipVM model)
        {
            var uniq_id = Guid.NewGuid().ToString();
            var scholarship = mapper.Map<Scholarship>(model);
            scholarship.uniq_id = uniq_id;
            scholarship.status = "pending";

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var data = JsonConvert.SerializeObject(scholarship);
            StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");
            var json = await client.PostAsync(uri, queryString);

            if (json.IsSuccessStatusCode)
            {

                var EmpResponse = json.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<Scholarship>(EmpResponse);
                return Ok(new SystemMessaging(MesagesCode.Insert, "Scholarship insert succesfully", item));
            }

            return BadRequest();
        }


        #endregion


        #region GetByUnique_Id / FindOne
        /// <summary>
        /// Get Scholarship By UniqId
        /// </summary>
        /// <param name="uniq_id"> Unique id of Scholarship </param>
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
                var item = JsonConvert.DeserializeObject<Scholarship>(EmpResponse);
                if (item == null)
                {
                    var result = new SystemMessaging(MesagesCode.Delete, "Scholarship doesn't exist");
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
                var items = JsonConvert.DeserializeObject<List<Scholarship>>(EmpResponse);

                return Ok(items);

            }
            return BadRequest();
        }

        #endregion


        #region Update
        /// <summary>
        /// Update Scholarship
        /// </summary>
        /// <param name="uniq_id">Scholarship uniq_id</param>
        ///<param name="status">Scholarship status</param>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update(string uniq_id, string status)
        {

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var Scholarship = await client.GetAsync(uri + "/findOne?where=(uniq_id,like," + uniq_id + ")");
            if (Scholarship.IsSuccessStatusCode)
            {
                var EmpResponse = Scholarship.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<Scholarship>(EmpResponse);

                if (item != null)
                {
                    item.status = status;
                    var data = JsonConvert.SerializeObject(item);
                    StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");
                    var json = await client.PutAsync(uri + "/" + item.id, queryString);
                    if (json.IsSuccessStatusCode) return Ok(new SystemMessaging(MesagesCode.Update, "Scholarship updated succesfully!", item));
                    else return BadRequest(new SystemMessaging(MesagesCode.Exception, "Scholarship couldn't update!", item));

                }
                else
                {
                    return BadRequest(new SystemMessaging(MesagesCode.Delete, "Scholarship doesn't exist"));
                }

            }
            return BadRequest(new SystemMessaging(MesagesCode.Delete, "Scholarship couldn't find"));
        }


        #endregion


        #region Delete
        /// <summary>
        /// Delete Scholarship By UniqId
        /// </summary>
        /// <param name="uniq_id"> Unique id of Scholarship </param>
        /// <returns></returns>
        [HttpDelete("Delete/{uniq_id}")]
        public async Task<IActionResult> Delete(string uniq_id)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var Scholarship = await client.GetAsync(uri + "/findOne?where=(uniq_id,like," + uniq_id + ")");
            if (Scholarship.IsSuccessStatusCode)
            {
                var EmpResponse = Scholarship.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<Scholarship>(EmpResponse);
                if (item != null)
                {
                    var json = await client.DeleteAsync(uri + "/" + item.id);

                    if (json.IsSuccessStatusCode)
                    {

                        EmpResponse = json.Content.ReadAsStringAsync().Result;
                        if (EmpResponse == "1")
                        {
                            return Ok(new SystemMessaging(MesagesCode.Delete, "Scholarship deleted succesfully!"));
                        }
                        else return BadRequest(new SystemMessaging(MesagesCode.Delete, "Scholarship couldn't deleted!"));
                    }
                }
                else return BadRequest(new SystemMessaging(MesagesCode.Delete, "Scholarship doesn't exist"));
            }

            return BadRequest();
        }


        #endregion


    }
}
