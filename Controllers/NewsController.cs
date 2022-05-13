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
    public class NewsController : ControllerBase
    {
        #region ctor
        private readonly TokenSettings tokenSettings;
        private readonly string uri = "http://172.16.10.132:3574/nc/aga_project_tyds/api/v1/news";
        public NewsController(TokenSettings tokenSettings)
        {
            this.tokenSettings = tokenSettings;
        }

        #endregion


        #region Create
        /// <summary>
        /// Create News
        /// </summary>
        /// <param name="model">News model </param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Create(NewsVM model)
        {
            var uniq_id = Guid.NewGuid().ToString();

            News news = new News()
            {
                created_at=DateTime.Now,
                content = model.content,
                uniq_id = uniq_id,
                title = model.title,
                hyperlink = model.hyperlink,
                icon= model.icon,
                slug = model.slug,
                images = JsonConvert.SerializeObject(model.images),
                status = "active"
            };


            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var data = JsonConvert.SerializeObject(news);
            StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");
            var json = await client.PostAsync(uri, queryString);

            if (json.IsSuccessStatusCode)
            {

                var EmpResponse = json.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<News>(EmpResponse);
                return Ok(new SystemMessaging(MesagesCode.Insert, "News insert succesfully", item));
            }

            return BadRequest();
        }


        #endregion


        #region GetByUnique_Id / FindOne
        /// <summary>
        /// Get News By UniqId
        /// </summary>
        /// <param name="uniq_id"> Unique id of News </param>
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
                var item = JsonConvert.DeserializeObject<News>(EmpResponse);
                if (item == null)
                {
                    var result = new SystemMessaging(MesagesCode.Delete, "News doesn't exist");
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
                var items = JsonConvert.DeserializeObject<List<News>>(EmpResponse);

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
                var items = JsonConvert.DeserializeObject<List<News>>(EmpResponse);

                return Ok(items);

            }
            return BadRequest();
        }

        #endregion


        #region Update
        /// <summary>
        /// Update News
        /// </summary>
        /// <param name="model">News model</param>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update(News model)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var data = JsonConvert.SerializeObject(model);
            StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");
            var json = await client.PutAsync(uri + "/" + model.id, queryString);
            if (json.IsSuccessStatusCode) return Ok(new SystemMessaging(MesagesCode.Update, "News updated succesfully!", model));
            else return BadRequest(new SystemMessaging(MesagesCode.Exception, "News couldn't update!", model));

        }


        #endregion


        #region Activate
        /// <summary>
        /// Activate News
        /// </summary>
        /// <param name="uniq_id">uniq_id of News</param>
        /// <returns></returns>
        [HttpPut("Activate")]
        public async Task<IActionResult> Activate(string uniq_id)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var News = await client.GetAsync(uri + "/findOne?where=(uniq_id,like," + uniq_id + ")");
            if (News.IsSuccessStatusCode)
            {
                var EmpResponse = News.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<News>(EmpResponse);
                if (item != null)
                {
                    item.status = "active";
                    var data = JsonConvert.SerializeObject(item);
                    StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");

                    var json = await client.PutAsync(uri + "/" + item.id, queryString);

                    if (json.IsSuccessStatusCode) return Ok(new SystemMessaging(MesagesCode.Update, "News activated succesfully!", item));
                    else return BadRequest(new SystemMessaging(MesagesCode.Exception, "News couldn't activated!", item));
                }
                else return BadRequest(new SystemMessaging(MesagesCode.NotFound, "News doesn't exist"));
            }

            return BadRequest();
        }


        #endregion


        #region Deactivate
        /// <summary>
        /// Deactivate News
        /// </summary>
        /// <param name="uniq_id">uniq_id of News</param>
        /// <returns></returns>
        [HttpPut("Deactivate")]
        public async Task<IActionResult> Deactivate(string uniq_id)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var News = await client.GetAsync(uri + "/findOne?where=(uniq_id,like," + uniq_id + ")");
            if (News.IsSuccessStatusCode)
            {
                var EmpResponse = News.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<News>(EmpResponse);
                if (item != null)
                {
                    item.status = "deactive";
                    var data = JsonConvert.SerializeObject(item);
                    StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");

                    var json = await client.PutAsync(uri + "/" + item.id, queryString);

                    if (json.IsSuccessStatusCode) return Ok(new SystemMessaging(MesagesCode.Update, "News deactivated succesfully!", item));
                    else return BadRequest(new SystemMessaging(MesagesCode.Exception, "News couldn't deactivated!", item));
                }
                else return BadRequest(new SystemMessaging(MesagesCode.NotFound, "News doesn't exist"));
            }

            return BadRequest();
        }


        #endregion


        #region Delete
        /// <summary>
        /// Delete News By UniqId
        /// </summary>
        /// <param name="uniq_id"> Unique id of News </param>
        /// <returns></returns>
        [HttpDelete("Delete/{uniq_id}")]
        public async Task<IActionResult> Delete(string uniq_id)
        {
            SystemMessaging result;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var News = await client.GetAsync(uri + "/findOne?where=(uniq_id,like," + uniq_id + ")");
            if (News.IsSuccessStatusCode)
            {
                var EmpResponse = News.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<News>(EmpResponse);
                if (item != null)
                {
                    var json = await client.DeleteAsync(uri + "/" + item.id);

                    if (json.IsSuccessStatusCode)
                    {

                        EmpResponse = json.Content.ReadAsStringAsync().Result;
                        if (EmpResponse == "1")
                        {
                            return Ok(new SystemMessaging(MesagesCode.Delete, "News deleted succesfully!"));
                        }
                        else return BadRequest(new SystemMessaging(MesagesCode.Delete, "News couldn't deleted!"));

                    }
                }
                else
                {
                    result = new SystemMessaging(MesagesCode.Delete, "News doesn't exist");
                    return BadRequest(result);
                }

            }

            return BadRequest();
        }


        #endregion


    }
}
