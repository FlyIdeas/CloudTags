using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudTagsApi.Services
{
    public interface ISecurityService
    {        
        string EncodedStrToHash(string Str);
        string DecryptA(string Str);
        string EncryptA(string Code);        
    }
}
