using System.Collections.Generic;

namespace FactoryBot.Tests.Models
{
    public class Model4
    {
        public List<int> SimpleList { get; set; } = default!;
            
        public string[] SimpleArray { get; set; } = default!;

        public List<Model1> ComplexList { get; set; } = default!;
            
        public Model3[] ComplexArray { get; set; } = default!;

        public Dictionary<int, string> SimpleDictionary { get; set; } = default!;

        public Dictionary<Model1, Model2> ComplexDictionary { get; set; } = default!;
    }
}