using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DemoWebsite.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DemoWebsite.Controllers
{
    [Route("files")]
    public class FilesController : Controller
    {
        private MailingService mailing;
        protected readonly ILogger<FilesController> _logger;
        private FilesLibrary _filesLibrary;

        public FilesController(
            EmployeeDataContext db, 
            IOptions<SmtpConfig> SmtpConfig,
            FilesLibrary filesLibrary,
            ILogger<FilesController> logger = null)
        {
            mailing = new MailingService(SmtpConfig);
            _filesLibrary = filesLibrary;

            if (logger != null)
            {
                _logger = logger;
            }       
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<FileEntity> db;

            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.GetAsync(_filesLibrary.Url).Result.Content.ReadAsStringAsync();
                    db = JsonConvert.DeserializeObject<IEnumerable<FileEntity>>(response);
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError(e, $"Files Manager API is not available");
                    return BadRequest($"Files Manager API is not available");
                }
            }

            return View(db.ToList());
        }

        [Route("upload")]
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFileCollection uploads)
        {         
            foreach (var uploadedFile in uploads)
            {
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        if (uploadedFile.Length <= 0)
                            continue;

                        if (uploadedFile.Length > 5000000)
                        {
                            break;
                        }

                        var fileName = ContentDispositionHeaderValue.Parse(uploadedFile.ContentDisposition).FileName.Trim('"');

                        using (var content = new MultipartFormDataContent())
                        {
                            content.Add(new StreamContent(uploadedFile.OpenReadStream())
                            {
                                Headers =
                                {
                                    ContentLength = uploadedFile.Length,
                                    ContentType = new MediaTypeHeaderValue(uploadedFile.ContentType)
                                }
                            }, 
                            "File", 
                            fileName);

                            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Username", "User");

                            var response = await httpClient.PostAsync(_filesLibrary.Url, content);
                        }
                    }
                    catch (HttpRequestException e)
                    {
                        _logger.LogError(e, $"Files Manager API is not available");
                        return BadRequest($"Files Manager API is not available");
                    }
                }
            }

            return RedirectToAction("Index");
        }

        [Route("update")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] FileEntity[] files)
        {
            foreach (var file in files)
            {
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        var jsonObject = JsonConvert.SerializeObject(file);
                        var stringContent = new StringContent(jsonObject.ToString(), System.Text.Encoding.UTF8, "application/json");
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Username", "User");
                        var response = await httpClient.PostAsync(_filesLibrary.Url + "update", stringContent);
                    }
                    catch (HttpRequestException e)
                    {
                        _logger.LogError(e, $"Files Manager API is not available");
                        return Json(false);
                    }
                }
            }

            return Json(true);
        }

        [Route("download")]
        [HttpGet]
        public ActionResult Download(int id)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var content = httpClient.GetAsync(_filesLibrary.Url + id).Result.Content;
                    var file = content.ReadAsByteArrayAsync().Result;
                    var fileName = content.Headers.ContentDisposition.FileName;
                    return File(file, "application/octet-stream", fileName);
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError(e, $"Files Manager API is not available");
                    return BadRequest($"Files Manager API is not available");
                }
            }
        }

        [Route("delete")]
        [HttpPost]
        public JsonResult Delete([FromBody]int[] ids)
        {
            foreach (var id in ids)
            {
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        var content = httpClient.DeleteAsync(_filesLibrary.Url + id).Result.Content;
                    }
                    catch (HttpRequestException e)
                    {
                        _logger.LogError(e, $"Files Manager API is not available");
                        return Json(false);
                    }
                }
            }

            return Json(true);
        }
    }
}
