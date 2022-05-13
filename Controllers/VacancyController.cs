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
    public class VacancyController : ControllerBase
    {
        #region ctor
        private readonly TokenSettings tokenSettings;
        private readonly string uri = "http://172.16.10.132:3574/nc/aga_project_tyds/api/v1/vacancies";
        public VacancyController(TokenSettings tokenSettings)
        {
            this.tokenSettings = tokenSettings;
        }

        #endregion


        #region Create
        /// <summary>
        /// Create Vacancy
        /// </summary>
        /// <param name="model">Vacancy model </param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Create(VacancyVM model)
        {
            var uniq_id = Guid.NewGuid().ToString();

            Vacancy Vacancy = new Vacancy()
            {
                uniq_id = uniq_id,
                created_at = DateTime.Now,
                deadline = model.deadline,
                sector_uniq_id = model.sector_uniq_id,
                brand_uniq_id = model.brand_uniq_id,
                job_title = model.job_title,
                description = model.description,
                job_duties = model.job_duties,
                job_requirements = JsonConvert.SerializeObject(model.job_requirements),
                location = model.location,
                salary = model.salary,
                schedule_type = model.schedule_type,
                schedule = model.schedule,
                other_information = model.other_information,
                slug = model.slug,
                status = "active"
            };

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var data = JsonConvert.SerializeObject(Vacancy);
            StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");
            var json = await client.PostAsync(uri, queryString);

            if (json.IsSuccessStatusCode)
            {

                var EmpResponse = json.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<Vacancy>(EmpResponse);
                return Ok(new SystemMessaging(MesagesCode.Insert, "Vacancy insert succesfully", item));
            }

            return BadRequest();
        }


        #endregion


        #region GetByUnique_Id / FindOne
        /// <summary>
        /// Get Vacancy By UniqId
        /// </summary>
        /// <param name="uniq_id"> Unique id of Vacancy </param>
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
                var item = JsonConvert.DeserializeObject<Vacancy>(EmpResponse);
                if (item == null)
                {
                    var result = new SystemMessaging(MesagesCode.Delete, "Vacancy doesn't exist");
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
                var items = JsonConvert.DeserializeObject<List<Vacancy>>(EmpResponse);

                return Ok(items);

            }
            return BadRequest();
        }

        #endregion


        #region GetAllActive

        /// <summary>
        /// GetAllActive
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllActive")]
        public async Task<IActionResult> GetAllActive()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var json = await client.GetAsync(uri + "?where=(status,eq,active)");
            if (json.IsSuccessStatusCode)
            {
                var EmpResponse = json.Content.ReadAsStringAsync().Result;
                var items = JsonConvert.DeserializeObject<List<Vacancy>>(EmpResponse);

                return Ok(items);

            }
            return BadRequest();
        }

        #endregion


        #region GetAllWithFilter

        /// <summary>
        /// GetAllWithFilter
        /// </summary>
        /// <param name="model">Vacancy Search  model </param>
        /// <returns></returns>
        [HttpPost("GetAllWithFilter")]
        public async Task<IActionResult> GetAllWithFilter(VacancySearchModel model)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var json = await client.GetAsync(uri);
            if (json.IsSuccessStatusCode)
            {
                var EmpResponse = json.Content.ReadAsStringAsync().Result;
                var items = JsonConvert.DeserializeObject<List<Vacancy>>(EmpResponse);

                if (!string.IsNullOrEmpty(model.brand_uniq_id))
                {
                    items = items.Where(c => c.brand_uniq_id == model.brand_uniq_id).ToList();
                }

                if (!string.IsNullOrEmpty(model.sector_uniq_id))
                {
                    items = items.Where(c => c.sector_uniq_id == model.sector_uniq_id).ToList();
                }

                if (!string.IsNullOrEmpty(model.job_title))
                {
                    items = items.Where(c => c.job_title == model.job_title).ToList();
                }

                return Ok(items);

            }
            return BadRequest();
        }



        #endregion


        #region Update
        /// <summary>
        /// Update Vacancy
        /// </summary>
        /// <param name="model">Vacancy model</param>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update(Vacancy model)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var data = JsonConvert.SerializeObject(model);
            StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");
            var json = await client.PutAsync(uri + "/" + model.id, queryString);
            if (json.IsSuccessStatusCode) return Ok(new SystemMessaging(MesagesCode.Update, "Vacancy updated succesfully!", model));
            else return BadRequest(new SystemMessaging(MesagesCode.Exception, "Vacancy couldn't update!", model));

        }


        #endregion


        #region Activate
        /// <summary>
        /// Activate Vacancy
        /// </summary>
        /// <param name="uniq_id">uniq_id of Vacancy</param>
        /// <returns></returns>
        [HttpPut("Activate")]
        public async Task<IActionResult> Activate(string uniq_id)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var Vacancy = await client.GetAsync(uri + "/findOne?where=(uniq_id,like," + uniq_id + ")");
            if (Vacancy.IsSuccessStatusCode)
            {
                var EmpResponse = Vacancy.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<Vacancy>(EmpResponse);
                if (item != null)
                {
                    item.status = "active";
                    var data = JsonConvert.SerializeObject(item);
                    StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");

                    var json = await client.PutAsync(uri + "/" + item.id, queryString);

                    if (json.IsSuccessStatusCode) return Ok(new SystemMessaging(MesagesCode.Update, "Vacancy activated succesfully!", item));
                    else return BadRequest(new SystemMessaging(MesagesCode.Exception, "Vacancy couldn't activated!", item));
                }
                else return BadRequest(new SystemMessaging(MesagesCode.NotFound, "Vacancy doesn't exist"));
            }

            return BadRequest();
        }


        #endregion


        #region Deactivate
        /// <summary>
        /// Deactivate Vacancy
        /// </summary>
        /// <param name="uniq_id">uniq_id of Vacancy</param>
        /// <returns></returns>
        [HttpPut("Deactivate")]
        public async Task<IActionResult> Deactivate(string uniq_id)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var Vacancy = await client.GetAsync(uri + "/findOne?where=(uniq_id,like," + uniq_id + ")");
            if (Vacancy.IsSuccessStatusCode)
            {
                var EmpResponse = Vacancy.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<Vacancy>(EmpResponse);
                if (item != null)
                {
                    item.status = "deactive";
                    var data = JsonConvert.SerializeObject(item);
                    StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");

                    var json = await client.PutAsync(uri + "/" + item.id, queryString);

                    if (json.IsSuccessStatusCode) return Ok(new SystemMessaging(MesagesCode.Update, "Vacancy deactivated succesfully!", item));
                    else return BadRequest(new SystemMessaging(MesagesCode.Exception, "Vacancy couldn't deactivated!", item));
                }
                else return BadRequest(new SystemMessaging(MesagesCode.NotFound, "Vacancy doesn't exist"));
            }

            return BadRequest();
        }


        #endregion


        #region Delete
        /// <summary>
        /// Delete Vacancy By UniqId
        /// </summary>
        /// <param name="uniq_id"> Unique id of Vacancy </param>
        /// <returns></returns>
        [HttpDelete("Delete/{uniq_id}")]
        public async Task<IActionResult> Delete(string uniq_id)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var Vacancy = await client.GetAsync(uri + "/findOne?where=(uniq_id,like," + uniq_id + ")");
            if (Vacancy.IsSuccessStatusCode)
            {
                var EmpResponse = Vacancy.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<Vacancy>(EmpResponse);
                if (item != null)
                {
                    var json = await client.DeleteAsync(uri + "/" + item.id);

                    if (json.IsSuccessStatusCode)
                    {

                        EmpResponse = json.Content.ReadAsStringAsync().Result;
                        if (EmpResponse == "1")
                        {
                            return Ok(new SystemMessaging(MesagesCode.Delete, "Vacancy deleted succesfully!"));
                        }
                        else return BadRequest(new SystemMessaging(MesagesCode.Delete, "Vacancy couldn't deleted!"));

                    }
                }
                else return BadRequest(new SystemMessaging(MesagesCode.Delete, "Vacancy doesn't exist"));
            }

            return BadRequest();
        }


        #endregion


    }
}
