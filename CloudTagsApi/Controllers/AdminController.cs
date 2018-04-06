using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CloudTagsApi.Context;
using CloudTagsApi.Models;
using CloudTagsApi.Services;

namespace CloudTagsApi.Controllers
{
    [Route("api/[controller]")]
    public class AdminController : Controller
    {
        public DataBaseContext _dbContext;
        public ISecurityService _securityService;

        public AdminController(
            DataBaseContext dbContext,
            ISecurityService securityService)
        {
            _dbContext = dbContext;
            _securityService = securityService;
        }

        [HttpGet]
        public IEnumerable<Word> GetWords()
        {
            var words = _dbContext.Words.OrderByDescending(w => w.Count).ToList();

            var model = words.Select(w => new Word()
            {
                Text = _securityService.DecryptA(w.Word),
                Weight = w.Count
            });

            return model;
        }
    }
}