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
    public class SectorController : ControllerBase
    {

        #region ctor
        private readonly TokenSettings tokenSettings;
        private readonly string uri = "http://172.16.10.132:3574/nc/aga_project_tyds/api/v1/sectors";
        public SectorController(TokenSettings tokenSettings)
        {
            this.tokenSettings = tokenSettings;
        }

        #endregion


        #region Create
        /// <summary>
        /// Create Sector
        /// </summary>
        /// <param name="title">Sector Name </param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Create(string title)
        {
            var uniq_id = Guid.NewGuid().ToString();
            Sector Sector = new Sector()
            {
                uniq_id = uniq_id,
                title = title
            };

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var data = JsonConvert.SerializeObject(Sector);
            StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");
            var json = await client.PostAsync(uri, queryString);

            if (json.IsSuccessStatusCode)
            {
                var EmpResponse = json.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<Sector>(EmpResponse);
                return Ok(new SystemMessaging(MesagesCode.Insert, "Sector insert succesfully", item));
            }

            return BadRequest();
        }


        #endregion


        #region GetByUnique_Id / FindOne
        /// <summary>
        /// Get Sector By UniqId
        /// </summary>
        /// <param name="uniq_id"> Unique id of Sector </param>
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
                var item = JsonConvert.DeserializeObject<Sector>(EmpResponse);
                if (item == null)
                {
                    var result = new SystemMessaging(MesagesCode.Delete, "Sector doesn't exist");
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
                var items = JsonConvert.DeserializeObject<List<Sector>>(EmpResponse);

                return Ok(items);

            }
            return BadRequest();
        }

        #endregion


        #region Delete
        /// <summary>
        /// Delete Sector By UniqId
        /// </summary>
        /// <param name="uniq_id"> Unique id of Sector </param>
        /// <returns></returns>
        [HttpDelete("Delete/{uniq_id}")]
        public async Task<IActionResult> Delete(string uniq_id)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var sector = await client.GetAsync(uri + "/findOne?where=(uniq_id,like," + uniq_id + ")");
            if (sector.IsSuccessStatusCode)
            {
                var EmpResponse = sector.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<Sector>(EmpResponse);
                if (item != null)
                {
                    var json = await client.DeleteAsync(uri + "/" + item.id);

                    if (json.IsSuccessStatusCode)
                    {
                        EmpResponse = json.Content.ReadAsStringAsync().Result;
                        if (EmpResponse == "1")
                        {
                            return Ok(new SystemMessaging(MesagesCode.Delete, "Sector deleted succesfully!"));
                        }
                        else return BadRequest(new SystemMessaging(MesagesCode.Delete, "Sector couldn't deleted!"));

                    }
                }
                else return BadRequest(new SystemMessaging(MesagesCode.Delete, "Sector doesn't exist"));
            }

            return BadRequest();
        }


        #endregion


    }
}
