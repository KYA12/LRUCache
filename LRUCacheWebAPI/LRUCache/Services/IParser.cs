using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LRUCache.Models;

namespace LRUCache.Services
{
    public interface IParser
    {
        void CreateIfNotExists(string fileName);
        List<Student> Read(string fileName);
        void Write(string fileName, List<Student> students);
    }
}
