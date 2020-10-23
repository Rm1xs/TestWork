
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls.Expressions;
using TestWork.EF;

namespace TestWork.Models
{
    public class Map
    {
        [Key]
        public int Id { get; set; }
        public int IdNumb { get; set; }
        public string Url { get; set; }
        public string Speed { get; set; }
        public int HistoryId { get; set; }
        public History History { get; set; }
    }
}