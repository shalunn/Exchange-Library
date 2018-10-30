using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FilesManager.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace FilesManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesLibraryController : ControllerBase
    {
        private readonly FilesDataContext _db;
        private IHostingEnvironment _appEnvironment;

        public FilesLibraryController(FilesDataContext db, IHostingEnvironment appEnvironment)
        {
            _db = db;
            _appEnvironment = appEnvironment;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<FileEntity>> Get()
        {
            return _db.Files.ToList();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var path = _db.Files.FirstOrDefault(x => x.Id == id).Path;
            var filename = _db.Files.FirstOrDefault(x => x.Id == id).Name;
            var type = _db.Files.FirstOrDefault(x => x.Id == id).Type.ToLower();
            var result = PhysicalFile(_appEnvironment.WebRootPath + path, "application/octet-stream", filename + '.' + type);

            return result;
        }

        [HttpPost]
        public async Task<IActionResult> Post(string targetIdStr, IFormFile file)
        {
            if (file.Length > 5000000 || file.FileName.Length > 50)
            {
                return BadRequest();
            }            

            var header = Request.Headers.TryGetValue("Authorization", out StringValues username);
            var user = username.Last().Split(' ')[1];

            var type = file.FileName.Split('.').Last();
            var name = file.FileName.Substring(0, file.FileName.Length - (type.Length + 1));            
            var guid = DateTime.Now.Ticks;

            var filename = string.Format(@"{0}.{1}", guid, type);
            string path = "/Files/" + filename;

            using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }            

            FileEntity fileEntity = new FileEntity
            {
                Name = name,
                Type = type.ToUpper(),
                Path = path,
                Size = file.Length,
                Guid = guid.ToString(),
                Username = user,
                Created = DateTime.Now,
                Modified = DateTime.Now
            };

            _db.Files.Add(fileEntity);
            _db.SaveChanges();

            return Ok();
        }

        [Route("update")]
        [HttpPost]
        public async Task<IActionResult> Post(string targetIdStr, FileEntity file)
        {
            var header = Request.Headers.TryGetValue("Authorization", out StringValues username);
            var user = username.Last().Split(' ')[1];

            var fileEntity = _db.Files.FirstOrDefault(x => x.Id == file.Id);
            fileEntity.Name = file.Name;
            _db.Files.Update(fileEntity);

            _db.SaveChanges();

            return Ok();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public bool Delete(int id)
        {
            var entry = _db.Files.FirstOrDefault(x => x.Id == id);

            if (entry != null)
            {
                var path = entry.Path;
                _db.Entry(entry).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

                FileEntity fileEntity = new FileEntity { Id = id };
                _db.Files.Remove(fileEntity);
                _db.SaveChanges();

                if (System.IO.File.Exists(_appEnvironment.WebRootPath + path))
                {
                    System.IO.File.Delete(_appEnvironment.WebRootPath + path);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
