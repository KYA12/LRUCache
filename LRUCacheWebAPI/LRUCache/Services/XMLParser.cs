using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using LRUCache.Models;

namespace LRUCache.Services
{
    public class XMLParser
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
            XmlSerializer serializer = new XmlSerializer(typeof(List<Student>));
            using (FileStream stream = File.OpenRead(path))
            {
                students = (List<Student>)serializer.Deserialize(stream);
                return students;
            }
        }
        public static void Write(string fileName, List<Student> Students)
        {
            var path = Path.Combine(
            Directory.GetCurrentDirectory(),
            fileName);
            XmlSerializer serializer = new XmlSerializer(typeof(List<Student>));
            using (FileStream stream = File.Create(path))
            {
                serializer.Serialize(stream, Students);
            }
        }
    }
}

