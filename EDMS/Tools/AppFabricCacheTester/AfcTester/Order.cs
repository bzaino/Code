using System;

namespace AfcTester
{
    [Serializable]
    public class Order
    {
        public int Id { get; set; }
        
        public string Name {get; set;}
        
        public DateTime DateAndTime {get; set;}
    }
}