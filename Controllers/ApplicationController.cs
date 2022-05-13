using AGA.Helpers;
using AGA.Models;
using AGA.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AGA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        #region ctor
        private readonly TokenSettings tokenSettings;
        private readonly string uri = "http://172.16.10.132:3574/nc/aga_project_tyds/api/v1/applications";
        public ApplicationController(TokenSettings tokenSettings)
        {
            this.tokenSettings = tokenSettings;
        }

        #endregion


        #region Create
        /// <summary>
        /// Create Application
        /// </summary>
        /// <param name="model">Application model </param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Create(ApplicationVM model)
        {
            var uniq_id = Guid.NewGuid().ToString();

            Application Application = new Application()
            {
                uniq_id = uniq_id,
                fullname = model.fullname,
                sector_uniq_id = model.sector_uniq_id,
                file = model.file,
                vacancy_id = model.vacancy_id,
                type = model.type,
                status = "pending"
            };

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var data = JsonConvert.SerializeObject(Application);
            StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");
            var json = await client.PostAsync(uri, queryString);

            if (json.IsSuccessStatusCode)
            {

                var EmpResponse = json.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<Application>(EmpResponse);
                return Ok(new SystemMessaging(MesagesCode.Insert, "Application insert succesfully", item));
            }

            return BadRequest();
        }


        #endregion


        #region GetByUnique_Id / FindOne
        /// <summary>
        /// Get Application By UniqId
        /// </summary>
        /// <param name="uniq_id"> Unique id of Application </param>
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
                var item = JsonConvert.DeserializeObject<Application>(EmpResponse);
                if (item == null)
                {
                    var result = new SystemMessaging(MesagesCode.Delete, "Application doesn't exist");
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
                var items = JsonConvert.DeserializeObject<List<Application>>(EmpResponse);

                return Ok(items);

            }
            return BadRequest();
        }

        #endregion


        #region GetAllWithFilter

        /// <summary>
        /// GetAllWithFilter
        /// </summary>
        /// <param name="model">Application Search  model </param>
        /// <returns></returns>
        [HttpPost("GetAllWithFilter")]
        public async Task<IActionResult> GetAllWithFilter(ApplicationSearchModel model)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var json = await client.GetAsync(uri);
            if (json.IsSuccessStatusCode)
            {
                var EmpResponse = json.Content.ReadAsStringAsync().Result;
                var items = JsonConvert.DeserializeObject<List<Application>>(EmpResponse);

                if (!string.IsNullOrEmpty(model.vacancy_id))
                {
                    items = items.Where(c => c.vacancy_id == model.vacancy_id).ToList();
                }

                if (!string.IsNullOrEmpty(model.sector_uniq_id))
                {
                    items = items.Where(c => c.sector_uniq_id == model.sector_uniq_id).ToList();
                }

                if (!string.IsNullOrEmpty(model.status))
                {
                    items = items.Where(c => c.status == model.status).ToList();
                }
                if (!string.IsNullOrEmpty(model.type))
                {
                    items = items.Where(c => c.type == model.type).ToList();
                }
                return Ok(items);

            }
            return BadRequest();
        }



        #endregion


        #region Update
        /// <summary>
        /// Update Application
        /// </summary>
        /// <param name="model">Application model</param>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update(Application model)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var data = JsonConvert.SerializeObject(model);
            StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");
            var json = await client.PutAsync(uri + "/" + model.id, queryString);
            if (json.IsSuccessStatusCode) return Ok(new SystemMessaging(MesagesCode.Update, "Application updated succesfully!", model));
            else return BadRequest(new SystemMessaging(MesagesCode.Exception, "Application couldn't update!", model));

        }


        #endregion


        #region Delete
        /// <summary>
        /// Delete Application By UniqId
        /// </summary>
        /// <param name="uniq_id"> Unique id of Application </param>
        /// <returns></returns>
        [HttpDelete("Delete/{uniq_id}")]
        public async Task<IActionResult> Delete(string uniq_id)
        {
             HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var Application = await client.GetAsync(uri + "/findOne?where=(uniq_id,like," + uniq_id + ")");
            if (Application.IsSuccessStatusCode)
            {
                var EmpResponse = Application.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<Application>(EmpResponse);
                if (item != null)
                {
                    var json = await client.DeleteAsync(uri + "/" + item.id);

                    if (json.IsSuccessStatusCode)
                    {

                        EmpResponse = json.Content.ReadAsStringAsync().Result;
                        if (EmpResponse == "1")
                        {
                            return Ok(new SystemMessaging(MesagesCode.Delete, "Application deleted succesfully!"));
                        }
                        else return BadRequest(new SystemMessaging(MesagesCode.Delete, "Application couldn't deleted!"));

                    }
                }
                else return BadRequest(new SystemMessaging(MesagesCode.Delete, "Application doesn't exist"));
            }

            return BadRequest();
        }


        #endregion
    }
}
