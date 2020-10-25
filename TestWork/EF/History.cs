using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TestWork.Models;

namespace TestWork.EF
{
    public class History
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string URL { get; set; }
        public DateTime DateCheck { get; set; }
        public IEnumerable<Map> ChildURLs { get; set; }
        public string TimeForCheck { get; set; }
        public string MaxValue { get; set; }
        public string MinValue { get; set; }
    }
}