using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbgCensus.Rest.Abstractions
{
    public interface ICensusRestClient
    {
        Task<T?> GetAsync<T>(string endPoint) where T : new();
    }
}
