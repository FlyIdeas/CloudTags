using CloudTagsApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudTagsApi.Services
{
    public interface ICrawlerService
    {
        Task<Dictionary<string, int>> getWords(string Url, int TotalWords);        
    }
}
