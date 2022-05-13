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
    public class ResidentialApplicationController : ControllerBase
    {
        #region ctor
        private readonly TokenSettings tokenSettings;
        private readonly string uri = "http://172.16.10.132:3574/nc/aga_project_tyds/api/v1/residential_applications";
        public ResidentialApplicationController(TokenSettings tokenSettings)
        {
            this.tokenSettings = tokenSettings;
        }

        #endregion


        #region Create
        /// <summary>
        /// Create ResidentialApplication
        /// </summary>
        /// <param name="model">ResidentialApplication model </param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Create(ResidentialApplicationVM model)
        {
            var uniq_id = Guid.NewGuid().ToString();
            ResidentialApplication app = new ResidentialApplication()
            {
                fullname = model.fullname,
                uniq_id = uniq_id,
                sector_uniq_id = model.sector_uniq_id,
                area_size = model.area_size,
                company_name = model.company_name,
                email = model.email,
                phone = model.phone,
                location = model.location,
                status = "pending"
            };

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var data = JsonConvert.SerializeObject(app);
            StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");
            var json = await client.PostAsync(uri, queryString);

            if (json.IsSuccessStatusCode)
            {

                var EmpResponse = json.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<ResidentialApplication>(EmpResponse);
                return Ok(new SystemMessaging(MesagesCode.Insert, "ResidentialApplication insert succesfully", item));
            }

            return BadRequest();
        }


        #endregion


        #region GetByUnique_Id / FindOne
        /// <summary>
        /// Get ResidentialApplication By UniqId
        /// </summary>
        /// <param name="uniq_id"> Unique id of ResidentialApplication </param>
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
                var item = JsonConvert.DeserializeObject<ResidentialApplication>(EmpResponse);
                if (item == null)
                {
                    var result = new SystemMessaging(MesagesCode.Delete, "ResidentialApplication doesn't exist");
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
                var items = JsonConvert.DeserializeObject<List<ResidentialApplication>>(EmpResponse);

                return Ok(items);

            }
            return BadRequest();
        }

        #endregion


        #region Update
        /// <summary>
        /// Update ResidentialApplication
        /// </summary>
        /// <param name="model">ResidentialApplication model</param>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update(ResidentialApplication model)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var data = JsonConvert.SerializeObject(model);
            StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");
            var json = await client.PutAsync(uri + "/" + model.id, queryString);
            if (json.IsSuccessStatusCode) return Ok(new SystemMessaging(MesagesCode.Update, "ResidentialApplication updated succesfully!", model));
            else return BadRequest(new SystemMessaging(MesagesCode.Exception, "ResidentialApplication couldn't update!", model));

        }


        #endregion


        #region Delete
        /// <summary>
        /// Delete ResidentialApplication By UniqId
        /// </summary>
        /// <param name="uniq_id"> Unique id of ResidentialApplication </param>
        /// <returns></returns>
        [HttpDelete("Delete/{uniq_id}")]
        public async Task<IActionResult> Delete(string uniq_id)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var ResidentialApplication = await client.GetAsync(uri + "/findOne?where=(uniq_id,like," + uniq_id + ")");
            if (ResidentialApplication.IsSuccessStatusCode)
            {
                var EmpResponse = ResidentialApplication.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<ResidentialApplication>(EmpResponse);
                if (item != null)
                {
                    var json = await client.DeleteAsync(uri + "/" + item.id);

                    if (json.IsSuccessStatusCode)
                    {

                        EmpResponse = json.Content.ReadAsStringAsync().Result;
                        if (EmpResponse == "1")
                        {
                            return Ok(new SystemMessaging(MesagesCode.Delete, "ResidentialApplication deleted succesfully!"));
                        }
                        else return BadRequest(new SystemMessaging(MesagesCode.Delete, "ResidentialApplication couldn't deleted!"));

                    }
                }
                else return BadRequest(new SystemMessaging(MesagesCode.Delete, "ResidentialApplication doesn't exist"));

            }

            return BadRequest();
        }


        #endregion
    }
}
