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
    public class SliderController : ControllerBase
    {
        #region ctor
        private readonly TokenSettings tokenSettings;
        private readonly string uri = "http://172.16.10.132:3574/nc/aga_project_tyds/api/v1/hero_slider";
        public SliderController(TokenSettings tokenSettings)
        {
            this.tokenSettings = tokenSettings;
        }

        #endregion


        #region Create
        /// <summary>
        /// Create Slider
        /// </summary>
        /// <param name="model">Slider model </param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Create(SliderVM model)
        {
            var uniq_id = Guid.NewGuid().ToString();
            Slider slider = new Slider()
            {
                description = model.description,
                uniq_id = uniq_id,
                title = model.title,
                priority = model.priority,
                f_index_id = model.f_index_id,
                media_path = model.media_path,
                button = JsonConvert.SerializeObject(model.button),
                file = model.file,
                status = "active"
            };

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var data = JsonConvert.SerializeObject(slider);
            StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");
            var json = await client.PostAsync(uri, queryString);

            if (json.IsSuccessStatusCode)
            {

                var EmpResponse = json.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<Slider>(EmpResponse);
                return Ok(new SystemMessaging(MesagesCode.Insert, "Slider insert succesfully", item));
            }

            return BadRequest();
        }


        #endregion


        #region GetByUnique_Id / FindOne
        /// <summary>
        /// Get Slider By UniqId
        /// </summary>
        /// <param name="uniq_id"> Unique id of Slider </param>
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
                var item = JsonConvert.DeserializeObject<Slider>(EmpResponse);
                if (item == null)
                {
                    var result = new SystemMessaging(MesagesCode.Delete, "Slider doesn't exist");
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
                var items = JsonConvert.DeserializeObject<List<Slider>>(EmpResponse);

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
                var items = JsonConvert.DeserializeObject<List<Slider>>(EmpResponse);

                return Ok(items);

            }
            return BadRequest();
        }

        #endregion


        #region Update
        /// <summary>
        /// Update Slider
        /// </summary>
        /// <param name="model">Slider model</param>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update(Slider model)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var data = JsonConvert.SerializeObject(model);
            StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");
            var json = await client.PutAsync(uri + "/" + model.id, queryString);
            if (json.IsSuccessStatusCode) return Ok(new SystemMessaging(MesagesCode.Update, "Slider updated succesfully!", model));
            else return BadRequest(new SystemMessaging(MesagesCode.Exception, "Slider couldn't update!", model));

        }


        #endregion


        #region UpdateRange
        /// <summary>
        ///  Priority Update Range
        /// </summary>
        /// <param name="pairs"> Dictionary(uniq_id,priority) </param>
        /// <returns></returns>
        [HttpPut("PriorityUpdateRange")]
        public async Task<IActionResult> PriorityUpdateRange(Dictionary<string, int> pairs)
        {
            List<Slider> sliderList = new List<Slider>();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            foreach (var item in pairs)
            {
                var response = await client.GetAsync(uri + "/findOne?where=(uniq_id,like," + item.Key + ")");
                var EmpResponse = response.Content.ReadAsStringAsync().Result;
                var slider = JsonConvert.DeserializeObject<Slider>(EmpResponse);
                slider.priority = item.Value;
                sliderList.Add(slider);
            }

            var data = JsonConvert.SerializeObject(sliderList);
            StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");
            var json = await client.PutAsync(uri + "/bulk", queryString);
            if (json.IsSuccessStatusCode) return Ok(new SystemMessaging(MesagesCode.Update, "updated succesfully!", sliderList));
            else return BadRequest(new SystemMessaging(MesagesCode.Exception, "couldn't update!", sliderList));

        }


        #endregion


        #region Activate
        /// <summary>
        /// Activate Slider
        /// </summary>
        /// <param name="uniq_id">uniq_id of Slider</param>
        /// <returns></returns>
        [HttpPut("Activate")]
        public async Task<IActionResult> Activate(string uniq_id)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var slider = await client.GetAsync(uri + "/findOne?where=(uniq_id,like," + uniq_id + ")");
            if (slider.IsSuccessStatusCode)
            {
                var EmpResponse = slider.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<Slider>(EmpResponse);
                if (item != null)
                {
                    item.status = "active";
                    var data = JsonConvert.SerializeObject(item);
                    StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");

                    var json = await client.PutAsync(uri + "/" + item.id, queryString);

                    if (json.IsSuccessStatusCode) return Ok(new SystemMessaging(MesagesCode.Update, "Slider activated succesfully!", item));
                    else return BadRequest(new SystemMessaging(MesagesCode.Exception, "Slider couldn't activated!", item));
                }
                else return BadRequest(new SystemMessaging(MesagesCode.NotFound, "Slider doesn't exist"));
            }

            return BadRequest();
        }


        #endregion


        #region Deactivate
        /// <summary>
        /// Deactivate Slider
        /// </summary>
        /// <param name="uniq_id">uniq_id of Slider</param>
        /// <returns></returns>
        [HttpPut("Deactivate")]
        public async Task<IActionResult> Deactivate(string uniq_id)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var slider = await client.GetAsync(uri + "/findOne?where=(uniq_id,like," + uniq_id + ")");
            if (slider.IsSuccessStatusCode)
            {
                var EmpResponse = slider.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<Slider>(EmpResponse);
                if (item != null)
                {
                    item.status = "deactive";
                    var data = JsonConvert.SerializeObject(item);
                    StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");

                    var json = await client.PutAsync(uri + "/" + item.id, queryString);

                    if (json.IsSuccessStatusCode) return Ok(new SystemMessaging(MesagesCode.Update, "Slider deactivated succesfully!", item));
                    else return BadRequest(new SystemMessaging(MesagesCode.Exception, "Slider couldn't deactivated!", item));
                }
                else return BadRequest(new SystemMessaging(MesagesCode.NotFound, "Slider doesn't exist"));
            }

            return BadRequest();
        }


        #endregion


        #region Delete
        /// <summary>
        /// Delete Slider By UniqId
        /// </summary>
        /// <param name="uniq_id"> Unique id of Slider </param>
        /// <returns></returns>
        [HttpDelete("Delete/{uniq_id}")]
        public async Task<IActionResult> Delete(string uniq_id)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var slider = await client.GetAsync(uri + "/findOne?where=(uniq_id,like," + uniq_id + ")");
            if (slider.IsSuccessStatusCode)
            {
                var EmpResponse = slider.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<Slider>(EmpResponse);
                if (item != null)
                {
                    var json = await client.DeleteAsync(uri + "/" + item.id);

                    if (json.IsSuccessStatusCode)
                    {

                        EmpResponse = json.Content.ReadAsStringAsync().Result;
                        if (EmpResponse == "1")
                        {
                            return Ok(new SystemMessaging(MesagesCode.Delete, "slider deleted succesfully!"));
                        }
                        else return BadRequest(new SystemMessaging(MesagesCode.Delete, "slider couldn't deleted!"));

                    }
                }
                else return BadRequest( new SystemMessaging(MesagesCode.Delete, "slider doesn't exist"));
            }

            return BadRequest();
        }


        #endregion

       
    }
}
