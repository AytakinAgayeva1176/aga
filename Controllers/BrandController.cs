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
    public class BrandController : ControllerBase
    {
        #region ctor
        private readonly TokenSettings tokenSettings;
        private readonly IMapper mapper;
        private readonly string uri = "http://172.16.10.132:3574/nc/aga_project_tyds/api/v1/brands";
        public BrandController(TokenSettings tokenSettings, IMapper mapper)
        {
            this.tokenSettings = tokenSettings;
            this.mapper = mapper;
        }

        #endregion


        #region Create
        /// <summary>
        /// Create Brand
        /// </summary>
        /// <param name="model">Brand model </param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Create(BrandVM model)
        {
            var uniq_id = Guid.NewGuid().ToString();
            var brand = mapper.Map<Brand>(model);
            brand.uniq_id = uniq_id;
          
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var data = JsonConvert.SerializeObject(brand);
            StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");
            var json = await client.PostAsync(uri, queryString);

            if (json.IsSuccessStatusCode)
            {

                var EmpResponse = json.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<Brand>(EmpResponse);
                return Ok(new SystemMessaging(MesagesCode.Insert, "Brand insert succesfully", item));
            }

            return BadRequest();
        }


        #endregion


        #region GetByUnique_Id / FindOne
        /// <summary>
        /// Get Brand By UniqId
        /// </summary>
        /// <param name="uniq_id"> Unique id of Brand </param>
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
                var item = JsonConvert.DeserializeObject<Brand>(EmpResponse);
                if (item == null)
                {
                    var result = new SystemMessaging(MesagesCode.Delete, "Brand doesn't exist");
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
                var items = JsonConvert.DeserializeObject<List<Brand>>(EmpResponse);

                return Ok(items);

            }
            return BadRequest();
        }

        #endregion


        #region Update
        /// <summary>
        /// Update Brand
        /// </summary>
        /// <param name="model">Brand model</param>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update(Brand model)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var data = JsonConvert.SerializeObject(model);
            StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");
            var json = await client.PutAsync(uri + "/" + model.id, queryString);
            if (json.IsSuccessStatusCode) return Ok(new SystemMessaging(MesagesCode.Update, "Brand updated succesfully!", model));
            else return BadRequest(new SystemMessaging(MesagesCode.Exception, "Brand couldn't update!", model));

        }


        #endregion


        #region Delete
        /// <summary>
        /// Delete Brand By UniqId
        /// </summary>
        /// <param name="uniq_id"> Unique id of Brand </param>
        /// <returns></returns>
        [HttpDelete("Delete/{uniq_id}")]
        public async Task<IActionResult> Delete(string uniq_id)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var Brand = await client.GetAsync(uri + "/findOne?where=(uniq_id,like," + uniq_id + ")");
            if (Brand.IsSuccessStatusCode)
            {
                var EmpResponse = Brand.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<Brand>(EmpResponse);
                if (item != null)
                {
                    var json = await client.DeleteAsync(uri + "/" + item.id);

                    if (json.IsSuccessStatusCode)
                    {

                        EmpResponse = json.Content.ReadAsStringAsync().Result;
                        if (EmpResponse == "1")
                        {
                            return Ok(new SystemMessaging(MesagesCode.Delete, "Brand deleted succesfully!"));
                        }
                        else return BadRequest(new SystemMessaging(MesagesCode.Delete, "Brand couldn't deleted!"));

                    }
                }
                else return BadRequest(new SystemMessaging(MesagesCode.Delete, "Brand doesn't exist"));
            }

            return BadRequest();
        }


        #endregion
    }
}
