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
    public class BlogController : ControllerBase
    {
        #region ctor
        private readonly TokenSettings tokenSettings;
        private readonly string uri = "http://172.16.10.132:3574/nc/aga_project_tyds/api/v1/blogs";
        public BlogController(TokenSettings tokenSettings)
        {
            this.tokenSettings = tokenSettings;
        }

        #endregion


        #region Create
        /// <summary>
        /// Create Blog
        /// </summary>
        /// <param name="model">Blog model </param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Create(BlogVM model)
        {
            var uniq_id = Guid.NewGuid().ToString();
            Blog blog = new Blog()
            {
                created_at = DateTime.Now,
                uniq_id = uniq_id,
                title = model.title,
                content = model.content,
                category_id = model.category_id,
                creator_name = model.creator_name,
                creator_photo = model.creator_photo,
                creator_smedia = model.content,
                alt_text = model.alt_text,
                image = model.image,
                slug = model.slug,
                status = "active"
            };

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var data = JsonConvert.SerializeObject(blog);
            StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");
            var json = await client.PostAsync(uri, queryString);

            if (json.IsSuccessStatusCode)
            {

                var EmpResponse = json.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<Blog>(EmpResponse);
                return Ok(new SystemMessaging(MesagesCode.Insert, "Blog insert succesfully", item));
            }

            return BadRequest();
        }


        #endregion


        #region GetByUnique_Id / FindOne
        /// <summary>
        /// Get Blog By UniqId
        /// </summary>
        /// <param name="uniq_id"> Unique id of Blog </param>
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
                var item = JsonConvert.DeserializeObject<Blog>(EmpResponse);
                if (item == null)
                {
                    var result = new SystemMessaging(MesagesCode.Delete, "Blog doesn't exist");
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
                var items = JsonConvert.DeserializeObject<List<Blog>>(EmpResponse);

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
                var items = JsonConvert.DeserializeObject<List<Blog>>(EmpResponse);

                return Ok(items);

            }
            return BadRequest();
        }

        #endregion


        #region Update
        /// <summary>
        /// Update Blog
        /// </summary>
        /// <param name="model">Blog model</param>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update(Blog model)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var data = JsonConvert.SerializeObject(model);
            StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");
            var json = await client.PutAsync(uri + "/" + model.id, queryString);
            if (json.IsSuccessStatusCode) return Ok(new SystemMessaging(MesagesCode.Update, "Blog updated succesfully!", model));
            else return BadRequest(new SystemMessaging(MesagesCode.Exception, "Blog couldn't update!", model));

        }


        #endregion


        #region Activate
        /// <summary>
        /// Activate Blog
        /// </summary>
        /// <param name="uniq_id">uniq_id of Blog</param>
        /// <returns></returns>
        [HttpPut("Activate")]
        public async Task<IActionResult> Activate(string uniq_id)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var Blog = await client.GetAsync(uri + "/findOne?where=(uniq_id,like," + uniq_id + ")");
            if (Blog.IsSuccessStatusCode)
            {
                var EmpResponse = Blog.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<Blog>(EmpResponse);
                if (item != null)
                {
                    item.status = "active";
                    var data = JsonConvert.SerializeObject(item);
                    StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");

                    var json = await client.PutAsync(uri + "/" + item.id, queryString);

                    if (json.IsSuccessStatusCode) return Ok(new SystemMessaging(MesagesCode.Update, "Blog activated succesfully!", item));
                    else return BadRequest(new SystemMessaging(MesagesCode.Exception, "Blog couldn't activated!", item));
                }
                else return BadRequest(new SystemMessaging(MesagesCode.NotFound, "Blog doesn't exist"));
            }

            return BadRequest();
        }


        #endregion


        #region Deactivate
        /// <summary>
        /// Deactivate Blog
        /// </summary>
        /// <param name="uniq_id">uniq_id of Blog</param>
        /// <returns></returns>
        [HttpPut("Deactivate")]
        public async Task<IActionResult> Deactivate(string uniq_id)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var Blog = await client.GetAsync(uri + "/findOne?where=(uniq_id,like," + uniq_id + ")");
            if (Blog.IsSuccessStatusCode)
            {
                var EmpResponse = Blog.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<Blog>(EmpResponse);
                if (item != null)
                {
                    item.status = "deactive";
                    var data = JsonConvert.SerializeObject(item);
                    StringContent queryString = new StringContent(data, Encoding.UTF8, "application/json");

                    var json = await client.PutAsync(uri + "/" + item.id, queryString);

                    if (json.IsSuccessStatusCode) return Ok(new SystemMessaging(MesagesCode.Update, "Blog deactivated succesfully!", item));
                    else return BadRequest(new SystemMessaging(MesagesCode.Exception, "Blog couldn't deactivated!", item));
                }
                else return BadRequest(new SystemMessaging(MesagesCode.NotFound, "Blog doesn't exist"));
            }

            return BadRequest();
        }


        #endregion


        #region Delete
        /// <summary>
        /// Delete Blog By UniqId
        /// </summary>
        /// <param name="uniq_id"> Unique id of Blog </param>
        /// <returns></returns>
        [HttpDelete("Delete/{uniq_id}")]
        public async Task<IActionResult> Delete(string uniq_id)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("xc-auth", tokenSettings.token);
            var Blog = await client.GetAsync(uri + "/findOne?where=(uniq_id,like," + uniq_id + ")");
            if (Blog.IsSuccessStatusCode)
            {
                var EmpResponse = Blog.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<Blog>(EmpResponse);
                if (item != null)
                {
                    var json = await client.DeleteAsync(uri + "/" + item.id);

                    if (json.IsSuccessStatusCode)
                    {
                        EmpResponse = json.Content.ReadAsStringAsync().Result;
                        if (EmpResponse == "1")
                        {
                            return Ok(new SystemMessaging(MesagesCode.Delete, "Blog deleted succesfully!"));
                        }
                        else return BadRequest(new SystemMessaging(MesagesCode.Delete, "Blog couldn't deleted!"));

                    }
                }
                else return BadRequest(new SystemMessaging(MesagesCode.Delete, "Blog doesn't exist"));
            }

            return BadRequest();
        }


        #endregion
    }
}

