using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LRUCache.Models
{
    public class Student : IEquatable<Student>
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string University { get; set; }
        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == this.GetHashCode();
        }
        public bool Equals(Student obj)
        {
            return obj.Id == this.Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
        public override string ToString()
        {
            return string.Format("[Id = {0};]", Id);
        }
    }
}
