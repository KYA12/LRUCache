using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using LRUCache.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Linq;
using System.IO;
using LRUCache.Cache;
using Microsoft.Extensions.Logging;
using LRUCache.Services;

namespace LRUCache.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly ILogger logger;
        private readonly string fileNameJson;
        private readonly string fileNameXML;
        private readonly ICacheService service;
        private readonly LRUCache<int, Student> cache;
        private readonly string dataType;

        public HomeController(IConfiguration iConfiguration, ILoggerFactory loggerFactory, ICacheService iService)
        {
            configuration = iConfiguration;
            fileNameJson = configuration["FileNameJson"];
            fileNameXML = configuration["FileNameXML"];
            dataType = configuration["DataType"];
            service = iService;
            if (dataType == "json")
                JsonParser.CreateIfNotExists(fileNameJson);
            if (dataType == "xml")
                XMLParser.CreateIfNotExists(fileNameXML);
            logger = loggerFactory.CreateLogger("FileLogger");
            cache = service.GetCache();
        }
        // GET api/home
        [HttpGet]
        public IActionResult Get()
        {
            logger.LogInformation($"Method Get api/home");
            List<Student> students = new List<Student>();
            List<Student> cachedStudents = new List<Student>();
            if (dataType == "json")
                students = JsonParser.Read(fileNameJson);
            if (dataType == "xml")
                students = XMLParser.Read(fileNameXML);
            if (students == null) return NotFound();
            List<Student> newStudents = new List<Student>();

            if (cache.Count != 0)
            {
                cachedStudents = cache.GetAllValues();
                for (int i = 0; i < cache.Count; i++)
                {
                    newStudents.Add(cachedStudents[i]);
                }
                                           
                foreach (Student s in newStudents)
                {
                    logger.LogInformation($"Students in the initial cache: {s.FirstName} {s.LastName}");
                }

                students.RemoveAll(x => newStudents.Contains(x));

                for (int i = 0; i < students.Count; i++)
                {
                    newStudents.Add(students[i]);
                    cache.Add(students[i].Id, students[i]);
                    string name = students[i].FirstName;
                    logger.LogInformation($"Not in the cache: {name} {students[i].LastName}");
                    logger.LogInformation($"Added to the cache: {name} {students[i].LastName}");
                }
            }
            else
            {
                for (int i = 0; i < students.Count; i++)
                {
                    cache.Add(students[i].Id, students[i]);
                    newStudents.Add(students[i]);
                    string name = newStudents[i].FirstName;
                    logger.LogInformation($"Added to new cache: {name} {students[i].LastName}");
                }
            }
            logger.LogInformation($"Cache Count: {cache.Count}");
            logger.LogInformation($"Cache Capacity: {cache.Capacity}");
            foreach (Student s in cache.GetAllValues())
            {
                logger.LogInformation($"Students in new cache: {s.FirstName} {s.LastName}");
            }
            return Ok(newStudents);
        }

        // GET api/home/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            logger.LogInformation($"Method Get api/home/{id}");
            List<Student> students = new List<Student>();
            List<Student> cachedStudents = new List<Student>();
            Student student = new Student();
           
            if (cache.Count != 0)
            {
                cachedStudents = cache.GetAllValues();
                for (int i = 0; i < cache.Count; i++)
                {
                    if (cachedStudents[i].Id == id)
                    {
                        Student cachedStudent = new Student();
                        if (cache.TryGetValue(cachedStudents[i].Id,out cachedStudent))
                        {
                            logger.LogInformation($"Cached Student: {cachedStudent.FirstName} {cachedStudent.LastName}");
                            foreach (Student s in cache.GetAllValues())
                            {
                                logger.LogInformation($"Students in cache: {s.FirstName} {s.LastName}");
                            }
                            return Ok(cachedStudents[i]);
                        }
                        
                    }
                        
                }
                if (dataType == "json")
                    students = JsonParser.Read(fileNameJson);
                if (dataType == "xml")
                    students = XMLParser.Read(fileNameXML);
                int ind = students.FindIndex(x => x.Id == id);
                if (ind == -1) return NotFound();
                student = students[ind];
                cache.Add(student.Id, student);
                logger.LogInformation($"NonCached Student: {student.FirstName}, {student.LastName}");
                logger.LogInformation($"Added to cache Student: {student.FirstName}, {student.LastName}");
                foreach (Student s in cache.GetAllValues())
                {
                    logger.LogInformation($"Students in cache: {s.FirstName}");
                }
                return Ok(student);
            }
            if (dataType == "json")
                students = JsonParser.Read(fileNameJson);
            if (dataType == "xml")
                students = XMLParser.Read(fileNameXML);
            int index = students.FindIndex(x => x.Id == id);
            if (index == -1) return NotFound();
            student = students[index];
            cache.Add(0, student);
            logger.LogInformation($"NonCached Student: {student.FirstName}, {student.LastName}");
            logger.LogInformation($"Added to cache Student: {student.FirstName}, {student.LastName}");
            foreach (Student s in cache.GetAllValues())
            {
                logger.LogInformation($"Students in cache: {s.FirstName} {s.LastName}");
            }
            return Ok(student);
        }

        // POST api/home
        [HttpPost]
        public IActionResult Post([FromBody] Student student)
        {
            List<Student> students = new List<Student>();
            List<Student> cached = new List<Student>();
            if (dataType == "json")
                students = JsonParser.Read(fileNameJson);
            if (dataType == "xml")
                students = XMLParser.Read(fileNameXML);
            logger.LogInformation($"Method POST api/home");
            cached = cache.GetAllValues();
            for (int i = 0; i < cache.Count; i++)
            {
                if (cached[i].Id == student.Id)
                {
                    Student cachedStudent = new Student();
                    if (cache.TryGetValue(cached[i].Id, out cachedStudent))
                    {
                        logger.LogInformation($"Cached Student: {cachedStudent.FirstName} {cachedStudent.LastName}");
                        Student _st = students.FirstOrDefault(x => x.Id == cachedStudent.Id);
                        if (_st == null)
                        {
                            logger.LogInformation($"Create cached student in file: {student.FirstName}, {student.LastName}");
                            students.Add(student);
                        }
                        else
                        {
                            logger.LogInformation($"Update cached student in file: {student.FirstName}, {student.LastName}");
                            int index = students.FindIndex(x => x.Id == cachedStudent.Id);
                            students[index] = student;
                        }
                        foreach (Student s in cache.GetAllValues())
                        {
                            logger.LogInformation($"Students in cache: {s.FirstName} {s.LastName}");
                        }
                        if (dataType == "json")
                            JsonParser.Write(fileNameJson, students);
                        if (dataType == "xml")
                            XMLParser.Write(fileNameXML, students);
                    }
                    return Ok(student);
                }

            }
                   
            logger.LogInformation($"NonCached Student: {student.FirstName}, {student.LastName}");
            Student _student = students.FirstOrDefault(x => x.Id == student.Id);
            if (_student == null)
            {
                logger.LogInformation($"Create new uncached Student in file: {student.FirstName}, {student.LastName}");
                students.Add(student);
            }
            else
            {
                logger.LogInformation($"Update noncached Student in file: {student.FirstName}, {student.LastName}");
                int index = students.FindIndex(x => x.Id == student.Id);
                students[index] = student;
            }
    
            cache.Add(student.Id, student);
            logger.LogInformation($"Added to cache Student: {student.FirstName}, {student.LastName}");
            if (dataType == "json")
                JsonParser.Write(fileNameJson, students);
            if (dataType == "xml")
                XMLParser.Write(fileNameXML, students);
            foreach (Student s in cache.GetAllValues())
            {
                logger.LogInformation($"Students in cache: {s.FirstName} {s.LastName}");
            }
            return Ok(student);
        }
        // DELETE api/home/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            List<Student> students = new List<Student>();
            Student student = new Student();
            List<Student> cached = new List<Student>();
            if (dataType == "json")
                students = JsonParser.Read(fileNameJson);
            if (dataType == "xml")
                students = XMLParser.Read(fileNameXML);
            int index = students.FindIndex(x => x.Id == id);
            if (index == -1) return NotFound();
            student = students[index];
            students.RemoveAt(index);
            if (dataType == "json")
                JsonParser.Write(fileNameJson, students);
            if (dataType == "xml")
                XMLParser.Write(fileNameXML, students);
            logger.LogInformation($"Method Delete api/home/{id}");
            logger.LogInformation($"Deleted Student from file: {student.FirstName}, {student.LastName}");
            foreach (Student s in cache.GetAllValues())
            {
                logger.LogInformation($"Students in cache: {s.FirstName} {s.LastName}");
            }
            return NoContent();
        }
    }
}
