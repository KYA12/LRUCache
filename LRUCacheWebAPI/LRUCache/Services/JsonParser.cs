using System;
using System.Collections.Generic;
using System.IO;
using LRUCache.Models;
using Newtonsoft.Json;

namespace LRUCache.Services
{
    public class JsonParser
    {
        public static void CreateIfNotExists(string fileName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(),
            fileName);
            if (!File.Exists(path))
            {
                List<Student> students = new List<Student>();
                students.Add(new Student { Id = 1, FirstName = "Petro", LastName = "Sidorenko", University = "KPI" });
                students.Add(new Student { Id = 2, FirstName = "Ivan", LastName = "Petrenko", University = "KNU" });
                students.Add(new Student { Id = 3, FirstName = "Evgen", LastName = "Kovalenko", University = "KPI" });
                students.Add(new Student { Id = 4, FirstName = "Ivan", LastName = "Boyko", University = "KNU" });
                students.Add(new Student { Id = 5, FirstName = "Andriy", LastName = "Kovpak", University = "KPI" });
                students.Add(new Student { Id = 6, FirstName = "Ivan", LastName = "Petrenko", University = "KNU" });
                students.Add(new Student { Id = 7, FirstName = "Olena", LastName = "Petrenko", University = "KPI" });
                students.Add(new Student { Id = 8, FirstName = "Evgen", LastName = "Zapashniy", University = "KNU" });
                students.Add(new Student { Id = 9, FirstName = "Mykola", LastName = "Tarasov", University = "KPI" });
                Write(fileName, students);
            }
        }
        public static List<Student> Read(string fileName)
        {
            List<Student> students = new List<Student>();
            var path = Path.Combine(Directory.GetCurrentDirectory(), 
            fileName);
            string jsonResult;
            using (StreamReader streamReader = new StreamReader(path))
            {
                jsonResult = streamReader.ReadToEnd();
                students = JsonConvert.DeserializeObject<List<Student>>(jsonResult);
                return students;
            }
        }
        public static void Write(string fileName, List<Student> Students)
        {
            var path = Path.Combine(
            Directory.GetCurrentDirectory(),
            fileName);
            string jSONString = JsonConvert.SerializeObject(Students);
            using (var streamWriter = File.CreateText(path))
            {
                streamWriter.Write(jSONString);
            }
        }
    }
}