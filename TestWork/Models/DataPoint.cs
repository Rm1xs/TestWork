using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace TestWork.Models
{
	[DataContract]
	public class DataPoint
	{
		public DataPoint(double x, double y)
		{
			this.X = x;
			this.Y = y;
		}
		[DataMember(Name = "x")]
		public Nullable<double> X = null;
		[DataMember(Name = "y")]
		public Nullable<double> Y = null;
	}
}