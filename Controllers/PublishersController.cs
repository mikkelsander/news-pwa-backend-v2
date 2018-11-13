using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PWANews.Entities;
using PWANews.Models;
using PWANews.Services;

namespace PWANews.Controllers
{
    [Route("/publishers")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private INewsClient _newsService;

        public PublishersController(INewsClient newsService)
        {
            _newsService = newsService;
        }

        // GET: api/Publishers
        [HttpGet]
        public async Task<List<Publisher>> Get()
        {

            var publishers = await _newsService.GetPublishers();
            return publishers;
        }

        //// GET: api/Publishers/5
        //[HttpGet("{id}", Name = "Get")]
        //public async Task<List<ArticleDTO>> Get(string id)
        //{
         
        //}

        //// POST: api/Publishers
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT: api/Publishers/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
