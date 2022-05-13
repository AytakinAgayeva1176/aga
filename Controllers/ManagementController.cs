using AGA.Helpers;
using AGA.Models;
using AGA.ViewModels;
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
    public class ManagementController : ControllerBase
    {
        #region ctor
        private readonly TokenSettings tokenSettings;
        private readonly string uri = "http://172.16.10.132:3574/nc/aga_project_tyds/api/v1/administrative_staff";
        public ManagementController(TokenSettings tokenSettings)
        {
            this.tokenSettings = tokenSettings;
        }

        #endregion


        #region Create
        /// <summary>
        /// Create Staff
        /// </summary>
        /// <param name="model">Staff model </param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Create(StaffVM model)
        {
            var uniq_id = Guid.NewGuid().ToString();
            Staff staff = new Staff()
            {
                activity = model.activity,
                uniq_id = uniq_id,
                job_title = model.job_title,
                biography = model.biography,
                fullname = model.fullname,
                main_image = model.main_image,
                slug = model.slug,
                social_media = JsonConvert.SerializeObject(model.social_media),
                images = JsonConvert.SerializeObject(model.images)
            };

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var data = JsonConvert.SerializeObject(staff);
            StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");
            var json = await client.PostAsync(uri, queryString);

            if (json.IsSuccessStatusCode)
            {

                var EmpResponse = json.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<Staff>(EmpResponse);
                return Ok(new SystemMessaging(MesagesCode.Insert, "Staff insert succesfully", item));
            }

            return BadRequest();
        }


        #endregion


        #region GetByUnique_Id / FindOne
        /// <summary>
        /// Get Staff By UniqId
        /// </summary>
        /// <param name="uniq_id"> Unique id of Staff </param>
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
                var item = JsonConvert.DeserializeObject<Staff>(EmpResponse);
                if (item == null)
                {
                    var result = new SystemMessaging(MesagesCode.Delete, "Staff doesn't exist");
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
                var items = JsonConvert.DeserializeObject<List<Staff>>(EmpResponse);

                return Ok(items);

            }
            return BadRequest();
        }

        #endregion


        #region Update
        /// <summary>
        /// Update Staff
        /// </summary>
        /// <param name="model">Staff model</param>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update(Staff model)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var data = JsonConvert.SerializeObject(model);
            StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");
            var json = await client.PutAsync(uri + "/" + model.id, queryString);
            if (json.IsSuccessStatusCode) return Ok(new SystemMessaging(MesagesCode.Update, "Staff updated succesfully!", model));
            else return BadRequest(new SystemMessaging(MesagesCode.Exception, "Staff couldn't update!", model));

        }


        #endregion


        #region Delete
        /// <summary>
        /// Delete Staff By UniqId
        /// </summary>
        /// <param name="uniq_id"> Unique id of Staff </param>
        /// <returns></returns>
        [HttpDelete("Delete/{uniq_id}")]
        public async Task<IActionResult> Delete(string uniq_id)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var Staff = await client.GetAsync(uri + "/findOne?where=(uniq_id,like," + uniq_id + ")");
            if (Staff.IsSuccessStatusCode)
            {
                var EmpResponse = Staff.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<Staff>(EmpResponse);
                if (item != null)
                {
                    var json = await client.DeleteAsync(uri + "/" + item.id);

                    if (json.IsSuccessStatusCode)
                    {
                        EmpResponse = json.Content.ReadAsStringAsync().Result;
                        if (EmpResponse == "1")
                        {
                            return Ok(new SystemMessaging(MesagesCode.Delete, "Staff deleted succesfully!"));
                        }
                        else return BadRequest(new SystemMessaging(MesagesCode.Delete, "Staff couldn't deleted!"));
                    }
                }
                else return BadRequest(new SystemMessaging(MesagesCode.Delete, "Staff doesn't exist"));
            }

            return BadRequest();
        }


        #endregion

    }
}
