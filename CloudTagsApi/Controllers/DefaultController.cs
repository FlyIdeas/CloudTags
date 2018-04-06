using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CloudTagsApi.Context;
using CloudTagsApi.Services;
using Newtonsoft.Json;
using CloudTagsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CloudTagsApi.Controllers
{    
    [Route("api/[controller]")]
    public class DefaultController : Controller
    {
        public DataBaseContext _dbContext;
        public ICrawlerService _crawlerService;
        public ISecurityService _securityService;
        
        public DefaultController(
            DataBaseContext dbContext, 
            ICrawlerService crawlerService, 
            ISecurityService securityService)
        {
            _dbContext = dbContext;
            _crawlerService = crawlerService;
            _securityService = securityService;
        }        

        public async Task<IActionResult> Get(string url)
        {
            const int COUNT = 100;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var crawedWords = await _crawlerService.getWords(url, COUNT);

            var list = _dbContext.Words.ToList();
            foreach (var item in list)
            {
                var decryptedWord = _securityService.DecryptA(item.Word);
                if (crawedWords.ContainsKey(decryptedWord))
                {
                    item.Count += crawedWords[decryptedWord];
                    _dbContext.Entry(item).State = EntityState.Modified;
                    crawedWords.Remove(decryptedWord);
                }              
            }

            foreach (var word in crawedWords)
            {
                _dbContext.Words.Add(new WordEntities()
                {
                    Id = _securityService.EncodedStrToHash(word.Key),
                    Word = _securityService.EncryptA(word.Key),
                    Count = word.Value
                });
            }

            await _dbContext.SaveChangesAsync();
            var words = _dbContext.Words.OrderByDescending(w => w.Count).Take(COUNT).ToList();

            var model = words.Select(w => new Word()
            {
                Text = _securityService.DecryptA(w.Word),
                Weight = w.Count
            });

            return Ok(model);
        }
    }
}